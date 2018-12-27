using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class PaymentHistoryManager : BaseManager
    {
        public PaymentHistoryManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public AssetCashflowHistory NewAssetCashflowHistory(AssetCashflowHistory assetCashflowHistory)
        {
            var tableObject = assetCashflowHistory.GetTableObject();
            var assetCashflowHistoryId = m_db.Insert("AssetCashflowHistory", "asset_cashflow_history_id", true, tableObject);
            assetCashflowHistory.AssetCashflowHistoryId = (int)assetCashflowHistoryId;
            return assetCashflowHistory;
        }

        public AssetPaymentInfo GetCurrentAssetPaymentInfo(int projectId, DateTime paymentDate, int assetId)
        {
            var items = m_db.Query<ABSMgrConn.TableAssetCashflowHistory>(
                "SELECT * FROM dbo.AssetCashflowHistory WHERE dbo.AssetCashflowHistory.project_id = @0 "
                + " AND dbo.AssetCashflowHistory.payment_date = @1 "
                + " AND dbo.AssetCashflowHistory.asset_id = @2 "
                + " ORDER BY TIME_STAMP", projectId, paymentDate, assetId);

            var histories = items.ToList().ConvertAll(x => new AssetCashflowHistory(x));

            AssetPaymentInfo assetPaymentInfo = new AssetPaymentInfo();
            assetPaymentInfo.AssetId = assetId;
            assetPaymentInfo.ProjectId = projectId;
            assetPaymentInfo.PaymentDate = paymentDate;
            foreach (var history in histories)
            {
                if (history.FieldType == AssetCashflowFieldType.Pricipal)
                {
                    assetPaymentInfo.Principal = Decimal.Parse(history.FieldValue);
                    assetPaymentInfo.AssetName = history.AssetName;
                }
                else if (history.FieldType == AssetCashflowFieldType.Interest)
                {
                    assetPaymentInfo.Interest = Decimal.Parse(history.FieldValue);
                    assetPaymentInfo.AssetName = history.AssetName;
                }
            }

            return assetPaymentInfo;
        }

        /// <summary>
        /// 获取最新的资产端数据
        /// </summary>
        /// <param name="projectId">产品Id</param>
        /// <param name="paymentDate">偿付日</param>
        /// <returns>最新的资产端数据</returns>
        public List<AssetCashflowHistory> GetCurrentAssetCashflowHistory(int projectId, DateTime paymentDate)
        {
            var originHistories = GetAssetCashflowHistory(projectId, paymentDate, AssetCashflowHandleType.SystemInit);
            var userEditHistories = GetAssetCashflowHistory(projectId, paymentDate, AssetCashflowHandleType.UserEdit);

            Func<AssetCashflowHistory, AssetCashflowHistory, bool> isNewValue = (l, r) => {
                return l.ProjectId == r.ProjectId && l.AssetId == r.AssetId && l.PaymentDate == r.PaymentDate
                    && l.FieldType == r.FieldType && l.TimeStamp < r.TimeStamp;
            };

            for (int i = 0; i < originHistories.Count; ++i)
            {
                var originHistory = originHistories[i];
                foreach (var userEditHistory in userEditHistories)
                {
                    if (isNewValue(originHistory, userEditHistory))
                    {
                        originHistory = userEditHistory;
                        originHistories[i] = userEditHistory;
                    }
                }
            }

            return originHistories;
        }

        public List<AssetCashflowHistory> GetAssetCashflowHistory(int projectId, DateTime paymentDate, AssetCashflowHandleType handleType, int assetId)
        {
            var items = m_db.Query<ABSMgrConn.TableAssetCashflowHistory>(
                "SELECT * FROM dbo.AssetCashflowHistory WHERE dbo.AssetCashflowHistory.project_id = @0 "
                + " AND dbo.AssetCashflowHistory.payment_date = @1 "
                + " AND dbo.AssetCashflowHistory.handle_type = @2 "
                + " AND dbo.AssetCashflowHistory.asset_id = @3 "
                + " ORDER BY TIME_STAMP DESC, dbo.AssetCashflowHistory.asset_id", projectId, paymentDate, (int)handleType, assetId);

            return items.ToList().ConvertAll(x => new AssetCashflowHistory(x));
        }

        public List<AssetCashflowHistory> GetAssetCashflowHistory(int projectId, DateTime paymentDate, AssetCashflowHandleType handleType)
        {
            var items = m_db.Query<ABSMgrConn.TableAssetCashflowHistory>(
                "SELECT * FROM dbo.AssetCashflowHistory WHERE dbo.AssetCashflowHistory.project_id = @0 "
                + " AND dbo.AssetCashflowHistory.payment_date = @1 "
                + " AND dbo.AssetCashflowHistory.handle_type = @2 "
                + " ORDER BY TIME_STAMP DESC, dbo.AssetCashflowHistory.asset_id", projectId, paymentDate, (int)handleType);

            return items.ToList().ConvertAll(x => new AssetCashflowHistory(x));
        }

        public List<AssetCashflowHistory> GetAssetCashflowHistory(int projectId, DateTime paymentDate,
            AssetCashflowHandleType handleType, AssetCashflowFieldType fieldType, int assetId)
        {
            var items = m_db.Query<ABSMgrConn.TableAssetCashflowHistory>(
                "SELECT * FROM dbo.AssetCashflowHistory WHERE dbo.AssetCashflowHistory.project_id = @0 "
                + " AND dbo.AssetCashflowHistory.payment_date = @1 "
                + " AND dbo.AssetCashflowHistory.handle_type = @2 "
                + " AND dbo.AssetCashflowHistory.field_type = @3 "
                + " AND dbo.AssetCashflowHistory.asset_id = @4 "
                + " ORDER BY TIME_STAMP DESC, dbo.AssetCashflowHistory.asset_id",
                projectId, paymentDate, (int)handleType, (int)fieldType, assetId);

            return items.ToList().ConvertAll(x => new AssetCashflowHistory(x));
        }

        public int UpdateAssetCashflowHistory(AssetCashflowHistory history)
        {
            var historyTable = history.GetTableObject();
            return m_db.Update("AssetCashflowHistory", "asset_cashflow_history_id", historyTable, history.AssetCashflowHistoryId);
        }

        public AssetPrepaymentHistory NewAssetPrepaymentHistory(AssetPrepaymentHistory assetPrepaymentHistory)
        {
            var tableObject = assetPrepaymentHistory.GetTableObject();
            var id = m_db.Insert("AssetPrepaymentHistory", "asset_prepayment_history_id", true, tableObject);
            assetPrepaymentHistory.AssetPrepaymentHistoryId = (int)id;
            return assetPrepaymentHistory;
        }

        public List<AssetPrepaymentHistory> GetAssetPrepaymentHistory(int projectId, int datasetId, int assetId)
        {
            var items = m_db.Query<ABSMgrConn.TableAssetPrepaymentHistory>(
                "SELECT * FROM dbo.AssetPrepaymentHistory WHERE dbo.AssetPrepaymentHistory.project_id = @0 "
                + " AND dbo.AssetPrepaymentHistory.dataset_id = @1 "
                + " AND dbo.AssetPrepaymentHistory.asset_id = @2 "
                + " ORDER BY TIME_STAMP DESC",
                projectId, datasetId, assetId);

            return items.ToList().ConvertAll(x => new AssetPrepaymentHistory(x));
        }

        private const string m_orderBy = " ORDER BY dbo.Tasks.end_time, dbo.Tasks.project_id desc, dbo.Tasks.task_id";
    }
}
