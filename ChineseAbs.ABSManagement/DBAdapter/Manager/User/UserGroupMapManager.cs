using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;


namespace ChineseAbs.ABSManagement.Manager
{
    public class UserGroupMapManager
        : BaseModelManager<UserGroupMap, ABSMgrConn.TableUserGroupMap>
    {
        public UserGroupMapManager()
        {
            m_defaultTableName = "dbo.UserGroupMap";
            m_defaultPrimaryKey = "user_group_map_id";
            m_defalutFieldPrefix = "user_group_map_";
        }

        public List<UserGroupMap> GetByUserGroupGuid(string userGroupGuid) 
        {
            var records = Select<ABSMgrConn.TableUserGroupMap>("user_group_guid", userGroupGuid);
            return records.Select(x => new UserGroupMap(x)).ToList();
        }
    }
}
