using ChineseAbs.ABSManagement.AssetEvent;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.Logic.Object;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.CalcService.Data.NancyData.Cashflows;

namespace ChineseAbs.ABSManagement
{
    public class AssetModifier
    {
        public AssetModifier(string userName)
        {
            m_userName = userName;
        }

        private bool m_isInitialized = false;
        private DateTime m_previousPaymentDay;
        private DateTime m_paymentDay;
        private DateTime m_asOfDay;
        private DateTime? m_nextPaymentDay;
        private DateTime? m_nextAsOfDay;
        private DealSchedule m_dealSchedule;

        public void Load(int projectId, DateTime asOfDate)
        {
            var project = m_dbAdapter.Project.GetProjectById(projectId);
            var dealSchedule = NancyUtils.GetDealSchedule(projectId, asOfDate.ToString("yyyyMMdd"));
            m_dealSchedule = dealSchedule;

            var errorMsg = "Search payment date/as of date failed, projectGuid=[" + project.ProjectGuid + "] asOfDate=[" + asOfDate.ToString() + "]";
            CommUtils.Assert(dealSchedule.DeterminationDates != null && dealSchedule.DeterminationDates.Length > 0, errorMsg);
            CommUtils.Assert(dealSchedule.PaymentDates != null && dealSchedule.PaymentDates.Length > 0, errorMsg);
            CommUtils.Assert(dealSchedule.DeterminationDates.Length == dealSchedule.PaymentDates.Length, errorMsg);
            CommUtils.Assert(asOfDate < dealSchedule.LegalMaturity, errorMsg);

            var asOfDateList = dealSchedule.DeterminationDates.ToList();
            asOfDateList.Insert(0, dealSchedule.FirstCollectionPeriodStartDate);
            asOfDateList.RemoveAt(asOfDateList.Count - 1);

            var paymentDateList = dealSchedule.PaymentDates.ToList();
            var projectLogicModel = new ProjectLogicModel(m_userName, project);

            var findPaymentDates = projectLogicModel.DealSchedule.DurationPeriods
                .Where(x => x.Dataset != null && x.Dataset.Instance != null
                    && DateUtils.ParseDigitDate(x.Dataset.Instance.AsOfDate) == asOfDate)
                .Select(x => x.Dataset.Instance.PaymentDate);
            CommUtils.AssertEquals(findPaymentDates.Count(), 1, "查找PaymentDay失败，AsOfDate={0}", asOfDate);

            m_paymentDay = findPaymentDates.Single().Value;
            var index = paymentDateList.IndexOf(m_paymentDay);
            CommUtils.Assert(index >= 0, errorMsg);

            m_asOfDay = asOfDate;
            if (index == 0)
            {
                m_previousPaymentDay = paymentDateList[0];
            }
            else
            {
                m_previousPaymentDay = paymentDateList[index - 1];
            }

            if (index == paymentDateList.Count - 1)
            {
                m_nextAsOfDay = null;
                m_nextPaymentDay = null;
            }
            else
            {
                m_nextAsOfDay = asOfDateList[index + 1];
                m_nextPaymentDay = paymentDateList[index + 1];
            }

            m_isInitialized = true;
        }

        public void GenerateNextDataset(int projectId)
        {
            CommUtils.Assert(m_isInitialized, "AssetModifier hasn't been initialized!");

            var project = m_dbAdapter.Project.GetProjectById(projectId);
            var datasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(project, m_asOfDay);
            CommUtils.Assert(Directory.Exists(datasetFolder), "Load dataset [" + project.ProjectGuid + "][" + m_asOfDay + "] failed!");

            if (!m_nextAsOfDay.HasValue)
            {
                return;
            }

            //Create folder for next dataset if not exists.
            var nextDatasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(project, m_nextAsOfDay.Value);
            if (!Directory.Exists(nextDatasetFolder))
            {
                try
                {
                    Directory.CreateDirectory(nextDatasetFolder);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Create dataset [" + project.ProjectGuid + "][" + m_asOfDay + "] folder failed! Exception: " + e.Message);
                }
            }

            //Copy and modify collertal.csv
            var collateralPath = Path.Combine(datasetFolder, "collateral.csv");
            var nextCollateralPath = Path.Combine(nextDatasetFolder, "collateral.csv");
            FileUtils.Copy(collateralPath, nextCollateralPath);

            var amortizationPath = Path.Combine(datasetFolder, "AmortizationSchedule.csv");
            var nextAmortizationPath = Path.Combine(nextDatasetFolder, "AmortizationSchedule.csv");
            FileUtils.Copy(amortizationPath, nextAmortizationPath, false);

            var promisedCashflowPath = Path.Combine(datasetFolder, "PromisedCashflow.csv");
            var nextPromisedCashflowPath = Path.Combine(nextDatasetFolder, "PromisedCashflow.csv");
            FileUtils.Copy(promisedCashflowPath, nextPromisedCashflowPath, false);

            var reinvestmentPath = Path.Combine(datasetFolder, "Reinvestment.csv");
            var nextReinvestmentPath = Path.Combine(nextDatasetFolder, "Reinvestment.csv");
            FileUtils.Copy(reinvestmentPath, nextReinvestmentPath, false);

            var currentCombileVaiablesPath = Path.Combine(datasetFolder, "CombinedVariables.csv");
            var nextCombileVaiablesPath = Path.Combine(nextDatasetFolder, "CombinedVariables.csv");

            FileUtils.Copy(currentCombileVaiablesPath, nextCombileVaiablesPath, false);

            var currentVariablesPath = Path.Combine(datasetFolder, "CurrentVariables.csv");
            var nextCurrentVariablesPath = Path.Combine(nextDatasetFolder, "CurrentVariables.csv");
            FileUtils.Copy(currentVariablesPath, nextCurrentVariablesPath, false);

            var futureVariablesPath = Path.Combine(datasetFolder, "FutureVariables.csv");
            var nextFutureVariablesPath = Path.Combine(nextDatasetFolder, "FutureVariables.csv");
            FileUtils.Copy(futureVariablesPath, nextFutureVariablesPath, false);

            var pastVariablesPath = Path.Combine(datasetFolder, "PastVariables.csv");
            var nextPastVariablesPath = Path.Combine(nextDatasetFolder, "PastVariables.csv");
            FileUtils.Copy(pastVariablesPath, nextPastVariablesPath, false);
            


            BasicAnalyticsData basicAnalyticsData = NancyUtils.GetBasicAnalyticsData(projectId, null, m_asOfDay);

            //AssetId, 剩余本金
            var dictPrincipalBalance = new Dictionary<int, double>();
            foreach (var assetCashFlow in basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems)
            {
                if (assetCashFlow.PaymentDate == m_paymentDay)
                {
                    dictPrincipalBalance[assetCashFlow.AssetId] = assetCashFlow.Performing;
                }
            }



            var collateralCsv = new CollateralCsv();
            collateralCsv.Load(nextCollateralPath);
            foreach (var record in collateralCsv)
            {
                var assetId = record.AssetId;
                //更新封包日日期
                collateralCsv.UpdateCellValue(assetId, "AsOfDate",
                    m_nextAsOfDay.Value.ToString("MM/dd/yyyy"));

                //更新剩余本金
                collateralCsv.UpdateCellValue(assetId, "PrincipalBalance",
                    dictPrincipalBalance.ContainsKey(assetId) ? dictPrincipalBalance[assetId].ToString() : "0");

                if (collateralCsv.ContainsColumn("AccrueFromAsOfDate"))
                {
                    collateralCsv.UpdateCellValue(assetId, "AccrueFromAsOfDate", "FALSE");
                }
            }
            collateralCsv.Save(nextCollateralPath);




            ProcessVaiables(m_dbAdapter.Dataset.GetYmlFolder(project), basicAnalyticsData);

            if (m_previousPaymentDay != m_paymentDay && File.Exists(nextPromisedCashflowPath))
            {
                var p = new PromisedCashflowCsv(nextPromisedCashflowPath);
                p.Load();
                p.Records = p.Records.Where(o => o.PaymentDate > m_previousPaymentDay).ToList();
                p.Save();
            }
        }

        private void ProcessVaiables(string projectFolder, BasicAnalyticsData basicAnalyticsData)
        {
            double sumPrincipal = basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                .Where(x => x.PaymentDate == m_paymentDay).Sum(x => x.Principal);
            double sumInterest = basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                .Where(x => x.PaymentDate == m_paymentDay).Sum(x => x.Interest);
            NancyBasicCashflowItem basicCashflowItem = basicAnalyticsData.BasicCashflow.BasicCashflowItems.Where(x => x.PaymentDate == m_previousPaymentDay).Single();
            
            VariablesHelper helper = new VariablesHelper(Path.Combine(projectFolder, m_nextAsOfDay.Value.ToString("yyyyMMdd")));
            helper.Load();
            helper.TranferToAsofdate(m_nextAsOfDay.Value);
            helper.Save(Path.Combine(projectFolder, m_nextAsOfDay.Value.ToString("yyyyMMdd")));
            
            //{
                
            //    var nextCurrentVariablesPath = Path.Combine(nextDatasetFolder, "CurrentVariables.csv");

            //    var nextFutureVariablesPath = Path.Combine(nextDatasetFolder, "FutureVariables.csv");

            //    var nextPastVariablesPath = Path.Combine(nextDatasetFolder, "PastVariables.csv");

            //    currentVariablesCsv = new VariablesCsv();
            //    currentVariablesCsv.Load(nextCurrentVariablesPath);
            //    //current variables中的 日期指的是前一个偿付日
            //    currentVariablesCsv.UpdateColumnDate(m_previousPaymentDay, m_paymentDay);

            //    futureVariablesCsv = new VariablesCsv();
            //    futureVariablesCsv.Load(nextFutureVariablesPath);

            //    if (m_paymentDay != m_previousPaymentDay)
            //    {
            //        futureVariablesCsv.RemoveColumn(m_previousPaymentDay);
            //    }

            //    //在PastVariables.csv中填入 支付日
            //    var pastVariablesCsv = new VariablesCsv();
            //    pastVariablesCsv.Load(nextPastVariablesPath);
            //    m_dealSchedule.PaymentDates.Where(x => x < m_paymentDay).ToList().ForEach(x =>
            //    {
            //        if (!pastVariablesCsv.DateColumns.Contains(x))
            //        {
            //            pastVariablesCsv.DateColumns.Add(x);
            //        }
            //    });
            //    pastVariablesCsv.Save(nextPastVariablesPath);
            //}

            //资产覆盖现金流的情况下，资产端的本金和利息一般是0
            //这时如果将 Collateral.InterestCollection 和 Collateral.PrincipalCollection 填 0，会导致证券端偿付金额为 0，不填即可
            if (MathUtils.MoneyNE(sumInterest, 0) && MathUtils.MoneyNE(sumPrincipal, 0))
            {
                helper.UpdateVariableValue("Collateral.InterestCollection", m_asOfDay, sumInterest.ToString());
                helper.UpdateVariableValue("Collateral.PrincipalCollection", m_asOfDay, sumPrincipal.ToString());
            }

            string reserveAmount = "";
            //第一期生成第二期数据时，由于CurrentVariables.csv中支付日是第一个支付日
            //Reserve账户会重新计算，所以不需要在ReserveAmount中填值
            if (m_paymentDay != m_dealSchedule.PaymentDates.First())
            {
                reserveAmount = basicCashflowItem.ReserveAmountEndBalance.ToString();
            }
            helper.UpdateVariableValue("ReserveAmount", m_asOfDay, reserveAmount);
            


            //第一期要特殊处理（生成第N期PaymentDate模型时，填写参数的ColumnName从第N-1期开始）
            if (m_paymentDay != m_previousPaymentDay)
            {
                foreach (var note in basicCashflowItem.Notes)
                {
                    var principal = (note.BeginningPrincipalOutstanding - note.PrincipalReceived).ToString();
                    helper.UpdateVariableValue(note.NoteName, m_paymentDay, principal);
                }
            }

            helper.Save(Path.Combine(projectFolder, m_nextAsOfDay.Value.ToString("yyyyMMdd")));
        }

        public bool Execute(IEnumerable<AssetEventBase> assetEvents)
        {
            return assetEvents.All(Execute);
        }

        public bool Execute(AssetEventBase assetEvent)
        {
            return assetEvent.PrevExecute() && assetEvent.Execute();
        }

        public bool Revert(IEnumerable<AssetEventBase> assetEvents)
        {
            return assetEvents.All(Revert);
        }

        public bool Revert(AssetEventBase assetEvent)
        {
            return assetEvent.PrevRevert() && assetEvent.Revert();
        }

        protected string m_userName;

        protected DBAdapter m_dbAdapter
        {
            get { return new DBAdapter(); }
        }
    }
}
