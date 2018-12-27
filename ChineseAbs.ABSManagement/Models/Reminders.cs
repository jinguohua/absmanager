using ABSMgrConn;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models
{
    public class Reminders : List<Reminder>
    {
        public Reminders() { }

        public Reminders(IEnumerable<TableReminders> queryTable)
        {
            foreach (var item in queryTable)
            {
                this.Add(new Reminder(item));
            }
        }

        public List<TableReminders> ToTableList()
        {
            var tableList = new List<TableReminders>();
            foreach (var item in this)
            {
                tableList.Add(item.GetTableObject());
            }
            return tableList;
        }
    }
    public class Reminder : BaseDataContainer<TableReminders>
    {

          public Reminder(TableReminders rem): base(rem)
        {
        }

          public Reminder() 
        {
          
        }

        public int ReminderId { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }


        public override TableReminders GetTableObject()
        {
            var rt = new TableReminders();
            rt.reminder_id = this.ReminderId;
            rt.project_id = this.ProjectId;
            rt.user_id = this.UserId;
            rt.username = this.UserName;
            rt.name = this.Name;
            rt.company = this.Company;
            rt.department = this.Department;
            rt.email = this.Email;
            rt.cellphone = this.CellPhone;
            return rt;
        }

        public override void FromTableObject(TableReminders obj)
        {
            this.ReminderId = obj.reminder_id;
            this.ProjectId = obj.project_id;
            this.UserId = obj.user_id.ToString();
            this.UserName = obj.username;
            this.Name = obj.name;
            this.Company = obj.company;
            this.Department = obj.department;
            this.Email = obj.email;
            this.CellPhone = obj.cellphone;
          
        }

    }
}
