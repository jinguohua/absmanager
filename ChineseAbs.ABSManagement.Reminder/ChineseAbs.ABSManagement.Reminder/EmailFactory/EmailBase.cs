using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Reminder.EmailFactory
{
    public abstract class EmailBase
    {
        public EmailBase(List<Models.MessageReminding> remindings)
        {
            m_dbAdapter = new DBAdapter("system");
            m_remindings = remindings;
        }

        public void SendAllGroupByUser()
        {
            var dictByUser = m_remindings.GroupToDictList(x => x.UserName);
            var accountInfos = m_dbAdapter.Authority.GetAccountInfoByUserNames(dictByUser.Keys);

            dictByUser.ToList().ForEach(x =>
            {
                var accountInfo = accountInfos.SingleOrDefault(m => m.username == x.Key);
                var emailContent = GenerateContentByUser(x);
                if (accountInfo == null)
                {
                    return;
                }
                else if (string.IsNullOrWhiteSpace(accountInfo.email))
                {
                    return;
                }
                if (emailContent == null)
                {
                    return;
                }
                try
                {
                    SendEmailUserThread(accountInfo.email, emailContent, m_emailTitle);
                    x.Value.ForEach(m => ModifySendStatus(m, emailContent, Models.MessageStatusEnum.SendOk));
                }
                catch (Exception ex)
                {
                    x.Value.ForEach(m => ModifySendStatus(m, emailContent, Models.MessageStatusEnum.SendFail));
                    Log.Error(ex);
                }
            });
        }

        private void SendEmailUserThread(string emailTo, string emailContent, string title)
        {
            MailSender.DisplayAccount = m_emailDisplayAccount;
            MailSender.SendAsync(emailTo, emailContent, title);
        }

        private void ModifySendStatus(Models.MessageReminding reminding, string emailbody, Models.MessageStatusEnum status)
        {
            var now = DateTime.Now;
            reminding.MessageStatus = status;
            reminding.MessageTime = now;
            reminding.MessageContent = emailbody;
            reminding.LastModifyTime = now;
            reminding.LastModifyUserName = "system";
            m_dbAdapter.MessageReminding.Update(reminding);
        }
 
        abstract public void LoadData();
        abstract public string GenerateContentByUser(KeyValuePair<string, List<MessageReminding>> singleUserRemindings);

        protected List<Models.MessageReminding> m_remindings;
        protected DBAdapter m_dbAdapter;
        protected ABSMgrConnDB m_db = ABSMgrConnDB.GetInstance();
        protected string m_emailTitle;
        protected string m_emailDisplayAccount;
    }
}
