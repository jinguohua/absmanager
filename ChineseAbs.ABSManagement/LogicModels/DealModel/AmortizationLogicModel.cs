using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class AmortizationRecord
    {
        public int AssetId { get; set; }

        public double Money { get; set; }

        public DateTime Date { get; set; }
    }

    public class AmortizationLogicModel : BaseLogicModel
    {
        public AmortizationLogicModel(AssetLogicModel assetLogicModel)
            : base(assetLogicModel.ProjectLogicModel)
        {
            m_assetLogicModel = assetLogicModel;
        }

        public List<AmortizationRecord> AmortizationRecords
        {
            get
            {
                if (m_amortizationRecords == null)
                {
                    m_amortizationRecords = LoadAmortizationRecords();
                }

                return m_amortizationRecords;
            }
        }

        private List<AmortizationRecord> m_amortizationRecords;

        public List<AmortizationRecord> LoadAmortizationRecords()
        {
            List<AmortizationRecord> amortizationRecords = null;
            var asset = m_assetLogicModel;
            if (asset.AmortizationType == AmortizationType.UserDefined)
            {
                var records = asset.Dataset.AmortizationSchedule.SelectByAsset(asset.AssetId);
                amortizationRecords = records.Select(x => new AmortizationRecord { 
                    AssetId = x.AssetId,
                    Date = x.ReductionDate,
                    Money = x.ReductionAmount,
                }).ToList();
            }
            else if (asset.IsEqualPmtOrEqualPrin)
            {
                var basicAnalyticsData = NancyUtils.GetBasicAnalyticsData(ProjectLogicModel.Instance.ProjectId, null, asset.Dataset.Instance.AsOfDate);
                var records = basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems.SelectByAsset(asset.AssetId);
                amortizationRecords = records.Select(x => new AmortizationRecord {
                    AssetId = x.AssetId,
                    Date = x.PaymentDate,
                    Money = x.Principal
                }).ToList();
            }
            else if (asset.AmortizationType == AmortizationType.SingleAmortization)
            {
                var records = asset.Dataset.AmortizationSchedule.SelectByAsset(asset.AssetId);
                var sumPrepayMoney = records.Sum(x => x.ReductionAmount);
                amortizationRecords = new List<AmortizationRecord> {
                    new AmortizationRecord {
                        AssetId = asset.AssetId,
                        Date = asset.SecurityData.LegalMaturityDate,
                        Money = asset.SecurityData.PrincipalBalance,
                    }
                };
            }

            CommUtils.AssertNotNull(amortizationRecords, "不支持的 AmortizationType");
            amortizationRecords = amortizationRecords.Where(x => MathUtils.MoneyNE(x.Money, 0))
                .OrderBy(x => x.Date).ToList();
            return amortizationRecords;
        }

        private AssetLogicModel m_assetLogicModel;
    }
}
