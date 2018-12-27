using ABS.Core.Models;
using SAFS.Core.Data;
using SAFS.Core.Permissions.Identity;
using SAFS.Core.Permissions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Services
{
    public class UserService : UserStore<AppUser>, SAFS.Core.Dependency.ILifetimeScopeDependency
    {
        public IRepository<UserActivity, int> ActivityRepository { get; set; }

        public bool Exist(string userName)
        {
            return UserRepository.NoTrackingEntities.FirstOrDefault(o => o.UserName == userName) != null;
        }

        public AppUser GetUserById(string id)
        {
            return UserRepository.Entities.Where(o => o.Id == id && !o.IsDeleted).FirstOrDefault();
        }

        public AppUser GetUserByNameOrEmail(string nameoremail)
        {
            String mailPattern = @"(.{3,}@.*\..*)";
            if (System.Text.RegularExpressions.Regex.IsMatch(nameoremail, mailPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return GetUserByEmail(nameoremail);
            }
            else
            {
                var user = GetUserByName(nameoremail);
                if (user != null && user.IsDeleted)
                {
                    return null;
                }
                else
                    return user;
            }
        }

        public AppUser GetUserByName(string userName)
        {
            return UserRepository.Entities.Where(o => o.UserName == userName && !o.IsDeleted).SingleOrDefault();
        }

        public AppUser GetUserByNickName(string nickName)
        {
            return UserRepository.Entities.Where(o => !o.IsDeleted && o.NickName == nickName && !o.IsDeleted).FirstOrDefault();
        }

        public AppUser GetUserByEmail(string userEmail)
        {
            return UserRepository.Entities.Where(o => !o.IsDeleted && o.Email.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
        }

        public void UpdateUserActivity(string aspnetuserID, string loginIp)
        {
            var user = UserRepository.Entities.Where(o => o.Id == aspnetuserID).FirstOrDefault();
            if (user != null)
            {
                if (user.Activity == null)
                {
                    var activity = new UserActivity();
                    activity.User = user;
                    activity.UserRegistrDate = user.CreatedTime;
                    activity.LastLoginDate = DateTime.Now;
                    activity.LastLoginIp = loginIp;
                    ActivityRepository.Insert(activity);
                }
                else
                {
                    var activity = user.Activity;
                    activity.LastLoginDate = DateTime.Now;
                    activity.LastLoginIp = loginIp;
                    ActivityRepository.Update(activity);
                }
            }
        }

        //public Task UpdateUserRolesAsync(AppUser user, string[] roles)
        //{
        //    return Task.Run(() =>
        //    {
        //        if (roles == null || roles.Length == 0)
        //        {
        //            user.Roles.Clear();
        //        }
        //        else
        //        {
        //            var deleted = user.Roles.Except(roles, a => a.Name, b => b).ToList();
        //            deleted.ForEach(role => user.Roles.Remove(role));

        //            if (user.Roles != null)
        //            {
        //                var added = roles.Except(user.Roles, a => a, b => b.Name);
        //                var rolelist = RoleRepository.Entities.Where(r => added.Contains(r.Name)).ToList();
        //                rolelist.ForEach(r => user.Roles.Add(r));
        //            }
        //            else
        //            {
        //                var rolelist = RoleRepository.Entities.Where(r => roles.Contains(r.Name)).ToList();
        //                rolelist.ForEach(r => user.Roles.Add(r));
        //            }
        //        }

        //        UserRepository.Update(user);
        //    });
        //}

        public List<AppUser> GetUsersByIds(string ids)
        {
            if (String.IsNullOrEmpty(ids))
                return new List<AppUser>();
            return GetUsersByIds(ids.Split(','));
        }

        public List<AppUser> GetUsersByIds(string[] ids)
        {
            var idArray = ids.ToList();
            return UserRepository.Get(o => !o.IsDeleted && idArray.Contains(o.Id)).ToList();
        }

        public string GetNickNameByIds(string ids)
        {
            var users = GetUsersByIds(ids);
            var nameArray = users.Select(o => !o.IsDeleted && String.IsNullOrEmpty(o.NickName) ? o.UserName : o.NickName).ToArray();
            return String.Join(",", nameArray);
        }

        public List<AppUser> GetUsers(string[] usernames)
        {
            if (usernames == null || usernames.Count() == 0)
                return new List<AppUser>();
            return UserRepository.Entities.Where(o => !o.IsDeleted).Where(o => usernames.Contains(o.UserName)).ToList();
        }

        public List<AppUser> GetAllUser()
        {
            var userList=UserRepository.Entities.Where(o => !o.IsDeleted).ToList();
            return userList;
        }

        public Dictionary<string, string> GetNicknames(string[] usernames)
        {
            var users = GetUsers(usernames);
            return users.ToDictionary(o => o.UserName, o => o.Name, StringComparer.CurrentCultureIgnoreCase);
        }

        public List<Role> GetAllRoles()
        {
          return RoleRepository.Entities.ToList();
        }
    }
}
