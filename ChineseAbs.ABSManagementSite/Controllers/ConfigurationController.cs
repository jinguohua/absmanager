using System;
using System.Web.Mvc;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Helpers;
using ChineseAbs.ABSManagementSite.Models;
using Newtonsoft.Json;
using ChineseAbs.ABSManagement.Utils;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ConfigurationController : BaseController
    {
        public ActionResult Index(string projectGuid)
        {
            int projectId = -1;
            if (!string.IsNullOrEmpty(projectGuid))
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                projectId = project.ProjectId;
            }

            ConfigurationViewModel config = GetConfigInfoByProjectId(projectId.ToString());
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            config.ProjectList = projects;

            var users = UserService.GetAllUser().Where(o => !o.IsDeleted);
            List<SelectListItem> items = users.Select(o =>
                    new SelectListItem() { Value = o.UserName, Text = o.Name + "(" + o.UserName + ")" })
                .ToList();
            var allUserNames = items.Select(o => o.Value).ToList();

            config.AuthedAccountList = new List<SelectListItem>();
            config.AuthedAccountList.Add(new SelectListItem
            {
                Selected = true,
                Text = "-请选择-",
                Value = string.Empty
            });

            config.AuthedAccountList.AddRange(items);

            //var accounts = m_dbAdapter.BankAccount.GetAccounts(projectId, false);
            //var contacts = m_dbAdapter.Contact.GetContacts(projectId);
            //var reminders = m_dbAdapter.Reminder.GetReminders(projectId);
            //var authedAccount  = m_dbAdapter.Authority.GetAllAuthedAccount(User.Identity.Name);
            //ConfigurationViewModel config = new ConfigurationViewModel();
            ////var sList = SerializeHelper.GetSelectList(projects);
            //var aaList = SerializeHelper.GetAuthedAccountSelectList(authedAccount);
            ////config.ProjectList = sList;
            //config.ProjectList = projects;
            //config.AccountList = accounts;
            //config.ContactList = contacts;
            //config.ReminderList = reminders;
            //config.AuthedAccountList = aaList;

            return View(config);
        }

        [HttpPost]
        public ActionResult DeleteReminder(string id, string projectId)
        {
            m_dbAdapter.Reminder.DeleteReminders(Int32.Parse(id), Int32.Parse(projectId));
            return Content("Success");
        }

        [HttpPost]
        public ActionResult AddNotifyUser(string projectId, string userId, string userName, string name, string company, string department, string email, string cellPhone)
        {
            int id = m_dbAdapter.Reminder.GetMaxReminderId() + 1;
            Reminder reminder = new Reminder()
            {
                ReminderId = id,
                ProjectId = Int32.Parse(projectId),
                UserId = (userId),
                UserName = userName,
                Name = name,
                Company = company,
                Department = department,
                Email = email,
                CellPhone = cellPhone
            };
            m_dbAdapter.Reminder.AddReminders(reminder);
            return Content(id.ToString());
        }

        [HttpPost]
        public ActionResult NewsKeyWordsConfig(string projectId, string key, string range)
        {
            try
            {
                m_dbAdapter.Monitor.SetNewsKeyWords(Int32.Parse(projectId), key, range);
                return Content("Success");
            }
            catch
            {
                return Content("Error");
            }
        }

        [HttpPost]
        public ActionResult GetConfigByProjectId(string projectId)
        {
            ConfigurationViewModel model = GetConfigInfoByProjectId(projectId);

            return Content(JsonConvert.SerializeObject(model));
        }

        public ConfigurationViewModel GetConfigInfoByProjectId(string projectId)
        {
            int id = -1;
            int.TryParse(projectId, out id);
            var accounts = m_dbAdapter.BankAccount.GetAccounts(id, false);
            var contacts = m_dbAdapter.Contact.GetContacts(id);
            var reminders = m_dbAdapter.Reminder.GetReminders(id);
            var setting = m_dbAdapter.RemindSetting.GetSettingByProjectId(id);
            NewsSetting newsSetting = m_dbAdapter.Monitor.GetNewsSettingByProjectId(id);
            if (newsSetting == null)
            {
                newsSetting = new NewsSetting() { KeyWords = "", Range = "" };
            }

            if (setting == null)
            {
                RemindSettings rs = new RemindSettings()
                {
                    ProjectId = id,
                    Frequency = int.Parse(MyEnumConvertor.MyRemindSettingDictionary["H24"]),
                    RemindType = int.Parse(MyEnumConvertor.MyRemindSettingDictionary["短信加邮件"]),
                    AutoRemind = false,
                    RemindDaily = false,
                };
                m_dbAdapter.RemindSetting.AddRemindSettings(rs);
                setting = m_dbAdapter.RemindSetting.GetSettingByProjectId(id);
            }

            ConfigurationViewModel model = new ConfigurationViewModel();
            model.AccountList = accounts;
            model.ContactList = contacts;
            model.ReminderList = reminders;
            model.RemindSettings = setting;
            model.NewsSetting = newsSetting;

            if (id > 0)
            {
                var project = m_dbAdapter.Project.GetProjectById(id);
                model.CurrentProjectGuid = project.ProjectGuid;
            }

            return model;
        }

        [HttpPost]
        public ActionResult SaveRemindSettings(string rowId, string projectId, string autoRemind, string frequency, string typeId, string remindDaily)
        {
            RemindSettings rs = new RemindSettings()
            {
                RowId = int.Parse(rowId),
                ProjectId = int.Parse(projectId),
                AutoRemind = bool.Parse(autoRemind),
                Frequency = int.Parse(frequency),
                RemindType = int.Parse(typeId),
                RemindDaily = bool.Parse(remindDaily),
            };
            m_dbAdapter.RemindSetting.UpdateRemindSettings(rs);
            return Content("Success");
        }

        public JsonResult SearchUser(string userInfo)
        {
            Reminders reminders = new Reminders();
            var user = UserService.GetUserByName(userInfo);
            if (user != null)
            {
                Reminder r = new Reminder
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Company = "",
                    Department = "",
                    Email = user.Email,
                    CellPhone = user.PhoneNumber
                };
                reminders.Add(r);
            }
            var result = new SAFS.Utility.Web.JsonResultDataEntity<Reminders>();
            result.Data = reminders;
            return Json(result);
        }
    }
}
