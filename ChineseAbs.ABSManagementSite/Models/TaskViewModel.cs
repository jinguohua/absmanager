using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.Object;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class TaskViewModel
    {
        public TaskViewModel()
        {
        }

        public string Id { get; set; }

        public string ProjectGuid { get; set; }

        public string ProjectName { get; set; }

        public string TaskName { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string ShortCode { get; set; }

        public TaskStatus Status { get; set; }

        public string PrevTasksNames { get; set; }

        public List<string> PrevTaskShortCodeArray { get; set; }

        public List<string> PrevTaskNameArray { get; set; }

        public TaskExtensionViewModel TaskExtension { get; set; }

        public List<TaskStatusHistoryViewModel> TaskStatusHistory { get; set; }

        public string TaskDetail { get; set; }

        public string TaskTarget { get; set; }

        public string TaskHandler { get; set; }

        //当前工作所在阶段（发行、存续期、终止、清算）
        public ProjectSeriesStage ProjectSeriesStage { get; set; }

        public string ProjectSeriesGuid { get; set; }

        /// <summary>
        /// 上一个工作的shortCode，不是前置工作，只用于工作页面的link跳转
        /// </summary>
        public string PreviousTaskShortCode { get; set; }

        /// <summary>
        /// 上一个工作的名称，不是前置工作，只用于工作页面的link跳转
        /// </summary>
        public string PreviousTaskName { get; set; }

        /// <summary>
        /// 下一个工作的shortCode，只用于工作页面的link跳转
        /// </summary>
        public string NextTaskShortCode { get; set; }

        /// <summary>
        /// 下一个工作的名称，只用于工作页面的link跳转
        /// </summary>
        public string NextTaskName { get; set; }
    }

    public class TaskExtensionViewModel
    {
        public TaskExtensionViewModel()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public object Info { get; set; }

        public TaskExtensionStatus Status { get; set; }

        public string Handler { get; set; }

        public DateTime? HandleTime { get; set; }
    }
    
    public class RecyclingPaymentDate
    {
        /// <summary>
        /// 需支付金额
        /// </summary>
        public decimal PaymentMoney { get; set; }

        /// <summary>
        /// 上次确认过的账户余额
        /// </summary>
        public decimal? ConfirmedAccountBalance { get; set; }

        /// <summary>
        /// 比较符号种类（例如 =, !=, >, < 等符号）
        /// </summary>
        public string CompareSign { set; get; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public string AccountType { set; get; }

        //解析json序列化后的RecyclingPaymentDate
        //缺省账户类型为专项计划账户
        //缺省比较类型为GreateThanEqual
        public static RecyclingPaymentDate FromJson(string json)
        {
            var obj = new RecyclingPaymentDate() 
            {
                PaymentMoney = 0,
                ConfirmedAccountBalance = null,
                AccountType = EAccountType.专项计划账户.ToString(),
                CompareSign = SignComparisonModes.GreateThanEqual.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(json))
            {
                var jsonObj = CommUtils.FromJson<RecyclingPaymentDate>(json);

                obj.PaymentMoney = jsonObj.PaymentMoney;
                obj.ConfirmedAccountBalance = jsonObj.ConfirmedAccountBalance;

                if (!string.IsNullOrWhiteSpace(jsonObj.AccountType))
                {
                    obj.AccountType = jsonObj.AccountType;
                }
                if (!string.IsNullOrWhiteSpace(jsonObj.CompareSign))
                {
                    obj.CompareSign = jsonObj.CompareSign;
                }
            }

            return obj;
        }
    }

    /// <summary>
    /// 比较符号种类（例如 =, !=, >, < 等符号）
    /// </summary>
    public enum SignComparisonModes
    {
        NotCompare = 1,
        Equal = 2,
        NotEqual = 3,
        GreaterThan = 4,
        LessThan = 5,
        GreateThanEqual = 6,
        LessThanEqual = 7
    }

    public class RecyclingPaymentDateViewModel
    {
        /// <summary>
        /// 当前账户余额
        /// </summary>
        public decimal CurrentAccountBalance { get; set; }

        /// <summary>
        /// 需支付金额
        /// </summary>
        public decimal PaymentMoney { get; set; }

        /// <summary>
        /// 上次确认过的账户余额
        /// </summary>
        public decimal? ConfirmedAccountBalance { get; set; }

        /// <summary>
        /// 当前账户名称
        /// </summary>
        public string CurrentAccountName { get; set; }

        /// <summary>
        /// 比较符号种类（例如 =, !=, >, < 等符号）
        /// </summary>
        public string CompareSign { set; get; }

        /// <summary>
        /// 当前账户余额和需支付余额比较结果
        /// </summary>
        public RecyclingPaymentDateCompareResult CompareResult { set; get; }

        public string PaymentDay { get; set; }
    }

    public class RecyclingPaymentDateCompareResult
    {
        //是否满足比较规则
        public bool IsPassed { get; set; }

        //不满足比较规则时的错误信息
        public string ErrorMsg { get; set; }

        //不满足比较规则时的错误信息(包含金额)
        public string ErrorDetail { get; set; }
    }

    public class AssetCashflowStatisticInfo
    {
        public AssetCashflowStatisticInfo()
        {
        }
        
        public double TotalCurrentPrinCollection { get; set; }

        public double TotalCurrentInterestCollection { get; set; }

        public string PaymentDay { get; set; }
    }

    public class TaskExDocumentViewModel
    {
        public List<TaskExDocumentItem> Documents { get; set; }

        public TaskExCheckListInfo TaskExCheckLists { get; set; }

        public string PaymentDay { get; set; }
    }

    public class CashflowViewModel
    {
        public CashflowViewModel()
        {
            CashflowList = new List<CashflowElementData>();
            OverridableVariables = new List<CashflowVariablesData>();
            CashflowDataResult = new List<List<string>>();
            ColHeader = new List<string>();
            MergeCellsInfo = new List<Tuple<int, int, int, int>>();
        }

        public DateTime[] PaymentDates { get; set; }

        public List<CashflowElementData> CashflowList { get; set; }
        
        public List<CashflowVariablesData> OverridableVariables { get; set; }

        public string TestFailRemind  { get; set; }

        public double PredictInterestCollection { get; set; }

        public double PredictPricipalCollection { get; set; }

        public double? CurrentInterestCollection { get; set; }

        public double? CurrentPricipalCollection { get; set; }

        public List<string> ColHeader { get; set; }

        public List<List<string>> CashflowDataResult { get; set; }

        public List<Tuple<int, int, int, int>> MergeCellsInfo { get; set; }

        public string PaymentDay { get; set; }

    }
    
    public class TaskStatusHistoryViewModel
    {
        public TaskStatusHistoryViewModel()
        {
        }

        public int TaskStatusHistoryId { get; set; }

        public int TaskId { get; set; }

        public TaskStatus PrevStatus { get; set; }

        public TaskStatus NewStatus { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public string Comment { get; set; }
    }

    public class CashflowDetailTableViewModel
    {
        public CashflowDetailTableViewModel()
        {
            ProjectCashflowList = new List<CashflowData>();
            AssetPoolList = new List<CashflowData>();
            CostList = new List<CashflowData>();
            AccountList = new List<CashflowData>();
            TriggerEventList = new List<CashflowData>();
            CurrPeriodCashflowInfoList = new List<CashflowData>();
            CashflowEventList = new List<CashflowHomePageEvent>();
        }

        public string FeePayable { get; set; }
        public string CurrentPaymentDate { get; set; }

        public List<string> ProjectCashflowHeader { get; set; }
        public List<string> SecurityCashflowHeader { get; set; }
        public List<string> AssetPoolHeader { get; set; }
        public List<string> CostHeader { get; set; }
        public List<string> AccountHeader { get; set; }
        public List<string> TriggerEventHeader { get; set; }
        public List<string> HomePageHeader { get; set; }

        /// <summary>
        /// 产品现金流表
        /// </summary>
        public List<CashflowData> ProjectCashflowList { get; set; }

        /// <summary>
        /// 证券现金流表
        /// </summary>
        public Dictionary<string,List<CashflowData>> SecurityCashflowList { get; set; }

        /// <summary>
        /// 资产池表
        /// </summary>
        public List<CashflowData> AssetPoolList { get; set; }

        /// <summary>
        /// 费用表
        /// </summary>
        public List<CashflowData> CostList { get; set; }

        /// <summary>
        /// 账户表
        /// </summary>
        public List<CashflowData> AccountList { get; set; }

        /// <summary>
        /// 触发事件表
        /// </summary>
        public List<CashflowData> TriggerEventList { get; set; }

        /// <summary>
        /// 当期资产池与证券信息汇总表
        /// </summary>
        public List<CashflowData> CurrPeriodCashflowInfoList { get; set; }

        /// <summary>
        /// 当期触发事件表（主页）
        /// </summary>
        public List<CashflowHomePageEvent> CashflowEventList { get; set; }

        public string TestFailRemind { get; set; }
    }

    public class CashflowHomePageEvent
    {
        public string EventKey { get; set; }

        public string EventValue { get; set; }
    }
}