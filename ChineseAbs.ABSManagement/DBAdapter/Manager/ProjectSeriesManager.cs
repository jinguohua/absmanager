using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class ProjectSeriesManager : BaseManager
    {
        public ProjectSeriesManager()
        {
            m_defaultTableName = "dbo.ProjectSeries";
            m_defaultPrimaryKey = "project_series_id";
            m_defaultOrderBy = " ORDER BY create_time desc, project_series_id desc";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public ProjectSeries NewProjectSeries(string name, ProjectSeriesType type, string personInCharge, DateTime createTime, DateTime estimatedFinishTime, string email)
        {
            var projectSeries = new ABSMgrConn.TableProjectSeries()
            {
                project_series_guid = Guid.NewGuid().ToString(),
                name = name,
                type_id = (int)type,
                create_time = createTime,
                create_user_name = UserInfo.UserName,
                estimated_finish_time = estimatedFinishTime,
                person_in_charge = personInCharge,
                record_status_id = (int)RecordStatus.Valid,
                project_series_stage_id = (int)ProjectSeriesStage.发行,
                email = email,
            };

            projectSeries.project_series_id = Insert(projectSeries);
            return new ProjectSeries(projectSeries);
        }

        public int RemoveByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableProjectSeries>("project_series_guid", guid);
            var obj = new ProjectSeries(record);
            obj.RecordStatus = RecordStatus.Deleted;
            return UpdateProjectSeries(obj);
        }

        public ProjectSeries GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableProjectSeries>("project_series_guid", guid);
            return new ProjectSeries(record);
        }

        public List<ProjectSeries> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TableProjectSeries, string>("project_series_guid", guids);
            return records.ToList().ConvertAll(x => new ProjectSeries(x));
        }
        public List<ProjectSeries> GetByPersonInCharge(string personInCharge) 
        {
            var records = Select<ABSMgrConn.TableProjectSeries>("person_in_charge", personInCharge).ToList();
            return records.ConvertAll(x => new ProjectSeries(x));
        }
        
        public bool Exists(int id)
        {
            return Exists<ABSMgrConn.TableProjectSeries>("project_series_id", id);
        }

        public class ProjectSeriesBasicInfo
        {
            public string ProjectSeriesName { get; set; }
            public string ProjectSeriesGuid { get; set; }
            public string CurrentProjectGuid { get; set; }
        }

        public List<ProjectSeriesBasicInfo> GetNameAndGuidByGuids(IEnumerable<string> guids)
        {
            if (guids.Count() == 0)
            {
                return new List<ProjectSeriesBasicInfo>();
            }

            var sql = "SELECT proj_ser.project_series_guid AS ProjectSeriesGuid,"
                + " proj_ser.name AS ProjectSeriesName,"
                + " proj.project_guid AS CurrentProjectGuid"
                + " FROM dbo.ProjectSeries as proj_ser "
                + " JOIN dbo.Project as proj "
                + " ON proj_ser.project_series_id = proj.project_series_id "
                + " WHERE proj_ser.record_status_id <> @recordStatusDeleted "
                + " AND proj_ser.project_series_stage_id = proj.type_id "
                + " AND proj_ser.project_series_guid IN (@guids)";

            var recordStatusDeleted = RecordStatus.Deleted;
            var records = m_db.Fetch<ProjectSeriesBasicInfo>(sql, new { guids, recordStatusDeleted });
            return records;
        }

        public ProjectSeries GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableProjectSeries>(id);
            return new ProjectSeries(record);
        }

        public List<ProjectSeries> GetByIds(IEnumerable<int> ids)
        {
            var records = Select<ABSMgrConn.TableProjectSeries, int>(ids);
            return records.ToList().ConvertAll(x => new ProjectSeries(x));
        }

        public int UpdateProjectSeries(ProjectSeries projectSeries)
        {
            var projectSeriesTable = projectSeries.GetTableObject();
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, projectSeriesTable, projectSeries.Id);
        }
    }
}
