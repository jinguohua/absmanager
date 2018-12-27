using System.Web;

namespace ChineseAbs.Web.Menu.Ajax
{
    /// <summary>
    /// Summary description for MenuHandler
    /// </summary>
    public class MenuHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
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