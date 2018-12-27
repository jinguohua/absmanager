using ChineseAbs.ABSManagement.Framework;
using ChineseAbs.ABSManagement.Utils;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Filters
{
    ///// <summary>
    ///// 将当前已登录的用户名附件到线程中
    ///// </summary>
    //public class UserStatusAttachThreadAttribute : ActionFilterAttribute
    //{
    //    public UserStatusAttachThreadAttribute()
    //    {
    //    }

    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        Platform.UserName = "";
    //        var identity = filterContext.HttpContext.User.Identity;
    //        if (identity.IsAuthenticated)
    //        {
    //            var loader = new UserProfileLoader(identity.Name);
    //            var profile = loader.Get(identity.Name);
    //            Platform.UserName = profile == null ? identity.Name : profile.UserName;
    //        }
    //    }
    //}
}
