using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Helpers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class ConfigurationViewModel
    {
        public ConfigurationViewModel()
        {
            //ProjectList = new List<SelectListItem>();
            AccountTypeList = MyEnumConvertor.ConvertToSelectList(typeof(EAccountType));
            DutyTypeList = MyEnumConvertor.ConvertToSelectList(typeof(EDutyType));
            FrequencyList = MyEnumConvertor.ConvertToSelectList(typeof(EFrequency));
            RemindTypeList = MyEnumConvertor.ConvertToSelectList(typeof(ERemindType));
        }
        public string CurrentProjectGuid { get; set; }
        public List<Project> ProjectList { get; set; }
        public NewsSetting NewsSetting { get; set; }
        public Accounts AccountList { get; set; }
        public Contacts ContactList { get; set; }
        public Reminders ReminderList { get; set; }
        //public List<SelectListItem> ProjectList { get; set; }
        public List<SelectListItem> AccountTypeList { get; set; }
        public List<SelectListItem> DutyTypeList { get; set; }
        public List<SelectListItem> FrequencyList { get; set; }
        public List<SelectListItem> RemindTypeList { get; set; }

        public List<SelectListItem> AuthedAccountList { get; set; }

        public RemindSettings RemindSettings { get; set; }
    }
}