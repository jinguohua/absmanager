using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    public class SuperUserManager : BaseManager
    {
        public SuperUserManager()
        {
            m_defaultTableName = "dbo.SuperUser";
            m_defaultPrimaryKey = "super_user_id";
            m_defaultOrderBy = " ORDER BY super_user_name desc";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public bool IsSuperUser()
        {
            CommUtils.AssertHasContent(UserInfo.UserName, "Username is empty");
            var records = Select<ABSMgrConn.TableSuperUser>("super_user_name", UserInfo.UserName);
            return records.Count() == 1;
        }
    }
}
