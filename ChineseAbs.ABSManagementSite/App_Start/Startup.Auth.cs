using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using Microsoft.AspNet.Identity.Owin;
using ABS.Core.Models;

namespace ChineseAbs.ABSManagementSite
{
    public partial class Startup
    {
        private void ApplyRedirect(CookieApplyRedirectContext context)
        {
            var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var mvcContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(mvcContext);
            var actionUri = helper.Action("Login", "Account", new { lang = routeData.Values["lang"] });
            //var returnUrl = HttpUtility.ParseQueryString(new System.Uri(context.RedirectUri).Query)[context.Options.ReturnUrlParameter];
            //routeValues.Add(context.Options.ReturnUrlParameter, returnUrl);
            context.Response.Redirect(actionUri);
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext<ApplicationUserManager<AppUser>>(ApplicationUserManager<AppUser>.Create);
            app.CreatePerOwinContext<ApplicationSignInManager<AppUser>>(ApplicationSignInManager<AppUser>.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                //Provider = new CookieAuthenticationProvider { OnApplyRedirect = ApplyRedirect }
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                    //    validateInterval: TimeSpan.FromMinutes(30),
                    //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
           
        }
    }
}