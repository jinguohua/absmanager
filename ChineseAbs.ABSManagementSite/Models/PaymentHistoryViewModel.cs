using ChineseAbs.ABSManagement.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.CalcService.Data.NancyData.Cashflows;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class PaymentHistoryViewModel
    {
        public PaymentHistoryViewModel()
        {
            HasReinvestmentInfo = false;

            NoteInfos = new List<NoteInfoViewModel>();
            Datasets = new List<DatasetViewModel>();
            Projects = new List<ProjectViewModel>();
            AssetDatasets = new List<AssetDatasetViewModel>();
        }

        /// <summary>
        /// 当前Project信息
        /// </summary>
        public Project CurrentProject { get; set; }

        /// <summary>
        /// 起息日
        /// </summary>
        public DateTime? ClosingDay { get; set; }

        /// <summary>
        /// 基本的NoteInfo 信息
        /// </summary>
        public List<NoteInfoViewModel> NoteInfos { get; set; }

        /// <summary>
        /// 多期的支付详情
        /// </summary>
        public List<DatasetViewModel> Datasets { get; set; }

        /// <summary>
        /// 所有有权限的Project信息
        /// </summary>
        public List<ProjectViewModel> Projects { get; set; }

        /// <summary>
        /// 多期的资产端数据
        /// </summary>
        public List<AssetDatasetViewModel> AssetDatasets { get; set; }

        public string AssetChartData { get; set; }

        /// <summary>
        /// 该产品是否包含循环购买数据（Reinvestment.csv）
        /// </summary>
        public bool HasReinvestmentInfo { get; set; }

        public string ExceptionMessage { get; set; }
    }

    /// <summary>
    /// Note的基本信息
    /// </summary>
    public class NoteInfoViewModel
    {
        /// <summary>
        /// 全称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 证券代码
        /// </summary>
        public string SecurityCode { get; set; }

        /// <summary>
        /// 发行金额
        /// </summary>
        public decimal? Notional { get; set; }

        /// <summary>
        /// 当期利率
        /// </summary>
        public string CouponString { get; set; }

        /// <summary>
        /// 预计到期日
        /// </summary>
        public DateTime? ExpectedMaturityDate { get; set; }

        /// <summary>
        /// 计息期规则
        /// </summary>
        public string AccrualMethon { get; set; }
    }

    /// <summary>
    /// 每期每Note支付的详细信息
    /// </summary>
    public class NoteDataViewModel
    {
        public NoteInfoViewModel NoteInfo { get; set; }

        /// <summary>
        /// 当期利率
        /// </summary>
        public string CurrentCouponRate { get; set; }

        public PaymentDetail PaymentDetail { get; set; }
    }

    /// <summary>
    /// 支付明细
    /// </summary>
    public class PaymentDetail
    {
        public decimal? PrincipalPaid { get; set; }

        public decimal? InterestPaid { get; set; }

        public decimal? EndingBalance { get; set; }
    }

    public class CashflowDatasetViewModel
    {
        public DatasetViewModel Dataset { get; set; }

        public string ProjectGuid { get; set; }

        public List<DateTime> ValidPaymentDays { get; set; }

        public List<DateTime> AllPaymentDays { get; set; }
    }

    public class DatasetViewModel
    {
        /// <summary>
        /// 第几期
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 支付日
        /// </summary>
        public DateTime? PaymentDay { get; set; }

        /// <summary>
        /// 汇总后的支付明细
        /// </summary>
        public PaymentDetail SumPaymentDetail { get; set; }

        /// <summary>
        /// 每Note支付的详细信息
        /// </summary>
        public List<NoteDataViewModel> NoteDatas { get; set; }
    }

    /// <summary>
    /// 单期资产端数据
    /// </summary>
    public class AssetDatasetViewModel
    {
        /// <summary>
        /// 第几期
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 支付日
        /// </summary>
        public DateTime PaymentDay { get; set; }

        /// <summary>
        /// 单期资产端数据汇总
        /// </summary>
        public AssetViewModel SumAsset { get; set; }

        /// <summary>
        /// 单期资产端数据
        /// </summary>
        public List<AssetViewModel> Assets { get; set; }

        /// <summary>
        /// 重计算SumAsset
        /// </summary>
        public void ReCalculateSumAsset()
        {
            SumAsset = new AssetViewModel()
            {
                Interest = Assets.Sum(asset => asset.Interest),
                Principal = Assets.Sum(asset => asset.Principal),
            };
        }
    }

    /// <summary>
    /// 单期单公司资产端数据
    /// </summary>
    public class AssetViewModel
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 台账Id
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// 偿付类型
        /// </summary>
        public AmortizationType AmortizationType { get; set; }

        /// <summary>
        /// 本金
        /// </summary>
        public decimal Principal { get; set; }

        /// <summary>
        /// 当期剩余本金
        /// </summary>
        public decimal PrincipalBalance { get; set; }

        /// <summary>
        /// 利息
        /// </summary>
        public decimal Interest { get; set; }

        /// <summary>
        /// 本金利息合计
        /// </summary>
        public decimal SumMoney { get { return Principal + Interest; } }
    }

    public class AssetCashflowDatasetViewModel
    {
        public AssetDatasetViewModel AssetDataset { get; set; }

        public string ProjectGuid { get; set; }

        public List<int> EnablePrepaymentAssetIds { get; set; }

        public List<DateTime> ValidPaymentDays { get; set; }

        public List<DateTime> AllPaymentDays { get; set; }
    }

    public class PrepayRecordListViewModel : List<PrepayRecordViewModel>
    {
        public PrepayRecordListViewModel(DatasetScheduleLogicModel datasetSchedule)
        {
            InitSchecule(datasetSchedule);
        }

        public PrepayRecordListViewModel(DatasetScheduleLogicModel datasetSchedule,
            IEnumerable<AmortizationScheduleRecord> records)
        {
            InitSchecule(datasetSchedule);
            AddRange(records);
        }

        public PrepayRecordListViewModel(DatasetScheduleLogicModel datasetSchedule,
            IEnumerable<NancyBasicAssetCashflowItem> records)
        {
            InitSchecule(datasetSchedule);
            AddRange(records);
        }

        private void InitSchecule(DatasetScheduleLogicModel datasetSchedule)
        {
            AsOfDateBegin = datasetSchedule.AsOfDateBegin;
            AsOfDateEnd = datasetSchedule.AsOfDateEnd;
            PaymentDate = datasetSchedule.PaymentDate;
        }

        public void Add(int assetId, double reductionAmount, DateTime reductionDate)
        {
            var prepayRecord = new PrepayRecordViewModel(assetId, reductionAmount, reductionDate);
            //TODO：计息区间的边界值处理？
            prepayRecord.InCurrentDataset = reductionDate <= AsOfDateEnd
                && reductionDate >= AsOfDateBegin;
            Add(prepayRecord);
        }

        public void Add(AmortizationScheduleRecord record)
        {
            var prepayRecord = new PrepayRecordViewModel(record);
            //TODO：计息区间的边界值处理？
            prepayRecord.InCurrentDataset = record.ReductionDate <= AsOfDateEnd
                && record.ReductionDate >= AsOfDateBegin;
            Add(prepayRecord);
        }

        public void Add(NancyBasicAssetCashflowItem record)
        {
            var prepayRecord = new PrepayRecordViewModel(record);
            //TODO：计息区间的边界值处理？
            prepayRecord.InCurrentDataset = record.PaymentDate == PaymentDate;
            Add(prepayRecord);
        }

        public void AddRange(IEnumerable<AmortizationScheduleRecord> records)
        {
            foreach (var record in records)
            {
                Add(record);
            }
        }

        public void AddRange(IEnumerable<NancyBasicAssetCashflowItem> records)
        {
            foreach (var record in records)
            {
                Add(record);
            }
        }

        public DateTime PaymentDate { get; set; }

        public DateTime AsOfDateBegin { get; set; }

        public DateTime AsOfDateEnd { get; set; }
    }

    public class PrepayRecordViewModel
    {
        public PrepayRecordViewModel(int assetId, double reductionAmount, DateTime reductionDate)
        {
            AssetId = assetId;
            ReductionAmount = reductionAmount.ToString("n2");
            ReductionDate = reductionDate.ToString("yyyyMMdd");
        }

        public PrepayRecordViewModel(AmortizationScheduleRecord record)
        {
            AssetId = record.AssetId;
            ReductionAmount = record.ReductionAmount.ToString("n2");
            ReductionDate = record.ReductionDate.ToString("yyyyMMdd");
        }

        public PrepayRecordViewModel(NancyBasicAssetCashflowItem record)
        {
            AssetId = record.AssetId;
            ReductionAmount = record.Principal.ToString("n2");
            ReductionDate = record.PaymentDate.ToString("yyyyMMdd");
        }

        public int AssetId { get; set; }
        public string ReductionAmount { get; set; }
        public string ReductionDate { get; set; }
        public bool InCurrentDataset { get; set; }
    }

    public class CollateralAssetViewModel
    {
        public CollateralAssetViewModel(AssetLogicModel asset)
        {
            AssetId = asset.AssetId;
            SecurityName = asset.SecurityData.SecurityName;
            PaymentMethod = asset.AmortizationType.ToString();
            Currency = asset.SecurityData.Currency.ToString();
            IsFloating = asset.SecurityData.IsFloating;
            AllInRate = asset.SecurityData.AllInRate;
            FloatingIndex = asset.SecurityData.FloatingIndex.ToString();
            FloatingIndexCode = asset.SecurityData.FloatingIndexCode.ToString();
            PaymentFrequency = asset.SecurityData.PaymentFrequency.ToString();
            AccrualMethod = Toolkit.AccrualMethod(asset.SecurityData.AccrualMethod);
            LegalMaturityDate = DateUtils.IsNormalDate(asset.SecurityData.LegalMaturityDate) ?
                Toolkit.DateToString(asset.SecurityData.LegalMaturityDate) : "-";
            PenultimateDate = DateUtils.IsNormalDate(asset.SecurityData.PenultimateDate) ?
                Toolkit.DateToString(asset.SecurityData.PenultimateDate) : "-";
            Region = asset.SecurityData.Region;
            ChineseRating = CommUtils.FormatChineseRating(asset.SecurityData.ChineseRating);
        }

        public int AssetId { get; set; }
        public string SecurityName { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public bool IsFloating { get; set; }
        public double AllInRate { get; set; }
        public string FloatingIndex { get; set; }
        public string FloatingIndexCode { get; set; }
        public string PaymentFrequency { get; set; }
        public string AccrualMethod { get; set; }
        public string LegalMaturityDate { get; set; }
        public string PenultimateDate { get; set; }
        public string Region { get; set; }
        public string ChineseRating { get; set; }
    }
}