using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    //public class LocalDeployedManager : BaseManager
    //{
    //    public LocalDeployedManager()
    //    {
    //    }

    //    protected override Loggers.AbstractLogger GetLogger()
    //    {
    //        return new Loggers.LoggerGeneric(UserInfo);
    //    }

    //    public LocalDeployedUserProfile GetUserProfile(string userName)
    //    {
    //        var records = Select<ABSMgrConn.TableLocalDeployedUserProfile>(
    //            "[dbo].[LocalDeployedUserProfile]", "user_name", userName);
    //        var userProfiles = records.ToList().ConvertAll(x => new LocalDeployedUserProfile(x));
    //        CommUtils.Assert(userProfiles.Count < 2, "重复的用户名[{0}]", userName);

    //        LocalDeployedUserProfile profile = null;
    //        if (userProfiles.Count > 0)
    //        {
    //            profile = userProfiles.Single();
    //        }
    //        else
    //        {
    //            profile = new LocalDeployedUserProfile();
    //            profile.Guid = Guid.NewGuid().ToString();
    //            profile.UserName = userName;
    //            profile.RealName = userName;
    //            profile.Company = string.Empty;
    //            profile.Department = string.Empty;
    //            profile.Email = string.Empty;
    //            profile.Cellphone = string.Empty;
    //            profile = NewUserProfile(profile);
    //        }

    //        return profile;
    //    }

    //    public List<LocalDeployedUserProfile> GetUserProfiles(List<string> userNames)
    //    {
    //        var records = Select<ABSMgrConn.TableLocalDeployedUserProfile, string>(
    //            "[dbo].[LocalDeployedUserProfile]", "user_name", userNames, "");
    //        var userProfiles = records.ToList().ConvertAll(x => new LocalDeployedUserProfile(x));
    //        return userProfiles;
    //    }

    //    public LocalDeployedUserProfile NewUserProfile(LocalDeployedUserProfile userProfile)
    //    {
    //        userProfile.Guid = Guid.NewGuid().ToString();
    //        userProfile.Id = Insert("[dbo].[LocalDeployedUserProfile]", "user_profile_id", userProfile.GetTableObject());
    //        return userProfile;
    //    }

    //    public int UpdateUserProfile(LocalDeployedUserProfile userProfile)
    //    {
    //        return m_db.Update("[dbo].[LocalDeployedUserProfile]", "user_profile_id", userProfile.GetTableObject());
    //    }
    //}
}
