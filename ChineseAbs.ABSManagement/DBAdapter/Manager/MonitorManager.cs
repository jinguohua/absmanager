using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class MonitorManager : BaseManager
    {
        public MonitorManager () { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerNewsKeyWords(UserInfo);
        }

        public void SetNewsKeyWords(int id, string names ,string range )
        {
            var count = m_db.ExecuteScalar<int>("select count(*) from dbo.Objects where Project_id= @0 ", id);
            if (count == 0)
            {
                var obj = new TableObjects();
                obj.project_id = id;
                obj.names = names;
                obj.range = range;
                m_logger.Log(id, "新增监控机构名新闻");
                m_db.Insert(obj);
            }
            else
            {
                m_logger.Log(id, "更新监控机构名新闻");
                m_db.Update<TableObjects>(" set names= @0,range = @1 where Project_id=@2 ", names, range , id);
            }
        }

        public NewsSetting GetNewsSettingByProjectId(int projectId)
        {
            NewsSetting newsSetting = m_db.SingleOrDefault<NewsSetting>(" select names as keyWords , range as range from dbo.Objects where project_id = @0", projectId);
            return newsSetting;
        }

    }
}
