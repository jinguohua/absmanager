using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class UserGroupMapController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        //根据userGroupGuid获取组成员Users
        [HttpPost]
        public ActionResult GetUserGroupUsers(string userGroupGuid)
        {
            var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
            CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

            var userGroupMaps = m_dbAdapter.UserGroupMap.GetByUserGroupGuid(userGroupGuid);


            var userNames = userGroupMaps.Select(x => x.UserName).ToArray();
            var users = UserService.GetUsers(userNames);

            var result = users.Select(x => new
            {
                userName = x.UserName,
                realName = x.Name,
            });
            var resultData = new SAFS.Utility.Web.JsonResultDataEntity<dynamic>() { Data = result };
            return Json(resultData);
        }

        //获取除组员以外的其他用户
        [HttpPost]
        public ActionResult GetUsers(string userGroupGuid)
        {
            var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
            CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

            var userGroupMaps = m_dbAdapter.UserGroupMap.GetByUserGroupGuid(userGroupGuid);
            var userGroupMapUserNames = userGroupMaps.Select(x => x.UserName).ToList();
            userGroupMapUserNames.Add(CurrentUserName);

            var accounts = m_dbAdapter.Authority.GetAllAuthedAccount();
            var accountUserNames = accounts.Select(x => x.UserName).ToList();

            var userNames = accountUserNames.Except(userGroupMapUserNames, StringComparer.OrdinalIgnoreCase).ToArray();
            var users = UserService.GetUsers(userNames);

            var result = users.Select(x => new
            {
                userName = x.UserName,
                realName = x.Name,
            });
            var resultData = new SAFS.Utility.Web.JsonResultDataEntity<dynamic>() { Data = result };
            return Json(resultData);
        }

        //增加组员
        [HttpPost]
        public ActionResult AddUsers(string userGroupGuid, string userNames)
        {
            return ActionUtils.Json(() =>
            {
                var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
                CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

                var userNameList = CommUtils.Split(userNames);
                foreach (var userName in userNameList)
                {
                    var userGroupMap = new UserGroupMap
                    {
                        UserGroupGuid = userGroupGuid,
                        UserName = userName
                    };
                    m_dbAdapter.UserGroupMap.New(userGroupMap);
                }

                return ActionUtils.Success(1);
            });
        }

        //删除组员
        [HttpPost]
        public ActionResult RemoveUsers(string userGroupGuid,  string userNames)
        {
            return ActionUtils.Json(() =>
            {
                var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
                CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

                var userGroupMaps = m_dbAdapter.UserGroupMap.GetByUserGroupGuid(userGroupGuid);
                var userGroupMapDic = userGroupMaps.ToDictionary(x => x.UserName);

                var userNameList = CommUtils.Split(userNames);
                foreach (var userName in userNameList)
                {
                    m_dbAdapter.UserGroupMap.Delete(userGroupMapDic[userName]);
                }

                return ActionUtils.Success(1);
            });
        }
        
    }
}