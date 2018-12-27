using ChineseAbs.ABSManagement.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Manager
{
    public class OverrideSingleAssetInterestManager
        : BaseModelManager<OverrideSingleAssetInterest, ABSMgrConn.TableOverrideSingleAssetInterest>
    {
        public OverrideSingleAssetInterestManager()
        {
            m_defaultTableName = "dbo.OverrideSingleAssetInterest";
            m_defaultPrimaryKey = "override_single_asset_interest_id";
            m_defalutFieldPrefix = "override_single_asset_interest_";
        }

        public List<OverrideSingleAssetInterest> GetByProject(int projectId)
        {
            var records = this.Select<ABSMgrConn.TableOverrideSingleAssetInterest>("project_id", projectId);
            var osaRecords = records.Select(x => new OverrideSingleAssetInterest(x))
                .OrderBy(x => x.PaymentDate).ThenBy(x => x.PaymentDate).ThenBy(x => x.CreateTime).ToList();
            return osaRecords;
        }

        public List<OverrideSingleAssetInterest> GetByPaymentDay(int projectId, DateTime paymentDate)
        {
            var osaRecords = GetByProject(projectId)
                .Where(x => x.PaymentDate == paymentDate)
                .OrderBy(x => x.AssetId).ThenBy(x => x.CreateTime).ToList();
            return osaRecords;
        }

        public OverrideSingleAssetInterest GetLatestByAsset(int projectId, DateTime paymentDay, int assetId)
        {
            var osaRecords = GetByPaymentDay(projectId, paymentDay);
            osaRecords = osaRecords.Where(x => x.AssetId == assetId)
                .OrderBy(x => x.CreateTime).ToList();

            if (osaRecords.Count > 0)
            {
                return osaRecords.First();
            }

            return null;
        }

        public void RemoveAll(int projectId, DateTime paymentDate)
        {
            var records = GetByPaymentDay(projectId, paymentDate);
            this.Delete(records);
        }
    }
}
