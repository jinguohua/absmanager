using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class UserGroupController : BaseController
    {
    	public ActionResult Index()
        {
            return View();
        }
	
        //获取所有UserGroup
        [HttpPost]
        public ActionResult GetUserGroups()
        {
            return ActionUtils.Json(() =>
            {
                var userGroups = m_dbAdapter.UserGroup.GetByOwner(CurrentUserName);

                var result = userGroups.ConvertAll(x => new
                    {
                        guid = x.Guid,
                        name = x.Name
                    });

                return ActionUtils.Success(result);
            });
        }



        //增加UserGroup
        [HttpPost]
        public ActionResult AddUserGroup(string name)
        {
            return ActionUtils.Json(() =>
            {
                ValidateUtils.Name(name, "组名");
                var userGroups = m_dbAdapter.UserGroup.GetByOwner(CurrentUserName);
                var userGroupNames = userGroups.Select(x => x.Name).ToList();
                CommUtils.Assert(!userGroupNames.Contains(name), "[{0}]组名已存在，请重新输入", name);
                var userGroup = new UserGroup 
                { 
                    Name = name,
                    Owner = CurrentUserName
                    
                };
              var result=  m_dbAdapter.UserGroup.New(userGroup);

              return ActionUtils.Success(result);
            });
        }

        //删除UserGroup
        [HttpPost]
        public ActionResult RemoveUserGroup(string userGroupGuid)
        {
            return ActionUtils.Json(() =>
            {
                var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
                CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

                var userGroupMaps = m_dbAdapter.UserGroupMap.GetByUserGroupGuid(userGroupGuid);

                userGroupMaps.ForEach(x => m_dbAdapter.UserGroupMap.Delete(x));
                m_dbAdapter.UserGroup.Delete(userGroup);

                return ActionUtils.Success(1);
            });
        }

        //修改UserGroup
        [HttpPost]
        public ActionResult ModifyUserGroup(string userGroupGuid, string name)
        {
            return ActionUtils.Json(() =>
            {
                var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
                CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

                ValidateUtils.Name(name, "组名");

                var userGroups = m_dbAdapter.UserGroup.GetByOwner(CurrentUserName);
                var userGroupNames = userGroups.Select(x => x.Name).ToList();
                CommUtils.Assert(!userGroupNames.Contains(name), "[{0}]组名已存在，请重新输入", name);

                userGroup.Name = name;
                m_dbAdapter.UserGroup.Update(userGroup);

                return ActionUtils.Success(1);
            });
        }
    }
}