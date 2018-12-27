using ChineseAbs.ABSManagement.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.Email
{
    public class EmailLogicModel : EmailBaseLogic
    {
        public EmailLogicModel(string username) : base(username)
        {
            m_inbox = new Inbox(username);
            m_sent = new Sent(username);
        }

        //收件箱
        private Inbox m_inbox;
        public Inbox Inbox { get { return m_inbox; } }

        //已发送
        public Sent m_sent;
        public Sent Sent { get { return m_sent; } }

        //草稿箱
        public Draft m_draft;
        public Draft Draft { get { return m_draft; } }

        //写信
        public void Compose(List<string> toAddrArr, string subject, string content, int isDraft, DateTime? sentTime)
        {
            var emailCont = new EmailContext();
            emailCont.Subject = subject;
            emailCont.Content = content;
            emailCont.IsDraft = isDraft;
            emailCont.sentTime = sentTime;
            var newEmailId = m_dbAdapter.EmailContext.New(emailCont).Id;
            toAddrArr.ForEach(x =>
            {
                var emailFromTo = new EmailFromTo();
                emailFromTo.From = EmailAddress;
                emailFromTo.To = x;
                m_dbAdapter.EmailFromTo.New(emailFromTo);
            });
        }

        //通讯录
        public void Contact()
        {
        }


    }
}
