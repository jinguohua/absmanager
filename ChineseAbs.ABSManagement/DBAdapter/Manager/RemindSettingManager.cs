using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class RemindSettingManager : BaseManager
    {
        public RemindSettingManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerRemindSetting(UserInfo);
        }

        #region Methods
        public RemindSettings GetSettingByProjectId(int projectId)
        {
            var tableSettings = m_db.Query<TableRemindSettings>("select * from dbo.RemindSettings where project_id = @0", projectId);

            RemindSettings rs = null;
            foreach (var item in tableSettings)
            {
                rs = new RemindSettings(item);
            }
            return rs;
        }

        public void AddRemindSettings(RemindSettings rs)
        {
            m_logger.Log(rs.ProjectId, "新增默认提醒设置");
            m_db.Insert(rs.GetTableObject());
        }

        public void UpdateRemindSettings(RemindSettings rs)
        {
            m_logger.Log(rs.ProjectId, "更新提醒设置");
            m_db.Update("RemindSettings", "row_id", rs.GetTableObject(), rs.RowId);
        }

        public int JudgeByProjectId(int projectId)
        {
           return m_db.ExecuteScalar<int>("select count(*) from dbo.RemindSettings where project_id = @0", projectId);
        }

        public int GetRowIdByProjectId(int projectId)
        {
            return m_db.Single<int>("select row_id from dbo.RemindSettings where project_id = @0 ", projectId);
        }
        #endregion

    }
}
