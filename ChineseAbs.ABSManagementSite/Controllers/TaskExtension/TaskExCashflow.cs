using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.LogicModels.DealModel;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DealModel;
using ChineseAbs.ABSManagement.Object;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.CalcService.Data.NancyData;
using CNABS.Mgr.Deal.Model;
using CNABS.Mgr.Deal.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public class TaskExCashflow : TaskExBase
    {
        public TaskExCashflow(string userName, string shortCode)
            : base(userName, shortCode)
        {
            this.OnFinishing += TaskExCashflow_OnFinishing;
        }

        HandleResult TaskExCashflow_OnFinishing()
        {
            if (Task.TaskExtension.TaskExtensionStatus != TaskExtensionStatus.Finished)
            {
                if (Task.TaskExtension.TaskExtensionStatus == TaskExtensionStatus.NotMatch)
                {
                    return new HandleResult(EventResult.Cancel, "数据不匹配，请重新核对。");
                }
                return new HandleResult(EventResult.Cancel, "请先确认核对。");
            }

            return new HandleResult();
        }

        /// <summary>
        /// 更新数据库中的参数到模型文件中
        /// </summary>
        private void UpdateVariablesFromTaskDBToModel(DatasetLogicModel dataset, CashflowViewModel dbViewModel)
        {
            if (dbViewModel == null || dataset.Instance == null)
            {
                return;
            }

            var dbVarList = dbViewModel.OverridableVariables;

            //更新FutureVariables.csv
            foreach (var keyVal in dbVarList)
            {
                dataset.Variables.UpdateVariableValue(keyVal.EnName, dataset.DatasetSchedule.PaymentDate, keyVal.Value.ToString());
            }

            //更新CurrentVariables.csv
            //设置 OtherIncome = 当期的OtherIncomeMax + 前一期模型的OtherIncome
            var otherIncomeMaxKeyVal = dbVarList.SingleOrDefault(x => x.EnName.Equals("OtherIncomeMax", StringComparison.CurrentCultureIgnoreCase));
            if (otherIncomeMaxKeyVal != null)
            {
                var key = "OtherIncome";
                var otherIncomeValue = otherIncomeMaxKeyVal.Value;

                if (dataset.Previous != null && dataset.Previous.Instance != null)
                {

                    string prevOtherIncomeVal = dataset.Previous.Variables.GetVariable(key);
                    double prevOtherIncome = 0;
                    if (double.TryParse(prevOtherIncomeVal, out prevOtherIncome))
                    {
                        otherIncomeValue += prevOtherIncome;
                    }
                }

                dataset.Variables.UpdateVariableValue(key, dataset.Variables.Asofdate, "其他收益");

                dataset.Variables.Save();
            }
        }

        public override object GetEntity()
        {
            var projectLogicModel = new ProjectLogicModel(m_userName, Task.ProjectId);
            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(Task.ShortCode);
            var paymentDate = Task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }

            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));
            paymentDate = datasetSchedule.PaymentDate;

            return new CashflowViewModel { PaymentDay = Toolkit.DateToString(paymentDate) };
        }

        /// <summary>
        /// 从文件中选取可修改变量
        /// </summary>
        private List<CashflowVariablesData> SelectOverridableVariables(DatasetLogicModel dataset)
        {
            var varList = new List<CashflowVariablesData>();
            var records = dataset.Variables.FutureVariables.Where(x => m_overridableVariableNames.Contains(x.Name));
            foreach (var record in records)
            {
                double value;
                if (!double.TryParse(record.Items[dataset.DatasetSchedule.PaymentDate], out value))
                {
                    value = 0;
                }

                varList.Add(new CashflowVariablesData
                {
                    EnName = record.Name,
                    CnName = Translate(record.Name, record.Description),
                    Value = value,
                });
            }
            return varList;
        }

        private readonly string[] m_overridableVariableNames = { "OtherIncomeMax", "Expense", "AuditFee",
                                                                   "RatingFee", "TrusteeFee", "ReserveFee",
                                                                   "PreReserveFee", "中证手续费储备金额" };

        private string Translate(string enName, string description)
        {
            //使用Description或系统翻译EnName作为UI上的中文名
            bool isInvalidDescription = string.IsNullOrEmpty(description) || description.StartsWith("?");
            var cnName = isInvalidDescription ? Toolkit.TranslateEn2CnFutureVariable(enName) : description;
            return cnName;
        }

        public void SaveNoteData()
        {
            m_dbAdapter.Task.CheckPrevIsFinished(Task);
            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(Task.ShortCode);
            var paymentDate = Task.EndTime.Value;

            var project = new ProjectLogicModel(m_userName, Task.ProjectId);
            var datasetSchedule = project.DealSchedule.GetByPaymentDay(paymentDate);

            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }
            else
            {
                if (datasetSchedule != null)
                {
                    paymentDate = datasetSchedule.PaymentDate;
                }
            }

            var dataset = datasetSchedule.Dataset;
            var acfTable = dataset.DealModel.AssetCashflowDt;
            var assetViewModel = Toolkit.GetAssetCashflow(acfTable, paymentDate);

            //判断本金与利息是否被覆盖
            var assetCashflowVariable = m_dbAdapter.AssetCashflowVariable.GetByProjectIdPaymentDay(project.Instance.ProjectId, paymentDate);
            var cashflowViewModel = new CashflowViewModel();
            cashflowViewModel.PredictInterestCollection = double.Parse(assetViewModel.TotalCurrentInterestCollection.ToString("n2"));
            cashflowViewModel.PredictPricipalCollection = double.Parse(assetViewModel.TotalCurrentPrinCollection.ToString("n2"));
            cashflowViewModel.OverridableVariables = SelectOverridableVariables(datasetSchedule.Dataset);

            var cfTable = ABSDealUtils.GetCashflowDt(datasetSchedule, assetCashflowVariable,
                cashflowViewModel.PredictInterestCollection, cashflowViewModel.PredictPricipalCollection,
                out double currInterest, out double currPrincipal);

            var notes = m_dbAdapter.Dataset.GetNotes(project.Instance.ProjectId);

            var currentDatasetColumnIndex = 2;
            for (int i = 0; i < cfTable.Columns.Count; ++i )
            {
                DateTime temp;
                if (DateTime.TryParse(cfTable.Columns[i].ColumnName, out temp)
                    && dataset.DatasetSchedule.PaymentDate == temp)
                {
                    currentDatasetColumnIndex = i;
                    break;
                }
            }

            foreach (var n in notes)
            {
                var nd = m_dbAdapter.Dataset.GetNoteData((int)n.NoteId, dataset.Instance.DatasetId);

                var prinKey = n.ShortName + ".Principal";
                var prinRow = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(prinKey) && r[1].ToString().Contains("Received"));
                CommUtils.AssertNotNull(prinRow, $"在证券端现金流表中，找不到[{prinKey}]");
                var prin = prinRow[currentDatasetColumnIndex].ToString();
                nd.PrincipalPaid = (decimal)((prin == string.Empty || prin == "-") ? 0.0 : double.Parse(prin));

                var interestKey = n.ShortName + ".Interest";
                var interestRow = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(interestKey) && r[1].ToString().Contains("Received"));
                CommUtils.AssertNotNull(interestRow, $"在证券端现金流表中，找不到[{interestKey}]");
                var interest = interestRow[currentDatasetColumnIndex].ToString();
                nd.InterestPaid = (decimal)((interest == string.Empty || interest == "-") ? 0.0 : double.Parse(interest));

                var endKey = n.ShortName + ".Beginning";
                var endRow = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(endKey) && r[1].ToString().Contains("Outstanding"));
                CommUtils.AssertNotNull(endRow, $"在证券端现金流表中，找不到[{endKey}]");
                var end = endRow[currentDatasetColumnIndex + 1].ToString();
                nd.EndingBalance = (decimal)((end == string.Empty || end == "-") ? 0.0 : double.Parse(end));

                m_dbAdapter.Dataset.UpdateNoteData(nd);
            }

            Task.TaskExtension.TaskExtensionStatus = TaskExtensionStatus.Finished;
            Task.TaskExtension.TaskExtensionHandler = m_userName;
            Task.TaskExtension.TaskExtensionHandleTime = DateTime.Now;
            m_dbAdapter.Task.SaveTaskExtension(Task.TaskExtension);

            new TaskLogicModel(project, Task).Start();
        }
    }
}