using ABS.Core.Models;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Framework;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite;
using ChineseAbs.ABSManagementSite.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ABS.Core.Services;
using System.Web.Mvc.Filters;
using SAFS.Core.Permissions.Service;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [SAFS.Web.MVC.Logging.OperateLogFilterAttribute]
    [Authorize]
    public class BaseController : Controller
    {
        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
            ViewBag.CurrentUser = CurrentUser;
        }

        protected void CheckPermission(PermissionObjectType objectType, string objectUniqueId, 
            PermissionType permissionType)
        {
            CheckPermission(CurrentUserName, objectType, objectUniqueId, permissionType);
        }

        protected void CheckPermission(string checkUserName, PermissionObjectType objectType, 
            string objectUniqueId, PermissionType permissionType)
        {
            CommUtils.AssertHasPermission(CurrentUserName, checkUserName, objectType, objectUniqueId, permissionType);
        }

        protected DBAdapter m_dbAdapter = null;

        public DBAdapter DbAdapter { get { return m_dbAdapter; } set { m_dbAdapter = value; } }

        private string m_currentUserName;

        protected string CurrentUserName
        {
            get
            {
                return CurrentUser.UserName;
            }
        }

        protected bool IsCurrentUser(string userName)
        {
            return User.Identity.Name.Equals(userName, StringComparison.CurrentCultureIgnoreCase);
        }

        public Platform Platform
        {
            get
            {
                return m_platform;
            }
            set
            {
                m_platform = value;
            }
        }

        private Platform m_platform;

        protected internal Tuple<string, MemoryStream> CnabsFile(string fileName,
            MemoryStream memoryStream, DownloadFileAuthorityType authority)
        {
            if (authority == DownloadFileAuthorityType.AllForbidden)
            {
                //TODO:
                throw new NotImplementedException("暂不支持DownloadFileAuthorityType.AllForbidden");
            }

            if (fileName.EndsWith("doc", StringComparison.CurrentCultureIgnoreCase)
                || fileName.EndsWith("docx", StringComparison.CurrentCultureIgnoreCase))
            {
                if (authority == DownloadFileAuthorityType.Word2PdfWithWatermark)
                {
                    var waterMark = new WaterMarkMultiText()
                    {
                        BigText = CommUtils.GetWatermarkTitle(),
                        SmallText = "[" + CurrentUserName + "]下载于" + DateTime.Now.ToString()
                    };

                    var pdfUtil = new PdfUtils(waterMark);
                    pdfUtil.PdfPermission = PdfUtils.PdfPermissionEnum.Printing;
                    var ms = pdfUtil.WordToPdfMemoryStream(memoryStream);
                    ms.Seek(0, SeekOrigin.Begin);

                    var pdfFileName = FileUtils.ConvertFileExtension(fileName, FileType.PDF);
                    return Tuple.Create(pdfFileName, ms);
                }
                else if (authority == DownloadFileAuthorityType.Word2Pdf)
                {
                    var pdfUtil = new PdfUtils();
                    var ms = pdfUtil.WordToPdfMemoryStream(memoryStream);
                    ms.Seek(0, SeekOrigin.Begin);

                    var pdfFileName = FileUtils.ConvertFileExtension(fileName, FileType.PDF);
                    return Tuple.Create(pdfFileName, ms);
                }
            }

            return Tuple.Create(fileName, memoryStream);
        }

        protected internal ActionResult CnabsFile(string filePathName, string contentType, 
            string fileDownloadName, DownloadFileAuthorityType authority)
        {
            if (authority == DownloadFileAuthorityType.AllForbidden)
            {
                //TODO:
                throw new NotImplementedException("暂不支持DownloadFileAuthorityType.AllForbidden");
            }

            if (filePathName.EndsWith("doc", StringComparison.CurrentCultureIgnoreCase)
                || filePathName.EndsWith("docx", StringComparison.CurrentCultureIgnoreCase))
            {
                if (authority == DownloadFileAuthorityType.Word2PdfWithWatermark)
                {
                    var waterMark = new WaterMarkMultiText()
                    {
                        BigText = CommUtils.GetWatermarkTitle(),
                        SmallText = "[" + CurrentUserName + "]下载于" + DateTime.Now.ToString()
                    };

                    var pdfUtil = new PdfUtils(waterMark);
                    pdfUtil.PdfPermission = PdfUtils.PdfPermissionEnum.Printing;
                    var ms = pdfUtil.WordToPdfMemoryStream(filePathName);
                    ms.Seek(0, SeekOrigin.Begin);

                    var pdfFileName = FileUtils.ConvertFileExtension(fileDownloadName, FileType.PDF);
                    return File(ms, contentType, pdfFileName);
                }
                else if (authority == DownloadFileAuthorityType.Word2Pdf)
                {
                    var pdfUtil = new PdfUtils();
                    var ms = pdfUtil.WordToPdfMemoryStream(filePathName);
                    ms.Seek(0, SeekOrigin.Begin);

                    var pdfFileName = FileUtils.ConvertFileExtension(fileDownloadName, FileType.PDF);
                    return File(ms, contentType, pdfFileName);
                }
            }

            return File(filePathName, contentType, fileDownloadName);
        }


        private ApplicationSignInManager<AppUser> _signInManager;
        public ApplicationSignInManager<AppUser> SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager<AppUser>>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private ApplicationUserManager<AppUser> _userManager;
        public ApplicationUserManager<AppUser> UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager<AppUser>>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public string CurrentUserId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                    return User.Identity.GetUserId();
                else
                    return "";
            }

        }

        public UserService UserService { get; set; }

        public MenuServices MenuService { get; set; }

        public CompanyService CompanyService { get; set; }

        public BaseCodeService BaseCodeService { get; set; }

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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}