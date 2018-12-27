using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager
{
    public class UserGroupManager
        : BaseModelManager<UserGroup, ABSMgrConn.TableUserGroup>
    {
        public UserGroupManager()
        {
            m_defaultTableName = "dbo.UserGroup";
            m_defaultPrimaryKey = "user_group_id";
            m_defalutFieldPrefix = "user_group_";
        }

        public List<UserGroup> GetByOwner(string Owner)
        {
            var records = Select<ABSMgrConn.TableUserGroup>("Owner", Owner);
            return records.Select(x => new UserGroup(x)).ToList();
        }
    }
}
