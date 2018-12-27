using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABS.ABSManagementSite
{
    public class UserViewModel
    {
        public string ID { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        public string RepeatPassword { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Departments { get; set; }

        public string[] Roles { get; set; }

        public string RoleNames { get; set; }

    }

    public class BaseUserInfo
    {
        public string UserID { get; set; }

        public string Avatar { get; set; }

        public int NotifyCount { get; set; }

        public int UnreadCount { get; set; }

        public string Name { get; set; }

        public string NickName { get; set; }

        public string Title { get; set; }

        public string Phone { get; set; }

        public string Group { get; set; }

        public string[] Roles { get; set; }
    }
}