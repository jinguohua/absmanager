﻿using System.Collections.Generic;
using System.Web;
using ChineseAbs.Logic;

namespace ChineseAbs.Web.Menu.Ajax
{
    /// <summary>
    /// CheckDailyHandler 的摘要说明
    /// </summary>
    public class CheckDailyHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            PointsService ds = new PointsService();
            string username = context.User.Identity.Name;
            bool Done = ds.IsUserDoneDaily(username);
            string ret;
            if (Done)
            {
                ret = "{\"Done\": true}";
            } else
            {
                ret = "{\"Done\": false}";
            }
            context.Response.ContentType = "application/json";
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
            context.Response.Write(ret);
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