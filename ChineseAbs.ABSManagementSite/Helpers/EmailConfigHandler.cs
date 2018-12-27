using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ChineseAbs.ABSManagementSite.Helpers
{
    /// <summary>
    /// Get email configuration from web.config
    /// </summary>
    public class EmailConfigHandler: ConfigurationSection
    {
        [ConfigurationProperty("mailDev")]
        public EmailSetting EmailSetting
        {
            get { return (EmailSetting)base["mailDev"]; }
        }
    }

    public class EmailSetting : ConfigurationElement
    {
        [ConfigurationProperty("host")]
        public String Host
        {
            get { return (string)base["host"]; }
        }

        [ConfigurationProperty("account")]
        public String Account
        {
            get { return (string)base["account"]; }
        }

        [ConfigurationProperty("password")]
        public String Password
        {
            get { return (string)base["password"]; }
        }

        [ConfigurationProperty("fromAddress")]
        public String FromAddress
        {
            get { return (string)base["fromAddress"]; }
        }

        [ConfigurationProperty("fromUser")]
        public String FromUser
        {
            get { return (string)base["fromUser"]; }
        }

        [ConfigurationProperty("to")]
        public String To
        {
            get { return (string)base["to"]; }
        }

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body</param>
        /// <param name="to">To</param>
        /// <param name="cc">cc</param>
        /// <param name="isHtml">Whether it's a html type mail</param>
        public void SendMail(string subject, string body, string to, string cc = "", Boolean isHtml = false)
        {
            try
            {
                // Prepare server
                var client = new SmtpClient(Host);
                client.Credentials = new NetworkCredential(Account, Password);

                // Setup mail
                var email = new MailMessage();
                email.From = new MailAddress(FromAddress, FromUser);
                email.Headers.Add("X-Priority", "3");
                email.Headers.Add("X-MSMail-Priority", "Normal");
                email.Headers.Add("X-Mailer", "Microsoft Outlook Express 6.00.2900.2869");
                email.Headers.Add("X-MimeOLE", "Produced By Microsoft MimeOLE V6.00.2900.2869");

                email.Headers.Add("ReturnReceipt", "1");
                email.To.Add(to);
                if (!String.IsNullOrEmpty(cc))
                    email.CC.Add(cc);

                // Setup content
                email.Subject = subject;
                email.IsBodyHtml = isHtml;
                email.Body = body;
                client.Send(email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occured when sending mail: ", ex);
            }
        }
    }
}