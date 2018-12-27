using ABS.ABSManagementSite.Models;
using ABS.Core.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite;
using ChineseAbs.ABSManagementSite.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class AccountController : BaseApiController
    {
        public class AuthModel
        {
            public class TokenItem
            {
                public string Token { get; set; }

                public long ExpireIn { get; set; }
            }

            public class UserInfo
            {
                public string Name { get; set; }

                public string Id { get; set; }

                public string Avatar { get; set; }
            }

            public enum EMenuType
            {
                Site,
                Navigation,
                Person
            }

            public class MenuItem
            {
                public string Id { get; set; }

                public string Icon { get; set; }

                public string Key { get; set; }

                public string Name { get; set; }

                public string ParentID { get; set; }

                public string Url { get; set; }

                public EMenuType Type { get; set; }

                public MenuItem[] Children { get; set; }
            }

            public UserInfo User { get; set; }

            public MenuItem[] Menus { get; set; }

            public TokenItem Token { get; set; }

        }

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
        public AuthModel Auth()
        {
            AuthModel model = new AuthModel();
            model.User = new AuthModel.UserInfo { Id = CurrentUser.Id, Name = CurrentUser.NickName };

            model.Menus = new AuthModel.MenuItem[] {
               // new AuthModel.MenuItem(){ Name = "首页", Id = 1, Key="H"  }
            };

            model.Token = new AuthModel.TokenItem { Token = Guid.NewGuid().ToString(), ExpireIn = DateTime.Now.AddDays(1).Ticks };

            return model;
        }

        [HttpGet]
        public BaseUserInfo Info()
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
        public string Logout()
        {
            var context = Request.Properties["MS_HttpContext"] as HttpContextWrapper;
            var sign = HttpContextBaseExtensions.GetOwinContext(context.Request).Get<ApplicationSignInManager<AppUser>>();

            sign.AuthenticationManager.SignOut();

            return "ok";
        }


        [HttpPost]
        [Route("api/account/password/modify")]
        public void ModifyPassword(ModifyPwdViewModel viewModel)
        {
            var oldPassword = viewModel.OldPassword;
            var password = viewModel.NewPassword;
            var repeatPassword = viewModel.RepeatPassword;
            CommUtils.Assert(oldPassword.Length >= 6, "原密码格式错误，密码最小长度为六位");
            CommUtils.AssertEquals(repeatPassword, password, "两次输入密码不一致");
            CommUtils.Assert(password.Length >= 6, "新密码最小长度为六位");
            var user = UserService.GetUserById(CurrentUserId);
            CommUtils.Assert(user != null, "用户[{0}]信息错误,无法修改密码", CurrentUser.UserName);
            var result = SignInManager.PasswordSignInAsync(user.UserName, oldPassword, true, false).Result;
            CommUtils.Assert(result != SignInStatus.Success || result != SignInStatus.LockedOut,
                "输入用户名和密码不匹配，如遗忘密码，请联系管理员进行密码重置");
            UserService.SetPasswordHashAsync(CurrentUser, password);
        }
    }

}
