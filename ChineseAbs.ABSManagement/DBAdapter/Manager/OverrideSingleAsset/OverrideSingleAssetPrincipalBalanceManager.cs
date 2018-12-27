using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager
{
    public class OverrideSingleAssetPrincipalBalanceManager
        : BaseModelManager<OverrideSingleAssetPrincipalBalance, ABSMgrConn.TableOverrideSingleAssetPrincipalBalance>
    {
        public OverrideSingleAssetPrincipalBalanceManager()
        {
            m_defaultTableName = "dbo.OverrideSingleAssetPrincipalBalance";
            m_defaultPrimaryKey = "override_single_asset_principal_balance_id";
            m_defalutFieldPrefix = "override_single_asset_principal_balance_";
        }

        public List<OverrideSingleAssetPrincipalBalance> GetByProject(int projectId)
        {
            var records = this.Select<ABSMgrConn.TableOverrideSingleAssetPrincipalBalance>("project_id", projectId);
            var osaRecords = records.Select(x => new OverrideSingleAssetPrincipalBalance(x))
                .OrderBy(x => x.PaymentDate).ThenBy(x => x.AssetId).ThenBy(x => x.CreateTime).ToList();
            return osaRecords;
        }

        public List<OverrideSingleAssetPrincipalBalance> GetByPaymentDay(int projectId, DateTime paymentDate)
        {
            var osaRecords = GetByProject(projectId)
                .Where(x => x.PaymentDate == paymentDate)
                .OrderBy(x => x.AssetId).ThenBy(x => x.CreateTime).ToList();
            return osaRecords;
        }

        public void RemoveAll(int projectId, DateTime paymentDate)
        {
            var records = GetByPaymentDay(projectId, paymentDate);
            this.Delete(records);
        }

        public OverrideSingleAssetPrincipalBalance GetLatestByAsset(int projectId, DateTime paymentDay, int assetId)
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
    }
}
