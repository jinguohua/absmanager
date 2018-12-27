
using ChineseAbs.ABSManagement.Models;
using System;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Manager
{
    public class CacheAcfReceiveManager
        : BaseModelManager<CacheAcfReceive, ABSMgrConn.TableCacheAcfReceive>
    {
        public CacheAcfReceiveManager()
        {
            m_defaultTableName = "dbo.CacheAcfReceive";
            m_defaultPrimaryKey = "cache_acf_receive_id";
            m_defalutFieldPrefix = "cache_acf_receive_";
        }

        public void Save(int projectId, DateTime paymentDay,
            double principal, double interest)
        {
            var record = Get(projectId, paymentDay);
            if (record == null)
            {
                var newRecord = new CacheAcfReceive();
                newRecord.ProjectId = projectId;
                newRecord.PaymentDay = paymentDay;
                newRecord.Principal = principal;
                newRecord.Interest = interest;
                New(newRecord);
            }
            else
            {
                record.Principal = principal;
                record.Interest = interest;
                Update(record);
            }
        }

        public CacheAcfReceive Get(int projectId, DateTime paymentDay)
        {
            var records = Select<ABSMgrConn.TableCacheAcfReceive>("project_id", projectId);
            var result = records.Select(x => new CacheAcfReceive(x))
                .Where(x => x.PaymentDay == paymentDay).ToList();
            if (result.Count == 1)
            {
                return result.First();
            }

            if (result.Count > 2)
            {
                CommUtils.Assert(false, "找到了多条CacheAcfReceive");
            }

            return null;
        }
    }
}
