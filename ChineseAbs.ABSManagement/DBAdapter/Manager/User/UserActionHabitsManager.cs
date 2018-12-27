using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager
{
    public class UserActionHabitsManager
        : BaseModelManager<UserActionHabits, ABSMgrConn.TableUserActionHabits>
    {
        public UserActionHabitsManager()
        {
            m_defaultTableName = "dbo.UserActionHabits";
            m_defaultPrimaryKey = "user_action_habits_id";
            m_defalutFieldPrefix = "user_action_habits_";
        }

        public List<UserActionHabits> GetByUserName(string userName)
        {
            var records = this.Select<ABSMgrConn.TableUserActionHabits>(m_defaultTableName, "user_name", userName);
            return records.Select(x => new UserActionHabits(x)).ToList();
        }

        public int DeleteByUserName(string userName)
        {
            var actionHabits = GetByUserName(userName);
            if (actionHabits.Count == 0)
            {
                return 0;
            }

            var ids = actionHabits.Select(x => x.Id);
            var sql = "delete from " + m_defaultTableName
                + " where " + m_defaultPrimaryKey + " in (@ids)";
            return m_db.Execute(sql, new { ids = ids });
        }
    }
}
