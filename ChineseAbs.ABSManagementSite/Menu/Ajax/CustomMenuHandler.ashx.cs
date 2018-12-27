using System;
using System.Web;
using ChineseAbs.Logic;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.Web.Menu.Ajax
{
    /// <summary>
    /// Summary description for CustomMenuHandler
    /// </summary>
    public class CustomMenuHandler : IHttpHandler
    {
        UserService service = new UserService();
        public void ProcessRequest(HttpContext context)
        {
            if (CommUtils.IsLocalDeployed())
            {
                return;
            }

            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            object obj = new object();
            switch (action)
            {
                case "menuSave":
                    string customMenu = context.Request["names"];
                    obj = 1;
                    try
                    {
                        service.UpdateUserCustomMenu(context.User.Identity.Name, customMenu);
                    }
                    catch (Exception/* ex*/)
                    {
                        obj = 0;
                    }
                    break;
                case "menuGet":
                    obj = service.GetUserCustomMenu(context.User.Identity.Name);
                    break;
            }
            if (obj == null)
                context.Response.Write("");
            else
                context.Response.Write(obj.ToString());

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}