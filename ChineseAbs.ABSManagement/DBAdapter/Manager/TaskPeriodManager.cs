
using System;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.Models;
using System.Linq;


namespace ChineseAbs.ABSManagement.Manager
{
    public class TaskPeriodManager
        : BaseModelManager<TaskPeriod, ABSMgrConn.TableTaskPeriod>
    {
        public TaskPeriodManager()
        {
            m_defaultTableName = "dbo.TaskPeriod";
            m_defaultPrimaryKey = "task_period_id";
            m_defalutFieldPrefix = "task_period_";
        }

        public List<TaskPeriod> GetTaskPeriodsByProjectId(int projectId)
        {
            var records = m_db.Fetch<ABSMgrConn.TableTaskPeriod>(
                "SELECT * FROM " + m_defaultTableName
                + " where project_id = @0 and record_status_id = @1", projectId, (int)RecordStatus.Valid);

            if (records.Count == 0)
            {
                return new List<TaskPeriod>();
            }

            return records.ToList().ConvertAll(x => new TaskPeriod(x));
        }

        public List<TaskPeriod> GetByProjectIdAndPaymentDay(int projectId, DateTime paymentDate)
        {
            var records = m_db.Fetch<ABSMgrConn.TableTaskPeriod>(
                "SELECT * FROM " + m_defaultTableName
                + " where project_id = @0 and payment_date = @1 and record_status_id = @2", projectId, paymentDate, (int)RecordStatus.Valid);

            if (records.Count == 0)
            {
                return new List<TaskPeriod>();
            }

            return records.ToList().ConvertAll(x => new TaskPeriod(x));
        }

        public TaskPeriod GetByShortCode(string shortCode)
        {
            var records = m_db.Fetch<ABSMgrConn.TableTaskPeriod>(
                "SELECT * FROM " + m_defaultTableName
                + " where short_code = @0 and record_status_id = @1", shortCode, (int)RecordStatus.Valid);

            if (records.Count == 0)
            {
                return null;
            }
            else if (records.Count > 1)
            {
                throw new ApplicationException("Get date failed,data of number greater than two,please contact the admin.");
            }

            return new TaskPeriod(records.First());
        }
    }
}
