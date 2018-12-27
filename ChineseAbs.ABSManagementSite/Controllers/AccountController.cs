using ABS.Core.Models;
using ABS.Core.Services;
using ChineseAbs.ABSManagement.Loggers;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Filters;
using ChineseAbs.ABSManagementSite.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class AccountController : BaseController
    {

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public TestServices TestServices { get; set; }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            //var users = DependencyResolver.Current.GetService<AccountController>();
            AppUser user = UserService.GetUserByNameOrEmail(model.UserName);
            string message = CheckUserStatus(user);

            if (String.IsNullOrEmpty(message))
            {
                var sign = HttpContext.GetOwinContext().Get<ApplicationSignInManager<AppUser>>();
                var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);
                switch (result)
                {
                    case SignInStatus.Success:
                        await SignInManager.SignInAsync(user, false, false);
                        UserService.UpdateUserActivity(user.Id, Request.UserHostAddress);
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "用户名或密码错误");
                        return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", message);
            }



            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return Redirect(Url.Content("~"));
        }


        private string CheckUserStatus(AppUser user)
        {
            if (user == null)
                return "用户名不存在";
            else if (!user.IsActive)
                return "用户已经被禁用";
            return "";
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return Redirect(Url.Content("~"));
        }
    }
}
