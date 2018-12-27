using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SAFS.Core.Permissions.Identity;
using SAFS.Core.Permissions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ChineseAbs.ABSManagementSite
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager<TUser> : UserManager<TUser> where TUser : User, new()
    {
        public ApplicationUserManager(IUserStore<TUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager<TUser> Create(IdentityFactoryOptions<ApplicationUserManager<TUser>> options, IOwinContext context)
        {
            UserStore<TUser> store = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(UserStore<TUser>)) as UserStore<TUser>;
            var manager = new ApplicationUserManager<TUser>(store);
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<TUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                //RequireNonLetterOrDigit = true,
                RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(20);
            manager.MaxFailedAccessAttemptsBeforeLockout = 10;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<TUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<TUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Google Authenticator", new GoogleAuthenticatorTokenProvider<TUser> { });
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<TUser>(dataProtectionProvider.Create(("ASP.NET Identity")));
            }
            return manager;
        }

        public TUser GetApplicationUser(string username, string password)
        {
            return this.Find(username, password);
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager<TUser> : SignInManager<TUser, string> where TUser : User, new()
    {
        public ApplicationSignInManager(ApplicationUserManager<TUser> userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override async Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user)
        {
            var userIdentity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.NickName));
            return userIdentity;
        }

        public static ApplicationSignInManager<TUser> Create(IdentityFactoryOptions<ApplicationSignInManager<TUser>> options, IOwinContext context)
        {
            return new ApplicationSignInManager<TUser>(context.GetUserManager<ApplicationUserManager<TUser>>(), context.Authentication);
        }
    }
}