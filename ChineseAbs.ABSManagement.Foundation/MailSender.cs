using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;

namespace ChineseAbs.ABSManagement.Foundation
{
    public class MailSender
    {
        private MailSender()
        {
        }

        private static MailSender GetInstance()
        {
            if (m_sender == null)
            {
                lock (m_lockObj)
                {
                    if (m_sender == null)
                    {
                        m_sender = new MailSender();
                        m_sender.Initialize();
                    }
                }
            }

            return m_sender;
        }

        private class MailConfigOption
        {
            public string servername { set; get; }
            public int smtpPort { set; get; }
            public int pop3Port { set; get; }
            public string adminaccount { set; get; }
            public string displayadminaccount { set; get; }
            public string adminpassword { set; get; }
        }

        private MailConfigOption ParseMailSetting()
        {
            var config = System.Configuration.ConfigurationManager.AppSettings["MailConfig"];
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MailConfigOption>(config);
        }

        private void Initialize()
        {
            Log.Info("启动邮件服务...");
            m_mailConfig = ParseMailSetting();

            m_queue = new ConcurrentQueue<MailMessage>();
            m_thread = new System.Threading.Thread(Run)
            {
                IsBackground = true,
                Name = "ThreadEmailSender"
            };

            m_running = true;
            m_thread.Start();
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient(m_mailConfig.servername, m_mailConfig.smtpPort);
            smtpClient.Credentials = new System.Net.NetworkCredential(m_mailConfig.adminaccount, m_mailConfig.adminpassword);
            smtpClient.Timeout = 60000;
            return smtpClient;
        }

        public MailMessage CreateEmailMessage(string emailTo, string emailContent, string title)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(m_mailConfig.adminaccount, m_mailConfig.displayadminaccount);
            mail.To.Add(emailTo);
            mail.Subject = title;
            mail.IsBodyHtml = true;
            mail.Body = emailContent;
            return mail;
        }

        public static void SendAsync(string emailTo, string emailContent, string title)
        {
            GetInstance().EnqueueMail(emailTo, emailContent, title);
        }

        private void EnqueueMail(string emailTo, string emailContent, string title)
        {
            if (string.IsNullOrWhiteSpace(DisplayAccount))
            {
                m_mailConfig.displayadminaccount = m_mailConfig.displayadminaccount ?? m_mailConfig.adminaccount;
            }
            else
            {
                m_mailConfig.displayadminaccount = DisplayAccount;
            }
            m_queue.Enqueue(CreateEmailMessage(emailTo, emailContent, title));
        }

        private void Run()
        {
            Log.Info("邮件服务已启动");
            while (true)
            {
                MailMessage mail;
                if (m_queue.TryDequeue(out mail))
                {
                    string mailTo = string.Empty;
                    try
                    {
                        mailTo = string.Join(";", mail.To.Select(x => x.Address).ToArray());
                        Log.Info("正在发送邮件到：" + mailTo + "...");

                        CreateSmtpClient().Send(mail);
                    }
                    catch (Exception e)
                    {
                        Log.Error("发送邮件[" + mailTo + "]失败", e);
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                }

                if (!m_running && m_queue.Count == 0)
                {
                    break;
                }
            }
        }

        public static void StopService()
        {
            GetInstance().Stop();
        }

        private void Stop()
        {
            Log.Info("停止邮件服务...");
            if (!m_running)
            {
                System.Threading.Thread.Sleep(5000);
            }

            m_running = false;
            if (m_thread != null)
            {
                m_thread.Join();
            }
            Log.Info("邮件服务已停止");
        }

        #region 变量

        private bool m_running = false;

        private System.Threading.Thread m_thread;
        private ConcurrentQueue<MailMessage> m_queue;

        private static MailSender m_sender;
        private static Object m_lockObj = new object();

        private MailConfigOption m_mailConfig;
        #endregion

        #region 属性
        public static string DisplayAccount { get; set; }
        #endregion
    }
}
