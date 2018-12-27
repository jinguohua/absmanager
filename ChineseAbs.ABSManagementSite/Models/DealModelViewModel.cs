using ABS.Core;
using ABS.Core.Models;
using ABS.Infrastructure.Models;
using ChineseAbs.ABSManagementSite.Helpers;
using CNABS.Mgr.Deal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ABS.ABSManagementSite.Models
{
    public class DealModelViewModel
    {
        public DealModelViewModel()
        {
            BasicInformation = new BasicInformationViewModel();
            Fees = new FeesViewModel();
            Notes = new NotesViewModel();
            Schedule = new ScheduleViewModel();
            CollateralRule = new CollateralRuleViewModel();
            CreditEnhancement = new CreditEnhancementViewModel();
            ScenarioRule = new ScenarioRuleViewModel();
        }

        public static List<SelectListItem> GetOrdinalSelectList(int total, string select = "", params int[] complex)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "-请选择-" });
            int i, j;
            string item;
            for (i = 1; i <= total; i++)
            {
                item = i.ToString();
                list.Add(new SelectListItem()
                {
                    Value = item,
                    Text = item,
                    Selected = item.Equals(select)
                });
                if (i <= complex.Length)
                {
                    for (j = 1; j <= complex[i - 1]; j++)
                    {
                        item = i.ToString() + "." + j.ToString();
                        list.Add(new SelectListItem()
                        {
                            Value = item,
                            Text = item,
                            Selected = item.Equals(select)
                        });
                    }
                }
            }
            return list;
        }

        public string Guid { get; set; }

        public int Id { get; set; }

        public EDealLabProjectStatus ModelStatus { get; set; }

        public bool IsInfoComplete { get; set; }

        [Display(Name = "模型名称")]
        public string ModelName { get; set; }

        [Display(Name = "创建者")]
        public string Creator { get; set; }

        [Display(Name = "基本信息")]
        public BasicInformationViewModel BasicInformation { get; set; }

        [Display(Name = "费用设置")]
        public FeesViewModel Fees { get; set; }

        [Display(Name = "证券设置")]
        public NotesViewModel Notes { get; set; }

        [Display(Name = "日期参数")]
        public ScheduleViewModel Schedule { get; set; }

        [Display(Name = "资产池规则")]
        public CollateralRuleViewModel CollateralRule { get; set; }

        [Display(Name = "事件与增信设置")]
        public CreditEnhancementViewModel CreditEnhancement { get; set; }

        [Display(Name = "支付规则中是否区分收入和本金分帐户")]
        public bool SplitInterestAndPrincipalProceeds { get; set; }

        [Display(Name = "情景设置")]
        public ScenarioRuleViewModel ScenarioRule { get; set; }
    }

    public class BasicInformationViewModel
    {

        [Display(Name = "产品简称")]
        [StringLength(20, ErrorMessage = "{0}超过字数限制({1}字符)")]
        public string DealName { get; set; }

        [Display(Name = "产品全称")]
        [StringLength(20, ErrorMessage = "{0}超过字数限制({1}字符)")]
        public string DealFullName { get; set; }

        [Display(Name = "原始权益人")]
        [StringLength(20, ErrorMessage = "{0}超过字数限制({1}字符)")]
        public string Originator { get; set; }

        [Display(Name = "受托机构")]
        [StringLength(20, ErrorMessage = "{0}超过字数限制({1}字符)")]
        public string Issuer { get; set; }

        [Display(Name = "主承销(推广)")]
        [StringLength(20, ErrorMessage = "{0}超过字数限制({1}字符)")]
        public string LeadUnderwriter { get; set; }

        [Display(Name = "交易流通场所")]
        public EExchange Exchange { get; set; }

        [Display(Name = "作者备注")]
        [StringLength(100, ErrorMessage = "{0}超过字数限制({1}字符)")]
        public string Comment { get; set; }
    }

    public class FeesViewModel
    {
        [Display(Name = "费用")]
        public List<FeeComponentViewModel> FeeComponents { get; set; }
    }

    public class FeeComponentViewModel
    {
        [Display(Name = "名称")]
        [StringLength(20, ErrorMessage = "费用{0}超过字数限制({1}字符)")]
        public string Name { get; set; }

        [Display(Name = "描述")]
        [StringLength(50, ErrorMessage = "费用{0}超过字数限制({1}字符)")]
        public string Description { get; set; }

        public bool NoFixedAmount { get { return IsProRated.HasValue && (bool)IsProRated; } }

        [Display(Name = "固定金额")]
        public double? FixedAmount { get; set; }

        [Display(Name = "类型")]
        public bool? IsProRated { get; set; }

        [Display(Name = "固定费率")]
        public bool IsFixedRate { get; set; }

        [Display(Name = "每期费率")]
        public bool IsPerPaymentRate { get; set; }

        [Display(Name = "首期费率不同")]
        public bool HasDiffFirstRate { get; set; }

        [Display(Name = "首期费率")]
        public double? FirstRate { get; set; }

        [Display(Name = "费率")]
        public double? FixedRate { get; set; }

        [Display(Name = "利差(bps)")]
        public double? Spread { get; set; }

        [Display(Name = "基准利率")]
        public string FloatingIndex { get; set; }

        [Display(Name = "上限")]
        public string Cap { get; set; }

        [Display(Name = "下限")]
        public string Floor { get; set; }

        [Display(Name = "累计上限")]
        public bool IsCumulativeCap { get; set; }

        [Display(Name = "计日天数")]
        public EAccrualMethod? AccrualMethod { get; set; }

        [Display(Name = "优先级")]
        public int? PaymentOrdinal { get; set; }

        [Display(Name = "费用计算基数")]
        public EBasisType? FeeBasisType { get; set; }

        [Display(Name = "记录未偿金额")]
        public bool NeedUnpaidAccount { get; set; }

        [Display(Name = "未偿金额计息")]
        public bool NeedInterestOnUnpaidAccount { get; set; }

        [Display(Name = "优先支付费用上限")]
        public bool NeedPriorityExpenseCap { get; set; }

        [Display(Name = "优先支出上限")]
        public string PriorityExpenseCap { get; set; }

        [Display(Name = "支付计划")]
        public List<DateValue> PaymentSchedules { get; set; }
    }


    public class NotesViewModel
    {
        [Display(Name = "证券总金额")]
        public decimal TotalBalance { get; set; }

        [Display(Name = "支持证券")]
        public List<NoteDataViewModel> NoteList { get; set; }
    }

    public class NoteDataViewModel
    {
        [Display(Name = "名称")]
        [Required(ErrorMessage = "证券{0}不能为空")]
        public string Name { get; set; }

        [Display(Name = "描述")]
        [Required(ErrorMessage = "证券{0}不能为空")]
        [StringLength(100, ErrorMessage = "证券{0}超过字数限制({1}字符)")]
        public string Description { get; set; }

        [Display(Name = "固定利率")]
        public bool IsFixed { get; set; }

        [Display(Name = "次级证券")]
        public bool IsEquity { get; set; }

        [Display(Name = "偿付顺序")]
        [Required(ErrorMessage = "请选择{0}")]
        public double? PaymentOrdinal { get; set; }

        [Display(Name = "利差(bps)")]
        public double? Spread { get; set; }

        [Display(Name = "利率")]
        public double? FixedRate { get; set; }

        [Display(Name = "本金占比")]
        [Required(ErrorMessage = "证券{0}不能为空")]
        public double? Notional { get; set; }

        [Display(Name = "本金(万)")]
        public double? NotionalAmount { get; set; }

        [Display(Name = "记录逾期利息")]
        public bool HasUnpaidInterest { get { return this.HasInterestOnUnpaidInterest; } }

        [Display(Name = "逾期利息计息")]
        public bool HasInterestOnUnpaidInterest { get; set; }

        [Display(Name = "计息期不调整")]
        public bool UnAdjustedAccrualPeriods { get; set; }

        [Display(Name = "基准利率")]
        public string FloatingIndex { get; set; }

        [Display(Name = "评级")]
        public string RatingStr { get; set; }

        [Display(Name = "有上限")]
        public bool HasCap { get; set; }

        [Display(Name = "上限")]
        public string CapFormula { get; set; }

        [Display(Name = "计日天数")]
        [Required(ErrorMessage = "请选择{0}")]
        public EAccrualMethod? AccrualMethod { get; set; }

        [Display(Name = "是否摊还")]
        public bool HasAmortizationSchedule { get; set; }

        [Display(Name = "摊还时间表")]
        public SimpleScheduleData AmortizationSchedule { get; set; }

        [Display(Name = "预计到期日")]
        public DateTime? ExpectedMaturityDate { get; set; }

        [Display(Name = "期望评级")]
        public string ExpectedRating { get; set; }
    }

    public class AdjustScheduleViewModel
    {
        public string OrigDate { get; set; }

        public string CustomDate { get; set; }
    }

    public class ScheduleViewModel
    {
        public ScheduleViewModel()
        {
            FrequencyTypes = MyEnumConvertor.ConvertToSelectList(typeof(EFrequency));
            DeterminationRuleTypes = MyEnumConvertor.ConvertToSelectList(typeof(EDeterminationRules));
            PaymentRollingTypes = MyEnumConvertor.ConvertToSelectList(typeof(ERolling));
            DeterminationDateRollingTypes = MyEnumConvertor.ConvertToSelectList(typeof(ERolling));
        }

        [Display(Name = "证券起息日")]
        public DateTime? ClosingDate { get; set; }

        [Display(Name = "首次支付日")]
        public DateTime? FirstPaymentDate { get; set; }

        [Display(Name = "法定到期日")]
        public DateTime? LegalMaturity { get; set; }

        [Display(Name = "法定到期日前一支付日")]
        public DateTime? PenultimateDate { get; set; }

        [Display(Name = "支付频率")]
        public EFrequency? PaymentFrequency { get; set; }
        public List<SelectListItem> FrequencyTypes { get; set; }

        [Display(Name = "月末支付")]
        public bool PaymentAtMonthEnd { get; set; }

        [Display(Name = "收款期间模式")]
        public EDeterminationRules? DeterminationRuleType { get; set; }
        public List<SelectListItem> DeterminationRuleTypes { get; set; }

        [Display(Name = "相对支付日(天)")]
        public int? DeterminationOffset { get; set; }

        [Display(Name = "首个收款期起始日")]
        public DateTime? FirstDeterminationDateBegin { get; set; }

        [Display(Name = "首个收款期截止日")]
        public DateTime? FirstDeterminationDateEnd { get; set; }

        [Display(Name = "日期调整规则")]
        public ERolling? PaymentRolling { get; set; }

        public List<SelectListItem> PaymentRollingTypes { get; set; }
        public List<SelectListItem> DeterminationDateRollingTypes { get; set; }

        [Display(Name = "收款期日期规则")]
        public ERolling? DeterminationDateRolling { get; set; }

        [Display(Name = "固定月末")]
        public bool DeterminationDateAtMonthEnd { get; set; }

        [Display(Name = "循环期截止")]
        public DateTime? ReinvestmentEndDate { get; set; }

        [Display(Name = "需要自定义收款期")]
        public bool AdjustCollectionSchedule { get; set; }

        [Display(Name = "需要自定义偿付日期")]
        public bool AdjustPaymentSchedule { get; set; }

        [Display(Name = "自定义收款日期")]
        public List<AdjustScheduleViewModel> CustomCollectionSchedule { get; set; }

        [Display(Name = "自定义偿付日期")]
        public List<AdjustScheduleViewModel> CustomPaymentSchedule { get; set; }

        [Display(Name = "封包日")]
        public DateTime? CloseAssetDate { get; set; }

    }

    public class CreditEnhancementViewModel
    {
        public CreditEnhancementViewModel()
        {
            BasisTypes = MyEnumConvertor.ConvertToSelectList(typeof(EBasisType));
            PaymentSequences = MyEnumConvertor.ConvertToSelectList(typeof(EPaymentSequence));
        }

        [Display(Name = "设置违约事件")]
        public bool HasEod { get; set; }

        [Display(Name = "最优先级违约")]
        public bool EodCheckHighestPriorityNoteOnly { get; set; }

        [Display(Name = "条件")]
        public EEodCheckCondition? EodCheckCondition { get; set; }

        [Display(Name = "违约可被修复")]
        public bool EodCanCure { get; set; }

        [Display(Name = "加速清偿")]
        public bool HasTurbo
        {
            get
            {
                return HasCumlossTurbo || HasNoteExpectedMaturityTurbo;
            }
        }

        [Display(Name = "预计到期日触发")]
        public bool HasEodExpectedMaturityTurbo { get; set; }

        [Display(Name = "加速清偿可被修复")]
        public bool TurboCancure { get; set; }

        [Display(Name = "累计违约率触发")]
        public bool HasCumlossTurbo { get; set; }

        [Display(Name = "阈值")]
        public double? CumlossTurboThreshold { get; set; }

        [Display(Name = "阈值时间表")]
        public bool HasCumlossTurboThresholdSchedule { get; set; }

        [Display(Name = "阈值时间表")]
        public SimpleScheduleData CumlossTurboThresholdSchedule { get; set; }

        [Display(Name = "违约率计算基数")]
        public EBasisType? CumlossTurboBasisType { get; set; }
        public List<SelectListItem> BasisTypes { get; set; }

        [Display(Name = "初始资产池余额")]
        public double? OriginalAPB
        {
            get
            {
                return 1.0;
            }
        }

        [Display(Name = "预计到期日触发")]
        public bool HasNoteExpectedMaturityTurbo { get; set; }

        [Display(Name = "设储备金账户")]
        public bool HasRiskReserve { get; set; }

        [Display(Name = "储备金上限")]
        public double? RiskReserveCap { get; set; }

        [Display(Name = "储备金利率")]
        public double? RiskReserveInterestRate { get; set; }

        [Display(Name = "储备金初始金额")]
        public double? RiskReserveInitValue { get; set; }

        [Display(Name = "储备金仅用于优先级摊还证券")]
        public bool RiskReserveForSeniorOnly { get; set; }

        [Display(Name = "是否有差额支付")]
        public bool HasDeficiencyPayment { get; set; }

        [Display(Name = "差额支付上限")]
        public double DeficiencyPaymentCap { get; set; }

        [Display(Name = "是否有外部担保")]
        public bool HasGuarantee { get; set; }

        [Display(Name = "担保上限")]
        public double GuaranteeCap { get; set; }

        [Display(Name = "保证金用作差额支付")]
        public bool UseLeaseMarginAsDeficiencyPayment { get; set; }

        [Display(Name = "保证金帐户初始值")]
        public double MarginInitValue { get; set; }

        [Display(Name = "支付时合并利息本金归集帐户")]
        public bool PayWaterfallByCombinedProceeds { get; set; }

        [Display(Name = "触发顺序")]
        public EPaymentSequence? DeficiencyPaymentSequence { get; set; }

        public List<SelectListItem> PaymentSequences { get; set; }

        [Display(Name = "违约事件触发立即清算")]
        public bool LiquidateWhenEod { get; set; }

        [Display(Name = "超额收益优先支付次级收益")]
        public bool PaySubResidualInterestBeforeOtherNotesPaidOff { get; set; }
    }

    public class CollateralRuleViewModel
    {
        public CollateralRuleViewModel()
        {
            ReinvestmentRule = new ReinvestmentRuleViewModel();
        }

        [Display(Name = "是否循环购买")]
        public bool HasReinvestment { get; set; }

        public ReinvestmentRuleViewModel ReinvestmentRule { get; set; }
    }

    public class ReinvestmentRuleViewModel
    {
        [Display(Name = "循环购买规则")]
        public string ReinvestmentRuleType
        {
            get
            {
                if (IsReinvestingSimilarAssets)
                {
                    return "similar";
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value == "similar")
                {
                    IsReinvestingSimilarAssets = true;
                }
                else
                {
                    IsReinvestingSimilarAssets = false;
                }
            }
        }

        [Display(Name = "是否延续当前资产池属性")]
        public bool IsReinvestingSimilarAssets { get; set; }

        [Display(Name = "加权剩余期限")]
        public double? Wal { get; set; }

        [Display(Name = "加权平均利率")]
        public double? Wac { get; set; }

        public string RatingString { get; set; }

        public double? RecoveryRate { get; set; }

        [Display(Name = "循环购买截止日期")]
        public DateTime? ReinvestmentEndDate { get; set; }
    }

    public class ScenarioRuleViewModel
    {
        public string ScenarioGuid { get; set; }

        public string ScenarioName { get; set; }

        [Display(Name = "CPR")]
        public double? CPR { get; set; }

        [Display(Name = "CDR")]
        public double? CDR { get; set; }

        [Display(Name = "使用账龄违约曲线")]
        public bool UseLoanAgeDefaultCurve { get; set; }

        [Display(Name = "使用账龄早偿曲线")]
        public bool UseLoanAgePrepaymentCurve { get; set; }


        [Display(Name = "账龄违约曲线")]
        public string LoanAgeDefaultCurve { get; set; }

        [Display(Name = "账龄早偿曲线")]
        public string LoanAgePrepaymentCurve { get; set; }
    }
}