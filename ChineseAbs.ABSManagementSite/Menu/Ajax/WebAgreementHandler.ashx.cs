using System;
using System.Web;
using ChineseAbs.Logic;


namespace ChineseAbs.Web.Menu.Ajax
{
    /// <summary>
    /// Summary description for WebAgreementHandler
    /// </summary>
    public class WebAgreementHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string userName = context.User.Identity.Name;
            context.Response.ContentType = "text/plain";
            if (!string.IsNullOrEmpty(userName))
            {
                var service = new WebAgreementService();
                double latestVersion = Convert.ToDouble(service.GetLatestWebAgreement().Rows[0]["version_id"]);                  
                service.AddWebAgreementSigned(userName, latestVersion);
            }
            context.Response.Write("true");
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




