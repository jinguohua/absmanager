using System;
using System.Linq;
using System.Web.Mvc;
using ChineseAbs.ABSManagement.Utils;
using ABS.Core.Services;
using ABS.Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using SAFS.Utility.Web;
using ABS.ABSManagementSite;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ManageController : BaseController
    {
        ///// <summary>
        ///// 初始化系统
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public string InitSystem(string key)
        //{
        //    var superKey = WebConfigUtils.SuperKey;
        //    CommUtils.AssertHasContent(superKey, "未配置超级秘钥，不能进行系统初始化");
        //    CommUtils.AssertEquals(string.Compare(key, superKey, StringComparison.Ordinal), 0, "超级秘钥错误");

        //    if (!Roles.RoleExists(RoleAdmin))
        //    {
        //        Roles.CreateRole(RoleAdmin);
        //    }

        //    var user = UserService.GetUserByName(SuperUserName);
        //    if (user != null)
        //    {
        //        UserService.DeleteAsync(new AppUser()
        //        {
        //            UserName = SuperUserName,
        //            PasswordHash = new PasswordHasher().HashPassword(superKey)
        //        });
        //    }

        //    UserService.CreateAsync(new AppUser()
        //    {
        //        UserName = SuperUserName,
        //        PasswordHash = new PasswordHasher().HashPassword(superKey)
        //    });
        //    Roles.AddUserToRole(SuperUserName, RoleAdmin);
        //    return "Success";
        //}

        public ActionResult Index()
        {
            return View();
        }

        [ActionName("Profile")]
        public ActionResult ProfilePage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAllUsers()
        {
            var result = new SAFS.Utility.Web.JsonResultDataEntity<dynamic>();
            var users = UserService.Users;
            result.Data = users.Where(o => !o.IsDeleted).ToList().Select(x => new
            {
                id = x.Id,
                userName = x.UserName,
                isLocked = x.LockoutEnabled,
                realName = string.IsNullOrEmpty(x.NickName) ? x.UserName : x.NickName,
                email = x.Email,
                phoneNumber = x.PhoneNumber,
                roles = x.Roles.Select(o => o.Name).ToList(),
                roleNames= string.Join(",",x.Roles.Select(o=>o.Description).ToList()),
            });
         
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> SetUserRole(string userName, bool isAdmin)
        {
            var user = UserService.GetUserByName(userName);
            CommUtils.Assert(user != null, "用户信息错误");
            if (isAdmin)
            {
                await UserService.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await UserService.RemoveFromRoleAsync(user, "Admin");
            }
            return Json(new JsonResultDataEntity<string>() { Code = 0 });
        }



        [HttpPost]
        public async Task<JsonResult> CreateUser(UserViewModel vuser)
        {
            CommUtils.AssertEquals(vuser.RepeatPassword, vuser.Password, "两次输入密码不一致");
            CommUtils.Assert(vuser.Password.Length >= 6, "密码最小长度为六位");
            CommUtils.Assert(UserService.GetUserByName(vuser.UserName) == null, "用户[{0}]已存在", vuser.UserName);
            AppUser user = new AppUser()
            {
                UserName = vuser.UserName,
                PasswordHash = new PasswordHasher().HashPassword(vuser.Password),
                Email = vuser.Email,
                NickName = vuser.NickName,
                PhoneNumber = vuser.PhoneNumber,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            await UserService.CreateAsync(user);
            user = UserService.GetUserByName(vuser.UserName);
            foreach (var item in vuser.Roles)
            {
                await UserService.AddToRoleAsync(user, item);
            }

            return Json(new JsonResultDataEntity<string>() { Code = 0 });
        }

        [HttpPost]
        public async Task<JsonResult> EditUser(UserViewModel vuser)
        {
            var user = UserService.GetUserById(vuser.ID);
            user.UserName = vuser.UserName;
            user.NickName = vuser.NickName;
            user.Email = vuser.Email;
            user.PhoneNumber = vuser.PhoneNumber;
            await UserService.UpdateAsync(user);
            var roles = user.Roles.ToList();
            for (int i = 0; i < roles.Count; i++)
            {
                await UserService.RemoveFromRoleAsync(user, roles[i].Name);
            }
            foreach (var item in vuser.Roles)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    await UserService.AddToRoleAsync(user, item);
                }

            }
            return Json(new JsonResultDataEntity<string>() { Code = 0 });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteUser(string userName)
        {
            AppUser user = UserService.GetUserByName(userName);
            CommUtils.Assert(user != null, "用户信息错误");
            CommUtils.Assert(!user.Roles.Any(o => o.Name.Equals("Admin")), "不能删除管理员用户");
            user.IsDeleted = true;
            await UserService.UpdateAsync(user);

            return Json(new JsonResultDataEntity<string>() { Code = 0 });
        }

        [HttpPost]
        public ActionResult UnlockUser(string userName)
        {
            var user = UserService.GetUserByName(userName);
            CommUtils.Assert(user != null, "用户[{0}]不存在", userName);
            CommUtils.Assert(user.LockoutEnabled, "用户[{0}]没有被锁定", userName);
            UserService.SetLockoutEnabledAsync(user, false);
            return Json(new JsonResultDataEntity<string>() { });
        }

        [HttpPost]
        public ActionResult ResetUserPassword(string userName, string password, string repeatPassword)
        {
            var user = UserService.GetUserByName(userName);
            CommUtils.AssertEquals(repeatPassword, password, "两次输入密码不一致");
            CommUtils.Assert(password.Length >= 6, "密码最小长度为六位");
            CommUtils.Assert(user != null, "用户[{0}]不存在", userName);
            UserService.SetPasswordHashAsync(user, new PasswordHasher().HashPassword(password));
            return Json(new JsonResultDataEntity<string>() { Code = 0 });
        }

        [HttpPost]
        public ActionResult ModifyPassword(string oldPassword, string password, string repeatPassword)
        {
            return ActionUtils.Json(() =>
            {
                var user = UserService.GetUserByName(CurrentUserName);

                CommUtils.Assert(oldPassword.Length >= 6, "原密码格式错误，密码最小长度为六位");
                CommUtils.AssertEquals(repeatPassword, password, "两次输入密码不一致");
                CommUtils.Assert(password.Length >= 6, "新密码最小长度为六位");
                CommUtils.Assert(user != null, "用户[{0}]不存在", CurrentUserName);
                var result = SignInManager.PasswordSignInAsync(user.UserName, password, true, false).Result;
                CommUtils.Assert(result != SignInStatus.Success || result != SignInStatus.LockedOut,
                    "输入用户名和密码不匹配，如遗忘密码，请联系管理员进行密码重置");
                UserService.SetPasswordHashAsync(CurrentUser, password);
                return ActionUtils.Success(1);
            });
        }
        [HttpPost]
        public ActionResult GetUserProfile()
        {
            return ActionUtils.Json(() =>
            {


                var reuslt = new
                {
                    guid = CurrentUser.Id,
                    userName = CurrentUser.UserName,
                    realName = CurrentUser.Name,
                    company = "",
                    department = "",
                    email = CurrentUser.Email,
                    cellphone = CurrentUser.PhoneNumber
                };

                return ActionUtils.Success(reuslt);
            });
        }

        [HttpPost]
        public ActionResult ModifyUserProfile(string realName, string company,
            string department, string email, string cellphone)
        {
            return ActionUtils.Json(() =>
            {
                //CommUtils.AssertHasContent(realName, "真实姓名不能为空");
                //CommUtils.Assert(realName.Length <= 50, "真实姓名不能超过50字符数");
                //var userName = User.Identity.Name;
                //var profile = m_dbAdapter.LocalDeployed.GetUserProfile(userName);

                //profile.RealName = realName;
                //profile.Company = company;
                //profile.Department = department;
                //profile.Email = email;
                //profile.Cellphone = cellphone;
                //m_dbAdapter.LocalDeployed.UpdateUserProfile(profile);

                return ActionUtils.Success(1);
            });
        }

        public JsonResult GetRoles()
        {
            var roleList = UserService.GetAllRoles();
            var data = roleList.ToList().Select(o =>
            new
            {
                id = o.Name,
                text = o.Description
            }).ToList();
            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = data }, JsonRequestBehavior.AllowGet);
        }

    }
}