using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace ChineseAbs.ABSManagement
{
    [Flags]
    public enum AuthorityType
    {
        Undefined = ~0,
        CreateProject = 0x01,
        ModifyTask = 0x02,
        ModifyModel = 0x04,
    }

    public class AuthorityManager : BaseManager
    {
        public AuthorityManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public List<int> GetAuthorizedProjectIds(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new List<int>() ;
            }

            List<int> projectIds = new List<int>();
            
            projectIds = m_db.Query<int>("SELECT project_id FROM dbo.Project where record_status_id <> @0",
                    (int)ChineseAbs.ABSManagement.Models.RecordStatus.Deleted).ToList();


            var dbAdapter = new DBAdapter();
            var projects = dbAdapter.Project.GetProjects(projectIds);

            //本地版，移除发行协作平台中的Projects（本地版暂不支持发行协作平台）
            //projects.RemoveAll(x => x.ProjectSeriesId.HasValue);
            projectIds = projects.ConvertAll(x => x.ProjectId);

            return projectIds == null ? new List<int>() : projectIds;
        }

        public List<int> GetAuthorizedProjectIds()
        {
            if (UserInfo == null)
            {
                return new List<int>();
            }

            return GetAuthorizedProjectIds(UserInfo.UserName);
        }

        public List<int> GetEnterpriseIdInAuthorizedProjectTable()
        {
            var sql = "SELECT DISTINCT(enterprise_id) FROM dbo.ProjectAuthority where enterprise_id > 0";
            var enterpriseIds = m_db.Fetch<int>(sql);
            return enterpriseIds;
        }

        public int? GetEnterpriseId(string userName)
        {
            if (CommUtils.IsLocalDeployed())
            {
                return -9527;
            }

            var vwProjects = m_db.Query<ABSMgrConn.VwUserEnterprise>(
                "SELECT * FROM dbo.vw_userEnterprise WHERE UserName = @0", userName);

            if (vwProjects.Count() == 0)
            {
                return null;
            }

            return vwProjects.Single().enterprise_id;
        }

        public int? GetEnterpriseId()
        {
            return GetEnterpriseId(UserInfo.UserName);
        }

        public bool IsAuthorizedUser(string userName)
        {
            return true;
        }

        public bool IsOpenFunction(string userName, string strFunctionId)
        {
            int id = int.Parse(strFunctionId);
            string sql = @" select count(*) from [dbo].[vw_enterpriseFunctions] e join [dbo].[vw_userEnterprise] u 
                        on e.enterprise_id = u.enterprise_id where e.function_id= @0  and u.username= @1                       
";
            int count = m_db.ExecuteScalar<int>(sql, id, userName);
            return count >= 1;
        }

        public bool IsEnterpriseAdministrator(string userName)
        {
            int? enterpriseId = GetEnterpriseId();
            if (enterpriseId != null)
            {
                string sql = @"select count(*) from " + m_db.ChineseABSDB + @"[cpy].[enterpriseAdministrator] e
    join " + m_db.ChineseABSDB + @"[dbo].[aspnet_users] u on  e.user_id = u.userid where u.username = @0 
    and e.enterprise_id=@1 ";
                var count = m_db.ExecuteScalar<int>(sql, userName, enterpriseId);
                if (count == 1)
                    return true;
            }
            return false;

        }

        public bool IsProfessionTypeAdmin(string userName)
        {
            string sql = @"select count(*) from " + m_db.ChineseABSDB + @"cpy.EnterpriseAdministrator aa join " + m_db.ChineseABSDB + @"cpy.EnterpriseApplication ea on aa.enterprise_id = ea.enterprise_id
join " + m_db.ChineseABSDB + @"dbo.aspnet_users u on u.userid = aa.user_id where u.username = @0 and ea.enterprise_type_id = 3";
            var count = m_db.ExecuteScalar<int>(sql, userName);
            return count > 0;
        }

        public bool AuthorizeDesignProject(int projectId, AuthorityType authorityType)
        {
            if (UserInfo == null || string.IsNullOrEmpty(UserInfo.UserName))
            {
                return false;
            }

            var authorities = CommUtils.GetEnumFlags(authorityType);
            Func<AuthorityType, int> parseAuthority = (authority) => authorities.Contains(authority) ? 1 : 0;

            var sql = "SELECT * FROM dbo.EditProductAuthority WHERE user_name = @0 AND modify_project_id = @1";
            var records = m_db.Query<ABSMgrConn.TableEditProductAuthority>(sql, UserInfo.UserName, projectId);
            CommUtils.Assert(records.Count() < 2, "Get authority [userName=" + UserInfo.UserName + "] [projectId=" + projectId + "] failed");
            if (records.Count() == 0)
            {
                var obj = new ABSMgrConn.TableEditProductAuthority();
                obj.user_name = UserInfo.UserName;
                obj.modify_model_authority = parseAuthority(AuthorityType.ModifyModel);
                obj.modify_task_authority = parseAuthority(AuthorityType.ModifyTask);
                obj.create_product_authority = 0;
                obj.modify_project_id = projectId;
                m_db.Insert("EditProductAuthority", "product_edit_authority_id", true, obj);
            }
            else
            {
                var obj = records.Single();
                obj.modify_model_authority = parseAuthority(AuthorityType.ModifyModel);
                obj.modify_task_authority = parseAuthority(AuthorityType.ModifyTask);
                m_db.Update("EditProductAuthority", "product_edit_authority_id", obj);
            }
            return true;
        }

        public bool AuthorizeToEnterprise(int projectId)
        {
            if (UserInfo == null || string.IsNullOrEmpty(UserInfo.UserName))
            {
                return false;
            }

            var enterpriseId = GetEnterpriseId();
            if (!enterpriseId.HasValue)
            {
                throw new ApplicationException("Get Enterprise info failed user name: [" + UserInfo.UserName + "]");
            }

            var projectAuthority = new ProjectAuthority();
            projectAuthority.EnterpriseId = enterpriseId.Value;
            projectAuthority.ProjectId = projectId;

            var newId = m_db.Insert("ProjectAuthority", "project_authority_id", true, projectAuthority.GetTableObject());
            return (int)newId > 0;
        }

        public bool IsAuthorized(int projectId, string userName)
        {
            return true;
        }

        public bool IsAuthorized(int projectId)
        {
            if (UserInfo == null)
            {
                return false;
            }

            return IsAuthorized(projectId, UserInfo.UserName);
        }

        public string GetAuthorizedEnterpriseName(int projectId)
        {
            if (CommUtils.IsLocalDeployed())
            {
                return CommUtils.GetEnterpriseName();
            }

            var projectAuthority = m_db.Query<ABSMgrConn.TableProjectAuthority>(
                "SELECT * FROM dbo.ProjectAuthority WHERE project_id = @0", projectId);
            if (projectAuthority.Count() <= 0)
            {
                return null;
            }

            if (projectAuthority.Count() > 1)
            {
                throw new ApplicationException("One product can authorized to only one enterprise.");
            }

            var enterpriseId = projectAuthority.Single().enterprise_id;
            var vwEnterpriseApplication = m_db.Query<ABSMgrConn.VwEnterpriseApplication>(
                    "SELECT * FROM " + m_db.ChineseABSDB + @"[cpy].[EnterpriseApplication] WHERE enterprise_id = @0", enterpriseId);

            if (vwEnterpriseApplication.Count() == 0)
            {
                return null;
            }

            return vwEnterpriseApplication.Single().enterprise_name;
        }

        public bool HasUserInfo()
        {
            return UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserName);
        }

        public List<int> GetAuthorizedProjectIds(AuthorityType authorityType)
        {
            if (!HasUserInfo())
            {
                return new List<int>();
            }

            string sql = string.Empty;
            switch (authorityType)
            {
                case AuthorityType.ModifyModel:
                    sql = "SELECT * FROM dbo.EditProductAuthority WHERE user_name = @0 AND modify_model_authority = @1";
                    break;
                case AuthorityType.ModifyTask:
                    sql = "SELECT * FROM dbo.EditProductAuthority WHERE user_name = @0 AND modify_task_authority = @1";
                    break;
                default:
                    throw new ApplicationException("Get authorized project id failed.");
            }

            var records = m_db.Query<ABSMgrConn.TableEditProductAuthority>(sql, UserInfo.UserName, 1);
            return records.ToList().Where(x => x.modify_project_id.HasValue)
                .ToList().ConvertAll(x => x.modify_project_id.Value);
        }

        public bool IsAuthorized(AuthorityType authorityType)
        {
            if (authorityType == AuthorityType.Undefined)
            {
                return false;
            }
            if (authorityType == AuthorityType.CreateProject)
            {
                var editProductAuthority = m_db.Query<ABSMgrConn.TableEditProductAuthority>(
                    "SELECT * FROM dbo.EditProductAuthority WHERE user_name = @0 AND create_product_authority = @1",
                    UserInfo.UserName, 1);
                return HasUserInfo() && editProductAuthority.Count() > 0;
            }
            else
            {
                var projectIds = GetAuthorizedProjectIds(authorityType);
                return projectIds.Count > 0;
            }
        }

        public bool IsUserExist(string userName)
        {
            return UserService.GetUserByName(userName) != null;
        }

        public string GetUserRealName()
        {
            return GetUserRealName(UserInfo.UserName);
        }

        public string GetUserRealName(string userName)
        {
            string sql = "select name from " + m_db.ChineseABSDB + @"[web].[AccountApplication] where username = @0 ";
            return m_db.ExecuteScalar<string>(sql, userName);
        }

        public UserProfile GetUserProfile(string userName)
        {
            var records = m_db.Fetch<ABSMgrConn.VwUserProfile>("SELECT name, username,avatar_file_code as avatar_path FROM " + m_db.ChineseABSDB + @"[web].[AccountApplication] "
                + " LEFT JOIN " + m_db.ChineseABSDB + @"[dbo].[UserProfile] on username = user_name "
                + " WHERE username = @userName", new { userName });

            if (records.Count == 0)
            {
                return null;
            }

            if (records.Count > 1)
            {
                throw new ApplicationException("Duplicate userName [" + userName + "] have been found!");
            }

            var record = records.First();
            return new UserProfile
            {
                RealName = record.name,
                UserName = record.username,
                AvatarPath = record.avatar_path
            };
        }
        

        public string GetAvatarUrl()
        {
            string sql = "SELECT avatar_path FROM " + m_db.ChineseABSDB + @"[dbo].[UserProfile] WHERE user_name = @0 ";
            string avatarUrl = m_db.ExecuteScalar<string>(sql, UserInfo.UserName);
            if (string.IsNullOrEmpty(avatarUrl))
            {
                avatarUrl = "/Images/Avatar/headerDefault.jpg";
            }
            return avatarUrl;
        }

        public List<LockMenu> GetFunctionsByIsFree(int i)
        {
            var list = m_db.Fetch<LockMenu>("select function_id as id, function_Name as name from " + m_db.ChineseABSDB + @"cpy.functions where isOld = 1 and isFree = @0", i);
            return list;
        }

        public List<LockMenu> GetUnOpenFunctionsByRelateUserName(string username)
        {
            var list = m_db.Fetch<LockMenu>(@" select function_id as id,function_Name as name from " + m_db.ChineseABSDB + @"cpy.functions where isOld = 1 and function_id not in (
               select distinct function_id from " + m_db.ChineseABSDB + @"cpy.enterpriseFunctions where enterprise_id in
                ( select enterprise_id from " + m_db.ChineseABSDB + @"cpy.enterpriseRelateUsers where user_id = 
                        (
                            select userid from " + m_db.ChineseABSDB + @"dbo.aspnet_users where username = @0
                        )
                ))", username);
            return list;
        }

        public List<LockMenu> GetEnterpriseUnFreeFunctions(string username)
        {
            var list = m_db.Fetch<LockMenu>(@"select function_id as id,function_Name as name from " + m_db.ChineseABSDB + @"cpy.functions where isOld = 1 and function_id  in (
            select distinct function_id from " + m_db.ChineseABSDB + @"cpy.enterpriseFunctions where enterprise_id in
                (
                    select enterprise_id from " + m_db.ChineseABSDB + @"cpy.enterpriseRelateUsers where user_id = 
                        (
                            select userid from " + m_db.ChineseABSDB + @"dbo.aspnet_users where username = @0
                        )
                )
) and isFree = 0", username);
            return list;
        }

        public List<AuthedAccount> GetAllAuthedAccount()
        {
            return UserService.GetAllUser().Select(u => new AuthedAccount { RealName = u.Name, UserName = u.UserName }).ToList();
        }

        public string GetNameByUserName(string userName)
        {
            var u = UserService.GetUserByName(userName);
            if (u != null)
                return u.Name;
            else
                return userName;
        }


        public List<AuthedAccount> GetAllAuthorityUserInfo()
        {
            var records = m_db.Fetch<AuthedAccount>(@"SELECT DISTINCT(user_name) as UserName, name as RealName FROM dbo.EditProductAuthority
            LEFT JOIN " + m_db.ChineseABSDB + @"web.AccountApplication on user_name = username ");
            return records;
        }

        public List<int> GetAuthorizedProjectIdsByUsername(string username)
        {
            var records = GetAuthorityByUsername(username);
            var projectIds = records.ConvertAll(x => x.ModifyProjectId);
            return projectIds;
        }

        public List<EditProductAuthority> GetAuthorityByUsername(string username)
        {
            var records = m_db.Query<ABSMgrConn.TableEditProductAuthority>(@"SELECT * FROM dbo.EditProductAuthority WHERE user_name = @0", username);
            return records.ToList().ConvertAll(x => new EditProductAuthority(x)).ToList();
        }

        public List<EnterpriseInfo> GetAllEnterpriseName()
        {
            var records = m_db.Fetch<EnterpriseInfo>(@"SELECT enterprise_id as EnterpriseId,enterprise_name as EnterpriseName FROM " + m_db.ChineseABSDB + @"[cpy].[EnterpriseApplication] where isLocked = 0 ");

            return records;
        }

        public VwEnterpriseApplication GetEnterpriseByName(string enterpriseName)
        {
            var records = m_db.Query<ABSMgrConn.VwEnterpriseApplication>(@"SELECT * FROM " + m_db.ChineseABSDB + @"[cpy].[EnterpriseApplication] where enterprise_name = @0", enterpriseName);
            if (records.Count() == 0)
            {
                return null;
            }

            if (records.Count() > 1)
            {
                throw new ApplicationException("Get data failed,data of number greater than two,please contact the admin.");
            }

            return records.First();
        }

        public bool HasEnterpriseByName(string enterpriseName)
        {
            if (CommUtils.IsLocalDeployed())
            {
                return enterpriseName == CommUtils.GetEnterpriseName();
            }
            else
            {
                var result = GetEnterpriseByName(enterpriseName);
                return result != null;
            }
        }

        public List<int> GetProjectIdsByEnterpriseId(int enterpriseId)
        {
            var projectAuthority = m_db.Query<ABSMgrConn.TableProjectAuthority>(
                "SELECT * FROM dbo.ProjectAuthority WHERE enterprise_id = @0", enterpriseId);
            var projectIds = projectAuthority.ToList().ConvertAll(x => x.project_id);

            return projectIds;
        }

        public EditProductAuthority NewProductAuthority(EditProductAuthority editProductAuthority)
        {
            var records = HasProductAuthority(editProductAuthority.Username, editProductAuthority.ModifyProjectId);
            CommUtils.Assert(!records, "用户[{0}]已存在该权限", editProductAuthority.Username);

            var newId = Insert("dbo.EditProductAuthority", "product_edit_authority_id", editProductAuthority.GetTableObject());
            editProductAuthority.productEditAuthorityId = newId;
            return editProductAuthority;
        }

        public EditProductAuthority GetProductAuthority(string username, int modifyProjectId)
        {
            var records = m_db.Fetch<ABSMgrConn.TableEditProductAuthority>(
                @"SELECT * FROM dbo.EditProductAuthority where user_name = @0 and modify_project_id = @1",
                username, modifyProjectId);
            if (records.Count == 1)
            {
                return new EditProductAuthority(records.First());
            }

            if (records.Count > 1)
            {
                throw new ApplicationException("Get product authority failed,data of number greater than two,please contact the admin.");
            }

            return null;
        }

        public bool HasProductAuthority(string username, int modifyProjectId)
        {
            var records = GetProductAuthority(username, modifyProjectId);

            return records != null;
        }

        public int DeleteProductAuthority(EditProductAuthority editProductAuthority)
        {
            return m_db.Delete("dbo.EditProductAuthority", "product_edit_authority_id", editProductAuthority.GetTableObject());
        }

        public int UpdateProductAuthority(EditProductAuthority editProductAuthority)
        {
            var productAuthorityTable = editProductAuthority.GetTableObject();

            return m_db.Update("dbo.EditProductAuthority", "product_edit_authority_id", productAuthorityTable, editProductAuthority.productEditAuthorityId);
        }

        public List<EditProductAuthority> GetProductAuthorityByUsername(string username)
        {
            var records = m_db.Fetch<ABSMgrConn.TableEditProductAuthority>(
                @"SELECT * FROM dbo.EditProductAuthority where user_name = @0 and create_product_authority = 1",
                username);

            if (records.Count == 0)
            {
                return new List<EditProductAuthority>();
            }

            return records.ToList().ConvertAll(x => new EditProductAuthority(x)).ToList();
        }

    }
}
