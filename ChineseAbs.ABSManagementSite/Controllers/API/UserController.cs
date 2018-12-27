using ABS.Core.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ChineseAbs.ABSManagementSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Threading.Tasks;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class UserController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<string> Login(LoginModel model)
        {
            string errorMessge = "";
            if (!ModelState.IsValid)
            {
                errorMessge = ("信息填写错误");
            }
            AppUser user = UserService.GetUserByNameOrEmail(model.UserName);
            if (user == null)
                errorMessge = "用户名不存在";
            else if (!user.IsActive)
                errorMessge = "用户已经被禁用";

            if (String.IsNullOrEmpty(errorMessge))
            {
                var context = Request.Properties["MS_HttpContext"] as HttpContextWrapper;
                var sign = HttpContextBaseExtensions.GetOwinContext(context.Request).Get<ApplicationSignInManager<AppUser>>();

                var result = await sign.PasswordSignInAsync(user.UserName, model.Password, true, false);
                switch (result)
                {
                    case SignInStatus.Success:
                        await sign.SignInAsync(user, false, false);
                        UserService.UpdateUserActivity(user.Id, context.Request.UserHostAddress);
                        errorMessge = "";
                        break;
                    case SignInStatus.Failure:
                    default:
                        errorMessge = "用户名或密码错误";
                        break;
                }
            }
            if (!String.IsNullOrEmpty(errorMessge))
                throw new Exception(errorMessge);
            else
                return "";
        }

        [HttpGet]
        public BaseUserInfo Current()
        {
            var cuser = this.CurrentUser;
            BaseUserInfo user = new BaseUserInfo();
            if (cuser != null)
            {
                user.Name = cuser.NickName ?? cuser.UserName;
                user.Phone = cuser.PhoneNumber;
                user.Roles = cuser.Roles.Select(o => o.Name).ToArray();
                user.UserID = cuser.Id;
                user.Group = String.Join(",", cuser.Organizations.Select(o => o.Name).ToArray());
            }
            return user;
        }

        [HttpPost]
        public void Logout()
        {
            var context = Request.Properties["MS_HttpContext"] as HttpContextWrapper;
            var sign = HttpContextBaseExtensions.GetOwinContext(context.Request).Get<ApplicationSignInManager<AppUser>>();

            sign.AuthenticationManager.SignOut();
        }

        [HttpGet]
        [ActionName("List")]
        public List<UserViewModel> List()
        {
            var users = UserService.GetAllUser();

            var data = users.Select(x => new UserViewModel
            {
                ID = x.Id,
                UserName = x.UserName,
                NickName = string.IsNullOrEmpty(x.NickName) ? x.UserName : x.NickName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Roles = x.Roles.Select(o => o.Name).ToArray(),
                RoleNames = string.Join(",", x.Roles.Select(o => o.Description).ToList()),
            }).ToList();

            return data;
        }

        [HttpPost]
        public async void Create(UserViewModel vuser)
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


        }

        [HttpPost]
        public async void Edit(UserViewModel vuser)
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

        }

        [HttpPost]
        public async void Delete(string userName)
        {
            AppUser user = UserService.GetUserByName(userName);
            CommUtils.Assert(user != null, "用户信息错误");
            CommUtils.Assert(!user.Roles.Any(o => o.Name.Equals("Admin")), "不能删除管理员用户");
            user.IsDeleted = true;
            await UserService.UpdateAsync(user);

        }

        [HttpPost]
        public void Unlock(string userName)
        {
            var user = UserService.GetUserByName(userName);
            CommUtils.Assert(user != null, "用户[{0}]不存在", userName);
            CommUtils.Assert(user.LockoutEnabled, "用户[{0}]没有被锁定", userName);
            UserService.SetLockoutEnabledAsync(user, false);

        }

        [HttpPost]
        public void ResetPassword(string userName, string password, string repeatPassword)
        {
            var user = UserService.GetUserByName(userName);
            CommUtils.AssertEquals(repeatPassword, password, "两次输入密码不一致");
            CommUtils.Assert(password.Length >= 6, "密码最小长度为六位");
            CommUtils.Assert(user != null, "用户[{0}]不存在", userName);
            UserService.SetPasswordHashAsync(user, new PasswordHasher().HashPassword(password));

        }

        [HttpPost]
        public void ModifyPassword(string oldPassword, string password, string repeatPassword)
        {
            CommUtils.Assert(oldPassword.Length >= 6, "原密码格式错误，密码最小长度为六位");
            CommUtils.AssertEquals(repeatPassword, password, "两次输入密码不一致");
            CommUtils.Assert(password.Length >= 6, "新密码最小长度为六位");
            var user = UserService.GetUserById(CurrentUserId);
            CommUtils.Assert(user != null, "用户[{0}]信息错误,无法修改密码", CurrentUser.UserName);
            var result =SignInManager.PasswordSignInAsync(user.UserName, oldPassword, true, false).Result;
            CommUtils.Assert(result != SignInStatus.Success || result != SignInStatus.LockedOut,
                "输入用户名和密码不匹配，如遗忘密码，请联系管理员进行密码重置");
            UserService.SetPasswordHashAsync(CurrentUser, password);
        }

       
    }
}
