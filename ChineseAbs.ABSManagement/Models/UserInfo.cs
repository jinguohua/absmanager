using ChineseAbs.ABSManagement.Framework;

namespace ChineseAbs.ABSManagement.Models
{
    public class UserInfo
    {
        public UserInfo()
        {
            this.UserName = Platform.UserName;
        }

        public UserInfo(string userName)
        {
            this.UserName = userName;
        }

        public string UserName { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}
