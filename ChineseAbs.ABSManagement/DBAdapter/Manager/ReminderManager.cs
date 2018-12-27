using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    public class ReminderManager :BaseManager
    {
        public ReminderManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerReminder(UserInfo);
        }

        #region Methods

        public Reminders GetReminders() 
        {
            var tblReminders = m_db.Query<TableReminders>(" select * from dbo.Reminders ");
            Reminders rt = new Reminders();
            foreach (var r in tblReminders)
            {
                Reminder reminder = new Reminder(r);
                reminder = Decrypt(reminder);
                rt.Add(reminder);
            }
            return rt;
        }

        public Reminders GetReminders(int projectId)
        {
            var tblReminders = m_db.Query<TableReminders>("select * from dbo.Reminders where project_id = @0 ",projectId);
            Reminders rt = new Reminders();
            foreach (var r in tblReminders)
            {
                Reminder reminder = new Reminder(r);
                reminder = Decrypt(reminder);
                rt.Add(reminder);
            }
            return rt;
        }

        public int AddReminders(Reminder reminder)
        {
            reminder = Encrypt(reminder);
            m_logger.Log(reminder.ProjectId, "新增提醒人");
            return (int)m_db.Insert(reminder.GetTableObject());
        }

        public void DeleteReminders(int reminderId,int projectId)
        {
            m_logger.Log(projectId, "删除提醒人");
            m_db.Delete<TableReminders>("where reminder_id = @0 ",reminderId);
        }

        public Reminders GetUser (string info)
        {
            string sql = @"select a.userid,a.userName,w.Name,w.company,w.department,email,cellPhone from chineseAbs.dbo.aspnet_users a
                            left outer join chineseabs.web.accountapplication w on a.username = w.username 
                            where a.username = @0 or w.email= @0 or w.cellPhone = @0 ";
            var tblReminders = m_db.Query<TableReminders>(sql, info);
            Reminders rt = new Reminders();
            foreach (var r in tblReminders)
            {
                Reminder reminder = new Reminder(r);
                rt.Add(reminder);
            }
            return rt;
        }

        public int GetMaxReminderId()
        {
            var id  = m_db.Single<int>(" select ident_current('dbo.reminders') ");
            return id;
        }

        #endregion

        public Reminder Encrypt(Reminder reminder)
        {
            reminder.UserName = RsaUtils.Encrypt(reminder.UserName);
            reminder.Email = RsaUtils.Encrypt(reminder.Email);
            reminder.CellPhone = RsaUtils.Encrypt(reminder.CellPhone);
            return reminder;
        }

        public Reminder Decrypt(Reminder reminder)
        {
            reminder.UserName = RsaUtils.Decrypt<string>(reminder.UserName);
            reminder.Email = RsaUtils.Decrypt<string>(reminder.Email);
            reminder.CellPhone = RsaUtils.Decrypt<string>(reminder.CellPhone);
            return reminder;
        }
    }
}
