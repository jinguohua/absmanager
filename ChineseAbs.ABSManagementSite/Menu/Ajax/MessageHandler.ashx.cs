using ChineseAbs.Logic;
using ChineseAbs.Logic.Object;
using System;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChineseAbs.Web.Menu.Ajax
{
    /// <summary>
    /// Summary description for MessageHandler
    /// </summary>
    public class MessageHandler : IHttpHandler
    {
        MessageService m_messageService = new MessageService();
        string m_userName = "";
        HttpContext m_context;

        public void ProcessRequest(HttpContext context)
        {
            MessageCountInfo mci = new MessageCountInfo();
            if (context.User.Identity.IsAuthenticated)
            {
                try
                {
                    m_userName = context.User.Identity.Name;
                    m_context = context;

                    string action = context.Request["action"];
                    if (!string.IsNullOrEmpty(action))
                    {
                        switch (action)
                        {
                            case "getunread":
                                int total = m_messageService.CountMessage(m_userName, "all", 0);
                                int managecount = m_messageService.CountABSManagementUnreadMessages(m_userName);
                                int ticketCount = m_messageService.CountTicketUnreadMseeages(m_userName);
                                mci.OK = true;
                                mci.managecount = managecount;
                                mci.total = total;
                                mci.ticketCount = ticketCount;
                                mci.abscount = total - managecount - ticketCount;
                                break;
                            case "mark":
                                int state = 0;
                                if (int.TryParse(context.Request["state"], out state) && Math.Abs(state) == 1)
                                    if (ChangeStateMultiple(context.Request["contentids"].Trim(), state))
                                        mci.OK = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception/* ex*/)
                {
                    mci.OK = false;
                }
            }
            else
            {
                mci.OK = false;
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
            context.Response.Write(JsonConvert.SerializeObject(mci));
        }

        private bool ChangeStateSingle(string ci, int state)
        {
            if (string.IsNullOrEmpty(ci))
                return false;
            int contentId = 0;
            if (!int.TryParse(ci, out contentId) || contentId <= 0)
                return false;
            Message content = m_messageService.GetContent(contentId);
            if (content == null)
                return false;
            if (content.Type == "public" || content.Type == "organization")
            {
                return m_messageService.ChangeStatePublic(m_userName, contentId, state);
            }
            else if (content.Type == "group")
            {
                if (m_context.User.IsInRole(content.Role))
                    return m_messageService.ChangeStatePublic(m_userName, contentId, state);
                return false;
            }
            else
            {
                Message msg = m_messageService.GetMessage(m_userName, contentId);
                if (msg == null)
                    return m_messageService.ChangeStatePublic(m_userName, contentId, state);
                else
                {
                    return m_messageService.ChangeStatePrivate((int)msg.MessageId, state);
                }
            }
        }

        private bool ChangeStateMultiple(string cis, int state)
        {
            if (string.IsNullOrEmpty(cis))
                return false;
            string[] arrContentIds = cis.Replace(" ", "").Split(',');
            int len = arrContentIds.Length;
            if (len > 0 && len < 11)
            {
                bool valid = true;
                for (int i = 0; i < len; i++)
                {
                    if (!ChangeStateSingle(arrContentIds[i], state))
                    {
                        valid = false;
                        break;
                    }
                }
                return valid;
            }
            return false;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class MessageCountInfo
        {
            public bool OK { get; set; }
            public int abscount { get; set; }
            public int managecount { get; set; }
            public int ticketCount { get; set; }
            public int total { get; set; }
        }
    }
}