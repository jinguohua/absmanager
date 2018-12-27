using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ChineseAbs.ABSManagement.Foundation
{
    public class ConfigHandler : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("accountID", IsRequired = true, IsKey = true)]
        public string AccountID
        {
            get
            {
                return this["accountID"].ToStringSafe();
            }
            set
            {
                this["accountID"] = value;
            }
        }

        [ConfigurationProperty("authToken", IsRequired = true)]
        public string AuthToken
        {
            get
            {
                return this["authToken"].ToStringSafe();
            }
            set
            {
                this["authToken"] = value;
            }
        }

        [ConfigurationProperty("appID", IsRequired = true)]
        public string AppID
        {
            get
            {
                return this["appID"].ToStringSafe();
            }
            set
            {
                this["appID"] = value;
            }
        }

        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get
            {
                return this["server"].ToStringSafe();
            }
            set
            {
                this["server"] = value;
            }
        }

        [ConfigurationProperty("pathFormat", IsRequired = true)]
        public string PathFormat
        {
            get
            {
                return this["pathFormat"].ToStringSafe();
            }
            set
            {
                this["pathFormat"] = value;
            }
        }

        [ConfigurationProperty("templates", IsRequired = true)]
        [ConfigurationCollection(typeof(TemplateCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public TemplateCollection Templates
        {
            get
            {
                return this["templates"] as TemplateCollection;
            }
        }
    }


    public class TemplateCollection : ConfigurationElementCollection
    {
        public Dictionary<string, string> ToDictonary()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (TemplateEntity t in this) {
                dic.Add(t.Name, t.Value);
            }
            return dic;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplateEntity();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as TemplateEntity).Name;
        }
    }

    public class TemplateEntity : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return this["name"].ToStringSafe();
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get
            {
                return this["value"].ToStringSafe();
            }
            set
            {
                this["value"] = value;
            }
        }
    }
}
