using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class EditProductAuthorityController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAllAuthorityUserInfo()
        {
            return ActionUtils.Json(() =>
            {
                var allAuthorityUserInfo = m_dbAdapter.Authority.GetAllAuthorityUserInfo();
                if (CommUtils.IsLocalDeployed())
                {
                    allAuthorityUserInfo.ForEach(x => x.RealName = x.UserName);
                }
                return ActionUtils.Success(allAuthorityUserInfo);
            });
        }

        private bool IsUserExist(string username)
        {
            return UserService.GetUserByName(username) != null;
        }

        [HttpPost]
        public ActionResult GetProjectAuthorityByUsername(string username)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(username, "当前用户不能为空");
                CommUtils.Assert(IsUserExist(username), "当前用户[" + username + "]不存在");

                var allAuthorityTable = m_dbAdapter.Authority.GetAuthorityByUsername(username);
                var projectIds = allAuthorityTable.ConvertAll(x => x.ModifyProjectId);
                var projects = m_dbAdapter.Project.GetProjects(projectIds);
                    
                var dictProject = projects.ToDictionary(x => x.ProjectId);

                Func<int, bool> isExistAuthority = x => x == 1;

                var result = allAuthorityTable.Where(x => dictProject.ContainsKey(x.ModifyProjectId))
                    .ToList().ConvertAll(x => {
                        var project = dictProject[x.ModifyProjectId];
                        return new
                        {
                            ProjectName = project.Name,
                            CreateUserName = Toolkit.ToString(project.CreateUserName),
                            EnterpriseName = m_dbAdapter.Authority.GetAuthorizedEnterpriseName(project.ProjectId),
                            CreateTime = project.CreateTime,
                            ModifyModelAuthority = isExistAuthority(x.ModifyModelAuthority),
                            ModifyTaskAuthority = isExistAuthority(x.ModifyTaskAuthority),
                            ProjectGuid = project.ProjectGuid
                        };
                    });
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetProjectNamesByEnterpriseName(string enterpriseName, string username)
        {
            return ActionUtils.Json(() =>
            {
                if (CommUtils.IsLocalDeployed())
                {
                    var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                    var projects = m_dbAdapter.Project.GetProjects(projectIds);
                    var result = projects.Select(x => new
                    {
                        Guid = x.ProjectGuid,
                        Name = x.Name
                    });

                    return ActionUtils.Success(result);
                }
                else
                {
                    var allAuthorityTable = m_dbAdapter.Authority.GetAuthorityByUsername(username);
                    var dictUserProjectIds = allAuthorityTable.ToDictionary(x => x.ModifyProjectId);

                    var enterprise = m_dbAdapter.Authority.GetEnterpriseByName(enterpriseName);
                    CommUtils.Assert(enterprise != null, "找不到对应的机构名称[" + enterpriseName + "]");
                    var enterpriseProjectIds = m_dbAdapter.Authority.GetProjectIdsByEnterpriseId(enterprise.enterprise_id);

                    var projects = m_dbAdapter.Project.GetProjects(enterpriseProjectIds);
                    projects = projects.Where(x => !dictUserProjectIds.ContainsKey(x.ProjectId)).ToList();
                    var result = projects.ConvertAll(x => new
                    {
                        Guid = x.ProjectGuid,
                        Name = x.Name
                    });
                    return ActionUtils.Success(result);
                }
            });
        }

        [HttpPost]
        public ActionResult GetAllEnterpriseName()
        {
            return ActionUtils.Json(() =>
            {
                if (CommUtils.IsLocalDeployed())
                {
                    List<EnterpriseInfo> allEnterpriseName = new List<EnterpriseInfo> {
                        new EnterpriseInfo {
                            EnterpriseId = 9527,
                            EnterpriseName = CommUtils.GetEnterpriseName()
                        }
                    };
                    return ActionUtils.Success(allEnterpriseName);
                }
                else
                {
                    var allEnterpriseName = m_dbAdapter.Authority.GetAllEnterpriseName();
                    var enterpriseIds = m_dbAdapter.Authority.GetEnterpriseIdInAuthorizedProjectTable();
                    allEnterpriseName = allEnterpriseName.Where(x => enterpriseIds.Contains(x.EnterpriseId)).ToList();

                    return ActionUtils.Success(allEnterpriseName);
                }
            });
        }

        [HttpPost]
        public ActionResult AddUserProductAuthority(string username, string enterpriseName, string projectGuid,
            bool modifyModel, bool modifyTask)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(modifyModel || modifyTask, "权限类型不能为空");
                CommUtils.Assert(IsUserExist(username), "用户[{0}]不存在", username);
                CommUtils.Assert(m_dbAdapter.Authority.HasEnterpriseByName(enterpriseName), "找不到对应的机构名称[" + enterpriseName + "]");
                CommUtils.Assert(projectGuid != "empty","产品名称不能为[无]");
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(!m_dbAdapter.Authority.HasProductAuthority(username, project.ProjectId), "用户[" + username + "]已拥有产品[" + project.Name + "]的操作权限，请在该用户的产品列表中直接修改");
                
                var productAuthority = new EditProductAuthority();
                productAuthority.Username = username;
                productAuthority.ModifyProjectId = project.ProjectId;
                productAuthority.CreateProductAuthority = 0;
                productAuthority.ModifyModelAuthority = Convert.ToInt32(modifyModel);
                productAuthority.ModifyTaskAuthority = Convert.ToInt32(modifyTask);

                m_dbAdapter.Authority.NewProductAuthority(productAuthority);
                var logDescription = modifyModel ? " [编辑模型权限]" : "";
                logDescription += modifyTask ? " [编辑工作权限]" : "";
                NewEditProjectPermissionLog(project.ProjectId, "添加产品权限：添加用户[{0}]的{1}", username, logDescription);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DeleteUserProductAuthority(string username, string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(IsUserExist(username), "用户[{0}]不存在", username);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var productAuthority = m_dbAdapter.Authority.GetProductAuthority(username, project.ProjectId);
                CommUtils.Assert(productAuthority != null, "用户[{0}]没有产品[{1}]的操作权限，请刷新页面后重试", username, project.Name);

                m_dbAdapter.Authority.DeleteProductAuthority(productAuthority);
                NewEditProjectPermissionLog(project.ProjectId, "删除产品权限：删除用户[{0}]对产品[{1}]的操作权限[编辑模型权限：{2}，编辑工作权限：{3}]",
                    username, project.Name, productAuthority.ModifyModelAuthority, productAuthority.ModifyTaskAuthority);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult ModifyUserProductAuthority(string username, string enterpriseName, string projectGuid,
            bool modifyModel, bool modifyTask)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(IsUserExist(username), "用户[{0}]不存在，请刷新页面后重试", username);
                CommUtils.Assert(enterpriseName=="无" || m_dbAdapter.Authority.HasEnterpriseByName(enterpriseName), "找不到对应的机构名称[" + enterpriseName + "]");

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var productAuthority = m_dbAdapter.Authority.GetProductAuthority(username,project.ProjectId);
                CommUtils.Assert(productAuthority != null, "用户[" + username + "]没有产品[" + project.Name + "]的操作权限，无法进行修改，请刷新页面后重试");

                if (!modifyModel && !modifyTask)
                {
                    m_dbAdapter.Authority.DeleteProductAuthority(productAuthority);
                    NewEditProjectPermissionLog(project.ProjectId, "修改产品权限：删除用户[{0}]对产品[{1}]的操作权限[编辑模型权限：{2}，编辑工作权限：{3}]（修改用户权限时，如果编辑模型与编辑工作的权限都不选中，系统会自动默认为删除当前产品权限）",
                        username, project.Name, productAuthority.ModifyModelAuthority, productAuthority.ModifyTaskAuthority);
                }
                else
                {
                    var logDescription = productAuthority.ModifyModelAuthority != Convert.ToInt32(modifyModel) ?
                        "编辑模型权限：" + productAuthority.ModifyModelAuthority + "→" + Convert.ToInt32(modifyModel) + "，" : "编辑模型权限不变，";

                    logDescription += productAuthority.ModifyTaskAuthority != Convert.ToInt32(modifyTask) ?
                        "编辑工作权限：" + productAuthority.ModifyTaskAuthority + "→" + Convert.ToInt32(modifyTask) : "编辑工作权限不变";

                    productAuthority.ModifyModelAuthority = Convert.ToInt32(modifyModel);
                    productAuthority.ModifyTaskAuthority = Convert.ToInt32(modifyTask);

                    m_dbAdapter.Authority.UpdateProductAuthority(productAuthority);

                    NewEditProjectPermissionLog(project.ProjectId, "修改产品权限：修改用户[{0}]对产品[{1}]的操作权限[{2}]", username, project.Name, logDescription);
                }

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult CheckCreateProductAuthorityByName(string username)
        {
            return ActionUtils.Json(() =>
            {
                var allAuthority = m_dbAdapter.Authority.GetProductAuthorityByUsername(username);

                var isExist = allAuthority.Where(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                    .ToList().Any(x => x.CreateProductAuthority == 1);

                return ActionUtils.Success(isExist);
            });
        }

        [HttpPost]
        public ActionResult AddCreateProductAuthority(string username)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(IsUserExist(username), "用户[{0}]不存在", username);

                var allAuthority = m_dbAdapter.Authority.GetProductAuthorityByUsername(username);
                var isExist = allAuthority.Where(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
                    .ToList().Any(x => x.CreateProductAuthority == 1);

                CommUtils.Assert(!isExist, "用户[{0}]已存在创建产品的权限，无需添加", username);

                var productAuthority = new EditProductAuthority();
                productAuthority.Username = username;
                productAuthority.CreateProductAuthority = 1;
                productAuthority.ModifyModelAuthority = 0;
                productAuthority.ModifyTaskAuthority = 0;
                productAuthority.ModifyProjectId = -1;

                m_dbAdapter.Authority.NewProductAuthority(productAuthority);
                NewEditProjectPermissionLog(null, "添加创建产品权限：添加用户[{0}]的创建产品权限", username);

                return ActionUtils.Success(isExist);
            });
        }

        [HttpPost]
        public ActionResult DeleteCreateProductAuthority(string username)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(IsUserExist(username), "用户[{0}]不存在，请刷新页面后重试", username);
                var allAuthority = m_dbAdapter.Authority.GetProductAuthorityByUsername(username);
                CommUtils.Assert(allAuthority.Count() != 0, "用户[{0}]没有创建产品的权限，请刷新页面后重试", username);
                allAuthority.ForEach(x => {
                    if (x.ModifyProjectId == -1)
                    {
                        m_dbAdapter.Authority.DeleteProductAuthority(x);
                        NewEditProjectPermissionLog(null,"删除创建产品权限：删除用户[{0}]的创建产品权限", username);
                    }
                    else
                    {
                        x.CreateProductAuthority = 0;
                        m_dbAdapter.Authority.UpdateProductAuthority(x);
                        NewEditProjectPermissionLog(x.ModifyProjectId, 
                            "删除创建产品权限：删除用户[{0}]的创建产品权限(错误数据处理：把[create_product_authority] = 1 && [modify_project_id] != -1 这种情况的数据更正)", username);
                    }
                });

                return ActionUtils.Success("");
            });
        }

        private void NewEditProjectPermissionLog(int? projectId, string description, params object[] args)
        {
            description = CommUtils.FormatString(description, args);
            m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, projectId, description, "只有超级管理员才能够进入的权限管理页面");
        }
    }
}
