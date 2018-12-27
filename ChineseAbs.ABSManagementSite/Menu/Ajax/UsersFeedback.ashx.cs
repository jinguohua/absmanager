using ChineseAbs.Logic;
using ChineseAbs.Web.Menu.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.SessionState;

namespace ChineseAbs.Web.Menu.Ajax
{
    /// <summary>
    /// Summary description for UsersFeedback
    /// </summary>
    public class UsersFeedback : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = context.Request["type"];
            string categary = context.Request["categary"];
            string feedback = HttpUtility.HtmlEncode(context.Request["text"]);
            string loginName = context.User.Identity.Name;
            string url = context.Request["url"];
            string Name = context.Request["name"];
            string email = context.Request["email"];

            object returnObj = new object();
            switch (type)
            {
                case "feedback":
                    DateTime time = DateTime.Now;
                    if (loginName == "")
                    {
                        loginName = "anonymous";
                        new AdminService().InsertUsersFeedbackComment(categary, time, loginName, feedback, url, Name, email);
                    }
                    else
                    {
                        new AdminService().InsertUsersFeedbackComment(categary, time, loginName, feedback, url, Name, email);
                    }
                    try
                    {
                        SmtpClient sc = new SmtpClient();
                        MailMessage mms = new MailMessage();
                        var config = ConfigurationManager.GetSection("feedback") as UserFeedbackSection;
                        var mailer = config.MailFeedbackHandler;
                        MailAddress to = new MailAddress(mailer.To);
                        MailAddress from = new MailAddress(mailer.FromAddress);
                        mms.To.Add(to);
                        mms.From = from;
                        mms.Subject = mailer.Subject;
                        mms.IsBodyHtml = true;
                        if (loginName == "anonymous")
                        {
                            mms.ReplyToList.Add(new MailAddress(email));
                            mms.Body = string.Format("<p>来自未登录用户： {0}</p><p>评论类型： {1}</p><p>评论内容： {2}</p><p>用户邮箱： {3}</p>", Name, categary, feedback, email);
                        }
                        else
                        {
                            DataTable dt = new AdminService().GetUserInfo("username", loginName);
                            string loginEmail = dt.Rows[0][2].ToString();
                            mms.ReplyToList.Add(new MailAddress(loginEmail));
                            mms.Body = string.Format("<p>来自已登录用户： {0}</p><p>评论类型： {1}</p><p>评论内容： {2}</p><p>用户姓名： {3}</p><p>公司： {4}</p><p>用户邮箱： {5}</p>", loginName, categary, feedback, dt.Rows[0][0], dt.Rows[0][1], loginEmail);
                        }
                        sc.UseDefaultCredentials = false;
                        sc.Credentials = new NetworkCredential(mailer.Account, mailer.Password);
                        sc.Host = mailer.Host;
                        sc.Send(mms);
                        returnObj = "{\"success\": 1}";
                    }
                    catch (Exception/* ex*/)
                    {
                        returnObj = "{\"success\": 0}";
                    }
                    break;
            }

            List<string> trustedHosts = new List<string>();
            //Add new hosts below if need cors (notice that cors header allow only one domain name)
            trustedHosts.Add("absmanager.cn-abs.com");
            trustedHosts.Add("deallab.cn-abs.com");
            string sender = context.Request.UrlReferrer == null ? "" : context.Request.UrlReferrer.Host;
            if (trustedHosts.Contains(sender))
            {
                context.Response.AddHeader("Access-Control-Allow-Origin", "https://" + sender);
            }
            context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            context.Response.Write(returnObj);
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