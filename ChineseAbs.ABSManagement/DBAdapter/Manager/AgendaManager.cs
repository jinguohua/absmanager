using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public class AgendaManager 
        :BaseModelManager<Agenda, ABSMgrConn.TableAgenda>
    {
        public AgendaManager()
        {
            m_defaultTableName = "Agenda";
            m_defaultPrimaryKey = "agenda_id";
            m_defalutFieldPrefix = "agenda_";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public int NewAgenda(Agenda agenda)
        {
            agenda.Guid = Guid.NewGuid().ToString();
            agenda.CreateTime = DateTime.Now;
            agenda.AgendaStatus = AgendaStatus.Running;
            agenda.RecordStatus = RecordStatus.Valid;
            return (int)m_db.Insert(agenda.GetTableObject());
        }

        public int UpdateAgenda(Agenda agenda)
        {
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, agenda.GetTableObject(), agenda.Id);
        }

        public int DeleteAgenda(Agenda agenda)
        {
            agenda.RecordStatus = RecordStatus.Deleted;
            var count = UpdateAgenda(agenda);
            CommUtils.AssertEquals(count, 1, "Remove agenda failed : id={0};recordsCount={1}", agenda.Id, count);
            return count;
        }

        public List<Agenda> GetAgendasByProjectId(int projectId,string startDate,string endDate)
        {
            var recordsAll = Select<ABSMgrConn.TableAgenda>("project_id", projectId);
            var records = recordsAll.Where(x => x.start_time < Convert.ToDateTime(startDate) && x.end_time > Convert.ToDateTime(endDate + " 23:59:59"));
            return recordsAll.Except(records).Select(x => new Agenda(x)).ToList<Agenda>();

        }

        public Agenda GetAgendaByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableAgenda>("agenda_guid", guid);
            return new Agenda(record);
        }

        public bool Exists(string guid)
        {
            return Exists<ABSMgrConn.TableAgenda>("agenda_guid", guid);
        }

        public List<Agenda> GetAgendasByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableAgenda>("project_id", projectId);
            return records.Select(x => new Agenda(x)).ToList();
        }

        public List<Agenda> GetAgendasByProjectIdDate(int projectId, string startDate)
        {
            string sql = "select Agenda.* from  (select top 3 start_date from  (select * , convert(varchar(20), start_time,23 ) start_date from Agenda ) tb1 where start_date>='" + startDate + "' group by start_date   ) tb2 left join Agenda on tb2.start_date= convert(varchar(20), Agenda.start_time,23 )  where project_id=" + projectId + "  order by start_time ";

            var records = m_db.Query<ABSMgrConn.TableAgenda>(sql).ToList<ABSMgrConn.TableAgenda>();
            return records.Select(x => new Agenda(x)).ToList();
        }


    }
}
