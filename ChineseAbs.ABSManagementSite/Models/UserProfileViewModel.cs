using ABS.Core.Models;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class TeamMemberListViewModel
    {
        public TeamMemberListViewModel()
        {
            PersonInCharge = new TeamMemberViewModel();
            Creator = new TeamMemberViewModel();
            TeamAdmins = new List<TeamMemberViewModel>();
            TeamMembers = new List<TeamMemberViewModel>();
        }

        public TeamMemberViewModel PersonInCharge { get; set; }

        public TeamMemberViewModel Creator { get; set; }

        public List<TeamMemberViewModel> TeamAdmins { get; set; }

        public List<TeamMemberViewModel> TeamMembers { get; set; }
    }

    public class TeamMemberViewModel
    {
        public TeamMemberViewModel(UserProfileLite user)
        {
            Permission = new PermissionViewModel();
            UserName = user.UserName;
            RealName = user.RealName;
        }

        public TeamMemberViewModel()
        {
            Permission = new PermissionViewModel();
        }

        public string UserName { set; get; }

        public string RealName { set; get; }

        public string AvatarPath { get; set; }

        public PermissionViewModel Permission { get; set; }
    }

    public class PermissionViewModel
    {
        public PermissionViewModel()
        {
            Set(false, false, false);
        }

        public PermissionViewModel(bool read, bool write, bool execute)
        {
            Set(read, write, execute);
        }

        public void Set(bool read, bool write, bool execute)
        {
            Read = read;
            Write = write;
            Execute = execute;
        }

        public void Set(List<Permission> permissions)
        {
            Read = false;
            Write = false;
            Execute = false;

            foreach (var permission in permissions)
            {
                switch(permission.Type)
                {
                    case PermissionType.Read:
                        Read = true;
                        break;
                    case PermissionType.Write:
                        Write = true;
                        break;
                    case PermissionType.Execute:
                        Execute = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool Read { get; set; }
        
        public bool Write { get; set; }

        public bool Execute { get; set; }
    }
}