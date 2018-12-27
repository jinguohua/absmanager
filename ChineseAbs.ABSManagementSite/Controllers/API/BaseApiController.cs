using ABS.Core.Models;
using ABS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ChineseAbs.ABSManagementSite;
using System.Web;

namespace ABS.ABSManagementSite.Controllers.API
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        public SAFS.Core.Dependency.IIocResolver IocResolver { get; set; }

        public UserService UserService { get; set; }
        public CompanyService CompanyService { get; set; }

        private AppUser currentUser = null;
        public AppUser CurrentUser
        {
            get
            {
                if (currentUser == null && User.Identity != null)
                {
                    string userID = CurrentUserId;


                    if (!String.IsNullOrEmpty(userID))
                        currentUser = UserService.GetUserById(userID);
                }
                return currentUser;
            }
        }

        public string CurrentUserId
        {
            get
            {
                return User.Identity.GetUserId();
            }

        }

        public int CompanyID
        {
            get
            {
                if (CurrentUser == null)
                    return CompanyService.DefaultCompany.Id;
                else
                    return CurrentUser.CompanyId ?? CompanyService.DefaultCompany.Id;
            }
        }

        private ApplicationSignInManager<AppUser> _signInManager;
        public ApplicationSignInManager<AppUser> SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager<AppUser>>();
            }
            private set
            {
                _signInManager = value;
            }
        }
    }
}
