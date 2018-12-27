using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using CNABS.Mgr.Deal.Model;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public class TaskExRecyclingPaymentDate : TaskExBase
    {
        public TaskExRecyclingPaymentDate(string userName, string shortCode)
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

        public override object GetEntity()
        {
            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(Task.ShortCode);
            var paymentDate = Task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }
            var projectLogicModel = new ProjectLogicModel(m_userName, Task.ProjectId);
            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
            var dealModel = datasetSchedule.Dataset.DealModel;
            paymentDate = datasetSchedule.PaymentDate;

            var absDeal = new ABSDeal(dealModel.YmlFolder, dealModel.DsFolder);
            var acfResult = absDeal.Result.AcfResult;
            acfResult.MergeOsa(dealModel.OverrideSingleAsset);
            acfResult.ReCalcSum();
            var acfDataset = acfResult.Dataset.FirstOrDefault(x => x.PaymentDay == paymentDate);
            double totalReceived = 0;
            if (acfDataset != null)
            {
                totalReceived = acfDataset.Sum.Principal + acfDataset.Sum.Interest;
            }

            var taskExInfo = RecyclingPaymentDate.FromJson(Task.TaskExtension.TaskExtensionInfo);

            var entity = new RecyclingPaymentDateViewModel();
            entity.ConfirmedAccountBalance = taskExInfo.ConfirmedAccountBalance;

            var compareSign = CommUtils.ParseEnum<SignComparisonModes>(taskExInfo.CompareSign);
            var acountType = CommUtils.ParseEnum<EAccountType>(taskExInfo.AccountType);
            var bankAccounts = m_dbAdapter.BankAccount.GetAccounts(Task.ProjectId, true);
            var accountBalance = bankAccounts.Where(x => x.AccountType == acountType)
                .Where(x => x.CurrentBalance != null).Sum(x => x.CurrentBalance.EndBalance);

            entity.CurrentAccountName = taskExInfo.AccountType;
            entity.CompareSign = taskExInfo.CompareSign;
            entity.CurrentAccountBalance = accountBalance;
            entity.PaymentMoney = (decimal)totalReceived;
            entity.CompareResult = CompareAccount((double)accountBalance, totalReceived, compareSign, acountType);
            entity.PaymentDay = Toolkit.DateToString(paymentDate);
            return entity;
        }

        public void Confirm(double currentAccountBalance, double paymentMoney)
        {
            var task = this.Task;
            m_dbAdapter.Task.CheckPrevIsFinished(Task);

            CommUtils.Assert(task.TaskExtensionId.HasValue, "ConfirmRecyclingPaymentDate失败，Task不包含扩展信息。");

            var taskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);

            CommUtils.AssertEquals(taskExtension.TaskExtensionType, TaskExtensionType.RecyclingPaymentDate.ToString(),
                "ConfirmRecyclingPaymentDate失败，TaskExtensionType错误。");

            var bankAccounts = m_dbAdapter.BankAccount.GetAccounts(task.ProjectId, true);

            var taskExInfo = RecyclingPaymentDate.FromJson(Task.TaskExtension.TaskExtensionInfo);
            var compareSign = CommUtils.ParseEnum<SignComparisonModes>(taskExInfo.CompareSign);
            var acountType = CommUtils.ParseEnum<EAccountType>(taskExInfo.AccountType);
            if (compareSign != SignComparisonModes.NotCompare)
            {
                var bankCount = bankAccounts.Count(x => x.AccountType == acountType);
                CommUtils.Assert(bankCount < 2, "[{0}]不唯一", acountType.ToString());
                CommUtils.Assert(bankCount > 0, "未配置[{0}]，请在[存续期设置]>[账户配置]中增加[{0}]。", acountType.ToString());
                var accountBalance = bankAccounts.Where(x => x.AccountType == acountType && x.CurrentBalance != null)
                    .Sum(x => x.CurrentBalance.EndBalance);

                CommUtils.AssertEquals((decimal)currentAccountBalance, accountBalance, 
                    "当前账户余额已更新，请刷新网页后再试");
            }

            var compareResult = CompareAccount(currentAccountBalance, paymentMoney, compareSign, acountType);
            CommUtils.Assert(compareResult.IsPassed, compareResult.ErrorDetail);

            taskExInfo.PaymentMoney = (decimal)paymentMoney;
            taskExInfo.ConfirmedAccountBalance = (decimal)currentAccountBalance;

            var json = CommUtils.ToJson(taskExInfo);
            taskExtension.TaskExtensionInfo = json;
            taskExtension.TaskExtensionStatus = TaskExtensionStatus.Finished;
            taskExtension.TaskExtensionHandler = m_userName;
            taskExtension.TaskExtensionHandleTime = DateTime.Now;
            m_dbAdapter.Task.SaveTaskExtension(taskExtension);

            var logicModel = new ProjectLogicModel(m_userName, task.ProjectId);
            new TaskLogicModel(logicModel, task).Start();
        }

        private RecyclingPaymentDateCompareResult CompareAccount(double accountBalance, double paymentMoney, SignComparisonModes compareSign, EAccountType accountType)
        {
            accountBalance = Math.Round(accountBalance, 2);
            paymentMoney = Math.Round(paymentMoney, 2);
            var isPassed = true;
            var errorMsgFormat = string.Empty;

            switch (compareSign)
            {
                case SignComparisonModes.NotCompare:
                    break;
                case SignComparisonModes.Equal:
                    isPassed = accountBalance == paymentMoney;
                    errorMsgFormat = "{0}余额{1}应该等于需支付金额{2}";
                    break;
                case SignComparisonModes.NotEqual:
                    isPassed = accountBalance != paymentMoney;
                    errorMsgFormat = "{0}余额{1}不应该与需支付金额{2}相等";
                    break;
                case SignComparisonModes.GreaterThan:
                    isPassed = accountBalance > paymentMoney;
                    errorMsgFormat = "{0}余额{1}应该大于需支付金额{2}";
                    break;
                case SignComparisonModes.LessThan:
                    isPassed = accountBalance < paymentMoney;
                    errorMsgFormat = "{0}余额{1}应该小于需支付金额{2}";
                    break;
                case SignComparisonModes.GreateThanEqual:
                    isPassed = accountBalance >= paymentMoney;
                    errorMsgFormat = "{0}余额{1}小于需支付金额{2}，余额不足";
                    break;
                case SignComparisonModes.LessThanEqual:
                    isPassed = accountBalance <= paymentMoney;
                    errorMsgFormat = "{0}余额{1}应该小于或等于需支付金额{2}";
                    break;
                default:
                    break;
            }

            return new RecyclingPaymentDateCompareResult
            {
                IsPassed = isPassed,
                ErrorMsg = string.Format(errorMsgFormat, accountType.ToString(),string.Empty, string.Empty),
                ErrorDetail = string.Format(errorMsgFormat, accountType.ToString(),
                    "[" + accountBalance.ToString("n2") + "]", "[" +　paymentMoney.ToString("n2") + "]"),
            };
        }
    }
}