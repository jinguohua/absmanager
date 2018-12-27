using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.LogicModels.OverrideSingleAsset;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Object;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.CalcService.Data.NancyData.Cashflows;
using CNABS.Mgr.Deal.Model;
using CNABS.Mgr.Deal.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public class TaskExAssetCashflow : TaskExBase
    {
        public TaskExAssetCashflow(string userName, string shortCode)
            : base(userName, shortCode)
        {
            this.OnFinishing += TaskExAssetCashflow_OnFinishing;
        }

        HandleResult TaskExAssetCashflow_OnFinishing()
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
            paymentDate = datasetSchedule.PaymentDate;
            CommUtils.Assert(datasetSchedule.Dataset.HasDealModel, "第{0}期模型未生成", Toolkit.DateToString(paymentDate));

            return paymentDate;
        }

        public MemoryStream GetACFTableFileByProject(string projectGuid, DateTime paymentDate)
        {
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            return GetAssetCashflowFile(project, paymentDate);
        }

        public MemoryStream GetAssetCashflowFile(Project project, DateTime paymentDay)
        {
            var projectLogicModel = new ProjectLogicModel(m_userName, project);
            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDay);
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));
            paymentDay = datasetSchedule.PaymentDate;

            var dealModel = datasetSchedule.Dataset.DealModel;

            var absDeal = new ABSDeal(dealModel.YmlFolder, dealModel.DsFolder);
            var acfResult = absDeal.Result.AcfResult;
            acfResult.MergeOsa(dealModel.OverrideSingleAsset);
            acfResult.ReCalcSum();
            var dt = acfResult.ExtractAssetCashflowDataTable(absDeal, paymentDay);
            
            var temproaryFolder = CommUtils.CreateTemporaryFolder(m_userName);
            var filePath = Path.Combine(temproaryFolder, "AssetCashflowTable.csv");
            ExcelUtils.WriteCsv(dt, filePath);

            var buffer = File.ReadAllBytes(filePath);
            CommUtils.DeleteFolderAync(temproaryFolder);
            return new MemoryStream(buffer);
        }

        public void ConfirmAssetCashflow()
        {
            m_dbAdapter.Task.CheckPrevIsFinished(Task);

            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(Task.ShortCode);
            var paymentDate = Task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }

            var taskExtension = Task.TaskExtension;

            taskExtension.TaskExtensionStatus = TaskExtensionStatus.Finished;
            taskExtension.TaskExtensionHandler = m_userName;
            taskExtension.TaskExtensionHandleTime = DateTime.Now;
            m_dbAdapter.Task.SaveTaskExtension(taskExtension);

            var logicModel = new ProjectLogicModel(m_userName, Task.ProjectId);
            new TaskLogicModel(logicModel, Task).Start();
        }
        
        public object GetACFTableByProject(AssetCashflowStatisticInfo viewModel, string projectGuid, DateTime paymentDate)
        {
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            return GetAssetCashflowTable(viewModel, project, paymentDate);
        }

        public object GetAssetCashflowTable(AssetCashflowStatisticInfo viewModel, Project project, DateTime paymentDate)
        {
            var acfTable = new DataTable();
            var dictionaryDates = new Dictionary<DateTime, DateTime>();

            try
            {
                var projectLogicModel = new ProjectLogicModel(m_userName, project);
                var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
                CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));
                paymentDate = datasetSchedule.PaymentDate;

                var dealModel = datasetSchedule.Dataset.DealModel;

                var absDeal = new ABSDeal(dealModel.YmlFolder, dealModel.DsFolder);
                var acfResult = absDeal.Result.AcfResult;
                acfResult.MergeOsa(dealModel.OverrideSingleAsset);
                acfResult.ReCalcSum();
                return acfResult.ExtractAssetCashflowTable(absDeal, paymentDate);
            }
            catch (ApplicationException ae)
            {
                var errorResult = new
                {
                    isError = true,
                    errorMessage = ae.Message
                };
                return errorResult;
            }
        }

        private void RemoveBeforeCurrPeriodColumns(DataTable table, DateTime taskEndTime)
        {
            for (int i = table.Columns.Count - 1; i >= 0; i--)
            {
                var c = table.Columns[i];
                DateTime date;
                if (DateTime.TryParse(c.ColumnName, out date))
                {
                    if (date < taskEndTime)
                    {
                        table.Columns.Remove(c);
                    }
                }
            }
        }
    }
}