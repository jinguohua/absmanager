
using ChineseAbs.ABSManagement.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Manager
{
    public class CacheAcfAssetReceiveManager
        : BaseModelManager<CacheAcfAssetReceive, ABSMgrConn.TableCacheAcfAssetReceive>
    {
        public CacheAcfAssetReceiveManager()
        {
            m_defaultTableName = "dbo.CacheAcfAssetReceive";
            m_defaultPrimaryKey = "cache_acf_asset_receive_id";
            m_defalutFieldPrefix = "cache_acf_asset_receive_";
        }

        public void Save(List<CacheAcfAssetReceive> records)
        {
            if (records.Count == 0)
            {
                return;
            }

            var cachedRecords = Get(records.First().ProjectId, records.First().PaymentDay);
            Delete(cachedRecords);

            New(records);
        }

        public List<CacheAcfAssetReceive> Get(int projectId, DateTime paymentDay)
        {
            var records = Select<ABSMgrConn.TableCacheAcfAssetReceive>("project_id", projectId);
            return records.Select(x => new CacheAcfAssetReceive(x))
                .Where(x => x.PaymentDay == paymentDay).ToList();
        }
    }
}
