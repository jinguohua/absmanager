using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Foundation
{
    public class SmsConfigOption
    {
        private SmsConfigOption()
        {

        }

        public static SmsConfigOption Current { get; private set; }

        static SmsConfigOption()
        {
            Current = new SmsConfigOption();

           
          var ss=  ConfigurationManager.GetSection("smsSection");

            ConfigHandler config = (ConfigHandler)ConfigurationManager.GetSection("smsSection");

            Current.AccountSID = config.AccountID;
            Current.AppID = config.AppID;
            Current.AuthToken = config.AuthToken;
            Current.PathTemplate = config.PathFormat;
            Current.Server = config.Server;
            Current.Templates = config.Templates.ToDictonary();

        }

        public string Server { get; set; }

        public string AccountSID { get; set; }

        public string AppID { get; set; }

        public string AuthToken { get; set; }

        public string PathTemplate { get; set; }

        public Dictionary<string, string> Templates { get; set; }

        public string GetServerURL(DateTime time)
        {
            string signToken = GetSignToken(time);
            string path = String.Format(PathTemplate, AccountSID, HashHelper.GetMd5(signToken).ToUpper());
            return String.Concat(Server.Trim('/'), "/", path.Trim('/'));
        }

        public string GetAuthorization(DateTime time)
        {
            string plain = String.Concat(AccountSID, ":", time.ToString("yyyyMMddHHmmss"));
            return Base64.Encrypt(plain);
        }

        private string GetSignToken(DateTime time)
        {
            return String.Concat(AccountSID, AuthToken, time.ToString("yyyyMMddHHmmss"));
        }

        public string GetTemplate(string p)
        {
            if (Templates.ContainsKey(p))
            {
                return Templates[p];
            }
            else
            {
                throw new SMSException("Can't find the template by " + p);
            }

        }
    }

}
