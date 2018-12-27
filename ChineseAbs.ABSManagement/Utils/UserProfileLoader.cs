using ABS.Core.Services;
using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    public class UserProfileLite
    {
        public UserProfileLite()
        {

        }

        public UserProfileLite(UserProfile userProfile)
        {
            RealName = userProfile.RealName;
            UserName = userProfile.UserName;
        }

        public string RealName { get; set; }

        public string UserName { get; set; }
    }

    public class UserProfileLoader: SAFS.Core.Dependency.ILifetimeScopeDependency
    {
        public DBAdapter DBAdapter
        {
            get {
                return m_dbAdapter;
            }
            set
            {
                m_dbAdapter = value;
            }
        }

        public UserService UserService { get; set; }

        public UserProfileLoader()
        {
        }

        public void Precache(IEnumerable<string> userNames)
        {
            
        }


        public string GetRealName(string userName)
        {
            var userProfileLite = Get(userName);
            return userProfileLite == null ? null : userProfileLite.RealName;
        }

        public string GetDisplayRealNameAndUserName(string userName)
        {
            if (CommUtils.IsLocalDeployed())
            {
                return userName;
            }

            var profile = Get(userName);
            if (profile != null)
            {
                return string.IsNullOrWhiteSpace(profile.RealName) || profile.UserName.Equals(profile.RealName)
                    ? profile.UserName : profile.RealName + "(" + profile.UserName + ")";
            }

            return userName;
        }

        public UserProfileLite Get(string userName)
        {
            return new UserProfileLite(GetUserProfile(userName));
        }

        public UserProfile GetUserProfile(string userName)
        {
            var user = UserService.GetUserByNickName(userName);

            return new UserProfile()
            {
                RealName = user == null ? "-" : user.Name,
                UserName = user == null ? "-" : user.UserName,
                AvatarPath = CommUtils.DefaultAvatarPath,
            };
        }

        private void LoadAllUserProfiles()
        {
            var nicknames = UserService.GetAllUser().Select(o => new { o.UserName, o.Name });
            m_userProfiles = new Dictionary<string, UserProfile>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var userProfile in nicknames)
            {
                m_userProfiles.Add(userProfile.UserName, new UserProfile { UserName = userProfile.UserName, RealName = userProfile.Name });
            }
        }

        private Dictionary<string, UserProfile> m_userProfiles;

        private DBAdapter m_dbAdapter;
    }
}
