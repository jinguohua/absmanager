using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.AssetEvent;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using SFL.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class PaymentHistoryAssetController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string projectGuid, string paymentDay)
        {
            var paymentDate = DateUtils.ParseDigitDate(paymentDay);

            var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);

            var acfTable = datasetSchedule.Dataset.DealModel.AssetCashflowDt;
            var assets = datasetSchedule.Dataset.Assets;
            Toolkit.AddAssetIdToRepeatedCNName(acfTable, assets);
            var dataset = datasetSchedule.Dataset.Instance;

            var viewModel = new AssetCashflowDatasetViewModel();
            viewModel.ProjectGuid = projectGuid;

            viewModel.AssetDataset = new AssetDatasetViewModel();
            viewModel.AssetDataset.Assets = new List<AssetViewModel>();

            foreach (var assetItem in datasetSchedule.Dataset.Assets)
            {
                //Asset中可能包含循环购买时，系统生成的Asset
                //（当AssetId为0，且在collateral.csv中不存在该Asset时，认为是系统生成Asset）
                if (assetItem.SecurityData == null)
                {
                    continue;
                }

                var asset = assetItem.BasicAsset;
                //当m_cdo.OverridesCollateralCF为true时（资产覆盖现金流开关打开）
                //Nancy不返回单笔资产信息（参考CAssetCashflowAnalyzer.vb SVN Revision=28324）
                if (asset == null)
                {
                    continue;
                }
                viewModel.AssetDataset.Assets.Add(new AssetViewModel
                {
                    AssetId = asset.AssetId,
                    Interest = (decimal)asset.Interest,
                    Principal = (decimal)asset.Principal,
                    PrincipalBalance = (decimal)asset.Performing,
                    Name = assetItem.SecurityData.SecurityName,
                    AmortizationType = assetItem.AmortizationType
                });
            }

            viewModel.AssetDataset.PaymentDay = paymentDate;
            viewModel.AssetDataset.ReCalculateSumAsset();
            viewModel.AssetDataset.Sequence = datasetSchedule.Sequence;

            var amortization = datasetSchedule.Dataset.AmortizationSchedule;
            viewModel.EnablePrepaymentAssetIds = amortization.ConvertAll(x => x.AssetId).Distinct().ToList();

            //添加当期之前（含今天所在的一期）的所有支付日
            var nowDate = DateTime.Today;
            viewModel.ValidPaymentDays = new List<DateTime>();
            foreach (var durationPeriod in projectLogicModel.DealSchedule.DurationPeriods)
            {
                //只添加已上传模型的支付日
                datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(durationPeriod.PaymentDate);
                if (datasetSchedule.Dataset.Instance != null)
                {
                    viewModel.ValidPaymentDays.Add(durationPeriod.PaymentDate);
                }

                if (durationPeriod.PaymentDate > nowDate)
                {
                    break;
                }
            }

            var cnabsDealId = projectLogicModel.Instance.CnabsDealId;
            if (cnabsDealId.HasValue)
            {
                viewModel.AllPaymentDays = m_dbAdapter.Model.GetPaymentDates(cnabsDealId.Value);
            }
            else
            {
                if (projectLogicModel.DealSchedule.Instanse != null)
                {
                    viewModel.AllPaymentDays = projectLogicModel.DealSchedule.Instanse.PaymentDates.ToList();
                }
                else
                {
                    viewModel.AllPaymentDays = new List<DateTime>();
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetAmortizationByAsset(string projectGuid, string paymentDate, int assetId)
        {
            return ActionUtils.Json(() =>
            {
                var project = new ProjectLogicModel(CurrentUserName, projectGuid);
                var dataset = project.DealSchedule.GetByPaymentDay(DateUtils.ParseDigitDate(paymentDate)).Dataset;
                var asset = dataset.Assets.Single(x => x.AssetId == assetId);
                
                if (asset.AmortizationType == AmortizationType.UserDefined)
                {
                    var records = dataset.AmortizationSchedule.SelectByAsset(assetId);
                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);
                    viewModel.AsOfDateBegin = dataset.DatasetSchedule.AsOfDateBegin;
                    viewModel.AsOfDateEnd = dataset.DatasetSchedule.AsOfDateEnd;
                    return ActionUtils.Success(viewModel);
                }
                else if (IsEqualPmtOrEqualPrin(asset.AmortizationType))
                {
                    if (IsEqualPmtOrEqualPrin(asset.SecurityData.PaymentMethod))
                    {
                        //当期的偿付类型是EqualPmt/EqualPrin
                        //返回值为系统根据EqualPmt/EqualPrin测算出的本金
                        var basicAnalyticsData = NancyUtils.GetBasicAnalyticsData(project.Instance.ProjectId, null, dataset.Instance.AsOfDate);
                        var assetCashflow = basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems.SelectByAsset(assetId);

                        var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, assetCashflow);
                        return ActionUtils.Success(viewModel);
                    }
                    else
                    {
                        //已经发生了提前偿付，后几期的偿付类型是EqualPmt/EqualPrin
                        //返回值为提前偿付+系统根据EqualPmt/EqualPrin测算出的本金
                        var records = dataset.AmortizationSchedule.SelectByAsset(assetId);
                        var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);

                        var nextDatasetBasicAnalyticsData = NancyUtils.GetBasicAnalyticsData(project.Instance.ProjectId, null, dataset.DatasetSchedule.Next.AsOfDateBegin);
                        var nextAssetCashflowItems = nextDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                            .SelectByAsset(assetId).Where(x => x.PaymentDate > dataset.DatasetSchedule.PaymentDate);

                        viewModel.AddRange(nextAssetCashflowItems);
                        return ActionUtils.Success(viewModel);
                    }
                }
                else if (asset.AmortizationType == AmortizationType.SingleAmortization)
                {
                    var records = dataset.AmortizationSchedule.SelectByAsset(assetId);
                    var sumPrepayMoney = records.Sum(x => x.ReductionAmount);
                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);
                    viewModel.Add(assetId, asset.SecurityData.PrincipalBalance - sumPrepayMoney, project.DealSchedule.LegalMaturity);
                    return ActionUtils.Success(viewModel);
                }
                
                return ActionUtils.Failure("无法识别的偿付类型，projectGuid=["
                    + projectGuid + "] + paymentDate=[" + paymentDate + "] + assetId=[" + assetId + "]");
            });
        }

        private void CreateDataset(Project project, string asOfDate)
        {
            var projectId = project.ProjectId;
            var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(projectId);

            if (datasets.Any(x => x.AsOfDate == asOfDate))
            {
                throw new ApplicationException("产品[" + project.Name + "]中，dataset [" + asOfDate + "]已存在。");
            }

            //Find payment date
            DateTime? paymentDate = null;
            var dealSchedule = NancyUtils.GetDealSchedule(project.ProjectId);
            if (dealSchedule == null || dealSchedule.PaymentDates == null || dealSchedule.PaymentDates.Length == 0)
            {
                //创建第一期Dataset时，没有模型数据，无法读取DealSchedule，此时PaymentDate从数据库中取得
                if (project.CnabsDealId.HasValue)
                {
                    var paymentDates = m_dbAdapter.Model.GetPaymentDates(project.CnabsDealId.Value);
                    var date = DateTime.ParseExact(asOfDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    if (paymentDates.All(x => x < date))
                    {
                        throw new ApplicationException("产品[" + project.Name + "]中，dataset [" + asOfDate + "]不在任何支付日之前。");
                    }
                    paymentDate = paymentDates.First(x => x >= date);
                }
            }
            else
            {
                //创建第N期Dataset时，有模型数据，此时PaymentDate按照DealSchedule中AsOfDate对应Index查出
                var curAsOfDate = DateUtils.ParseDigitDate(asOfDate);
                if (curAsOfDate == dealSchedule.FirstCollectionPeriodStartDate)
                {
                    paymentDate = dealSchedule.PaymentDates[0];
                }
                else
                {
                    for (int i = 0; i < dealSchedule.DeterminationDates.Length - 1; ++i)
                    {
                        if (curAsOfDate == dealSchedule.DeterminationDates[i])
                        {
                            paymentDate = dealSchedule.PaymentDates[i + 1];
                            break;
                        }
                    }
                }
            }

            CommUtils.AssertNotNull(paymentDate, "无法根据AsOfDate[" + asOfDate + "]查询到对应的支付日。");

            var notes = m_dbAdapter.Dataset.GetNotes(projectId);

            //Create dataset
            var dataset = new Dataset();
            dataset.ProjectId = projectId;
            dataset.AsOfDate = asOfDate;
            dataset.PaymentDate = paymentDate;
            LogEditProduct(EditProductType.CreateProduct, projectId, "创建Dataset[" + projectId + "][" + asOfDate + "]", "");
            dataset = m_dbAdapter.Dataset.NewDataset(dataset);

            //Create note result
            var noteResult = new NoteResults();
            noteResult.ProjectId = projectId;
            noteResult.DatasetId = dataset.DatasetId;
            LogEditProduct(EditProductType.CreateProduct, projectId, "创建NoteResult[" + projectId + "][" + dataset.DatasetId + "]", "");
            noteResult = m_dbAdapter.Dataset.NewNoteResult(noteResult);

            //Create note data
            foreach (var note in notes)
            {
                var noteData = new NoteData();
                noteData.NoteId = note.NoteId;
                noteData.DatasetId = dataset.DatasetId;
                LogEditProduct(EditProductType.CreateProduct, projectId, "创建NoteData[" + note.NoteName + "][" + note.NoteId + "][" + dataset.DatasetId + "]", "");
                noteData = m_dbAdapter.Dataset.NewNoteData(noteData);
            }
        }

        [HttpPost]
        public ActionResult CheckPrepayDateByAsset(string projectGuid, string paymentDate, int assetId,
            string prepayDate, double money, string distributionType, string distributionDetail)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(money >= 0, "提前偿付金额不能为负数");
                var type = CommUtils.ParseEnum<PrepayDistrubutionType>(distributionType);
                var project = new ProjectLogicModel(CurrentUserName, projectGuid);
                var dataset = project.DealSchedule.GetByPaymentDay(DateUtils.ParseDigitDate(paymentDate)).Dataset;
                CommUtils.Assert(DateUtils.ParseDigitDate(prepayDate) >= dataset.DatasetSchedule.AsOfDateBegin
                    && DateUtils.ParseDigitDate(prepayDate) <= dataset.DatasetSchedule.AsOfDateEnd,
                    "提前偿付日期必须在当期时间范围（" + dataset.DatasetSchedule.AsOfDateBegin.ToShortDateString()
                    + "~" + dataset.DatasetSchedule.AsOfDateEnd.ToShortDateString() + "）内");
                
                var asset = dataset.Assets.Single(x => x.AssetId == assetId);
                if (asset.AmortizationType == AmortizationType.UserDefined)
                {
                    var amortization = dataset.AmortizationSchedule;
                    amortization.AddPrepayment(assetId, DateUtils.ParseDigitDate(prepayDate), money, type, distributionDetail);
                }

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult CalculatePrepayAmortizationByAsset(string projectGuid, string paymentDate, int assetId,
            string prepayDate, double money, string distributionType)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(money >= 0, "提前偿付金额不能为负数");
                var type = CommUtils.ParseEnum<PrepayDistrubutionType>(distributionType);
                CommUtils.Assert(type != PrepayDistrubutionType.Custom, "自定义模式下，无法预计算提前偿付数据。");

                var project = new ProjectLogicModel(CurrentUserName,projectGuid);
                var dataset = project.DealSchedule.GetByPaymentDay(DateUtils.ParseDigitDate(paymentDate)).Dataset;
                var asset = dataset.Assets.Single(x => x.AssetId == assetId);

                CommUtils.Assert(DateUtils.ParseDigitDate(prepayDate) >= dataset.DatasetSchedule.AsOfDateBegin
                    && DateUtils.ParseDigitDate(prepayDate) <= dataset.DatasetSchedule.AsOfDateEnd,
                    "提前偿付日期必须在当期时间范围（" + dataset.DatasetSchedule.AsOfDateBegin.ToShortDateString()
                    + "~" + dataset.DatasetSchedule.AsOfDateEnd.ToShortDateString() + "）内");

                if (asset.AmortizationType == AmortizationType.UserDefined)
                {
                    var amortization = dataset.AmortizationSchedule;
                    amortization.AddPrepayment(assetId, DateUtils.ParseDigitDate(prepayDate), money, type);
                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, amortization.SelectByAsset(assetId));
                    return ActionUtils.Success(viewModel);
                }

                //处理等额本金、等额本息、一次偿付类的提前偿付，需要下期模型存在
                if (dataset.Next == null)
                {
                    //下期模型不存在，自动生成下一期模型
                    CommUtils.AssertNotNull(dataset.DatasetSchedule.Next, "找不到下期模型封包日，提前偿付测算失败");

                    CreateDataset(project.Instance, dataset.DatasetSchedule.Next.AsOfDateBegin.ToString("yyyyMMdd"));

                    var assetModifier = new AssetModifier(CurrentUserName);
                    assetModifier.Load(project.Instance.ProjectId, dataset.DatasetSchedule.AsOfDateBegin);
                    assetModifier.GenerateNextDataset(project.Instance.ProjectId);
                }
                CommUtils.AssertNotNull(dataset.Next, "无法找到下期模型数据，提前偿付测算失败");

                if (IsEqualPmtOrEqualPrin(asset.AmortizationType))
                {
                    var curDatasetBasicAnalyticsData = NancyUtils.GetBasicAnalyticsData(project.Instance.ProjectId, null, dataset.DatasetSchedule.AsOfDateBegin);
                        
                    double curDatasetPrincipal = 0;
                    if (IsEqualPmtOrEqualPrin(asset.SecurityData.PaymentMethod))
                    {
                        //当期未发生过提前偿付，当期偿付金额是Nancy测算金额
                        curDatasetPrincipal = curDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                            .Single(x => x.AssetId == assetId && x.PaymentDate == dataset.DatasetSchedule.PaymentDate).Principal;
                    }
                    else
                    {
                        var nextDatasetAsset = dataset.Next.Assets.Single(x => x.AssetId == assetId);
                        CommUtils.Assert(IsEqualPmtOrEqualPrin(nextDatasetAsset.AmortizationType),
                            "第[" + dataset.Next.DatasetSchedule.PaymentDate.ToString("yyyyMMdd") + "]期偿付类型错误，提前偿付测算失败");

                        //当期发生过提前偿付，当期偿付金额是AmortizationSchedule中金额
                        var curAmortizationRecord = dataset.AmortizationSchedule.Single(x => x.ReductionDate == dataset.DatasetSchedule.PaymentDate);
                        //当期未发生本次提前偿付的偿付本金
                        curDatasetPrincipal = (double)curAmortizationRecord.ReductionAmount;
                    }

                    var sumFutureDatasetPrincipal = curDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                        .SelectByAsset(assetId)
                        .Where(x => x.PaymentDate > dataset.DatasetSchedule.PaymentDate)
                        .Sum(x => x.Principal);
                            
                    var records = new List<AmortizationScheduleRecord>();
                    //当期的新的本金偿付值是 根据等额本金/等额本息计算出的本金值 加上提前偿付金额
                    records.Add(new AmortizationScheduleRecord()
                    {
                        AssetId = assetId,
                        ReductionAmount = curDatasetPrincipal + money,
                        ReductionDate = dataset.DatasetSchedule.PaymentDate
                    });

                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);

                    //更新下期的本金期末余额（减去提前偿付金额），重新根据等额本金/等额本息计算本金
                    dataset.Next.CollateralCsv.UpdateCellValue(assetId, "PrincipalBalance", (sumFutureDatasetPrincipal - money).ToString());
                    dataset.Next.CollateralCsv.Save();

                    var nextDatasetBasicAnalyticsData = NancyUtils.GetBasicAnalyticsData(project.Instance.ProjectId, null, dataset.DatasetSchedule.Next.AsOfDateBegin);
                    var nextAssetCashflowItems = nextDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                        .SelectByAsset(assetId).Where(x => x.PaymentDate > dataset.DatasetSchedule.PaymentDate);

                    viewModel.AddRange(nextAssetCashflowItems);

                    //恢复下期的本金期末余额
                    dataset.Next.CollateralCsv.UpdateCellValue(assetId, "PrincipalBalance", sumFutureDatasetPrincipal.ToString());
                    dataset.Next.CollateralCsv.Save();

                    return ActionUtils.Success(viewModel);
                }
                else if (asset.AmortizationType == AmortizationType.SingleAmortization)
                {
                    var nextDatasetAsset = dataset.Next.Assets.Single(x => x.AssetId == assetId);
                    CommUtils.AssertEquals(nextDatasetAsset.SecurityData.PaymentMethod, ZEnums.EPaymentMethod.UNDEFINEDENUM,
                        "无法识别的偿付类型，提前偿付测算失败。");

                    var nextAmortizationRecords = dataset.Next.AmortizationSchedule.SelectByAsset(assetId);
                    CommUtils.AssertEquals(nextAmortizationRecords.Count, 0,
                        "第[" + dataset.Next.DatasetSchedule.PaymentDate.ToString("yyyyMMdd") + "]期已发生提前偿付, 提前偿付测算失败");

                    var records = dataset.AmortizationSchedule.SelectByAsset(assetId);
                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);
                    viewModel.Add(assetId, money, DateUtils.ParseDigitDate(prepayDate));

                    var sumPrepayAmount = records.Sum(x => x.ReductionAmount);
                    CommUtils.Assert(sumPrepayAmount <= asset.SecurityData.PrincipalBalance,
                        "提前偿付金额[" + sumPrepayAmount + "]大于剩余未偿付金额[" + asset.SecurityData.PrincipalBalance + "]");

                    viewModel.Add(assetId, asset.SecurityData.PrincipalBalance - sumPrepayAmount, asset.SecurityData.MaturityDate);
                    return ActionUtils.Success(viewModel);
                }

                return ActionUtils.Success("无法识别的偿付类型，projectGuid=["
                    + projectGuid + "] + paymentDate=[" + paymentDate + "] + assetId=[" + assetId + "]");
            });
        }

        [HttpPost]
        public ActionResult PrepayAmortizationByAsset(string projectGuid, string paymentDate, int assetId,
            string prepayDate, double money, string distributionType, string distributionDetail, string comment)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(money >= 0, "提前偿付金额不能为负数");
                var type = CommUtils.ParseEnum<PrepayDistrubutionType>(distributionType);

                var project = new ProjectLogicModel(CurrentUserName, projectGuid);
                var dataset = project.DealSchedule.GetByPaymentDay(DateUtils.ParseDigitDate(paymentDate)).Dataset;
                var asset = dataset.Assets.Single(x => x.AssetId == assetId);

                CommUtils.Assert(DateUtils.ParseDigitDate(prepayDate) >= dataset.DatasetSchedule.AsOfDateBegin
                    && DateUtils.ParseDigitDate(prepayDate) <= dataset.DatasetSchedule.AsOfDateEnd,
                    "提前偿付日期必须在当期时间范围（" + dataset.DatasetSchedule.AsOfDateBegin.ToShortDateString()
                    + "~" + dataset.DatasetSchedule.AsOfDateEnd.ToShortDateString() + "）内");

                if (asset.AmortizationType == AmortizationType.UserDefined)
                {
                    var amortization = dataset.AmortizationSchedule;
                    amortization.AddPrepayment(assetId, DateUtils.ParseDigitDate(prepayDate), money, type, distributionDetail);
                    amortization.Save();

                    var history = new AssetPrepaymentHistory();
                    history.ProjectId = project.Instance.ProjectId;
                    history.DatasetId = dataset.Instance.DatasetId;
                    history.AssetId = assetId;
                    history.PrepayAmount = money;
                    history.PrepayTime = DateUtils.ParseDigitDate(prepayDate);
                    history.DistributionType = distributionType;
                    history.DistributionDetail = distributionDetail;
                    history.Comment = comment;
                    history.TimeStamp = DateTime.Now;
                    history.TimeStampUserName = CurrentUserName;
                    m_dbAdapter.PaymentHistory.NewAssetPrepaymentHistory(history);

                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, amortization.SelectByAsset(assetId));
                    return ActionUtils.Success(viewModel);
                }

                CommUtils.Assert(type != PrepayDistrubutionType.Custom, "等额本金、等额本息、一次偿付类的资产不支持自定义偿付。");

                //处理等额本金、等额本息、一次偿付类的提前偿付，需要下期模型存在
                if (dataset.Next == null)
                {
                    //下期模型不存在，自动生成下一期模型
                    CommUtils.AssertNotNull(dataset.DatasetSchedule.Next, "找不到下期模型封包日，提前偿付测算失败");

                    CreateDataset(project.Instance, dataset.DatasetSchedule.Next.AsOfDateBegin.ToString("yyyyMMdd"));

                    var assetModifier = new AssetModifier(CurrentUserName);
                    assetModifier.Load(project.Instance.ProjectId, dataset.DatasetSchedule.AsOfDateBegin);
                    assetModifier.GenerateNextDataset(project.Instance.ProjectId);
                }
                CommUtils.AssertNotNull(dataset.Next, "无法找到下期模型数据，提前偿付测算失败");

                if (IsEqualPmtOrEqualPrin(asset.AmortizationType))
                {
                    var curDatasetBasicAnalyticsData = NancyUtils.GetBasicAnalyticsData(project.Instance.ProjectId, null, dataset.DatasetSchedule.AsOfDateBegin);

                    double curDatasetPrincipal = 0;
                    if (IsEqualPmtOrEqualPrin(asset.SecurityData.PaymentMethod))
                    {
                        //当期未发生过提前偿付，当期偿付金额是Nancy测算金额
                        curDatasetPrincipal = curDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                            .Single(x => x.AssetId == assetId && x.PaymentDate == dataset.DatasetSchedule.PaymentDate).Principal;
                    }
                    else
                    {
                        var nextDatasetAsset = dataset.Next.Assets.Single(x => x.AssetId == assetId);
                        CommUtils.Assert(IsEqualPmtOrEqualPrin(nextDatasetAsset.AmortizationType),
                            "第[" + dataset.Next.DatasetSchedule.PaymentDate.ToString("yyyyMMdd") + "]期偿付类型错误，提前偿付测算失败");

                        //当期发生过提前偿付，当期偿付金额是AmortizationSchedule中金额
                        var curAmortizationRecord = dataset.AmortizationSchedule.Single(x => x.ReductionDate == dataset.DatasetSchedule.PaymentDate);
                        //当期未发生本次提前偿付的偿付本金
                        curDatasetPrincipal = (double)curAmortizationRecord.ReductionAmount;
                    }

                    //保存AmortizationSchedule
                    var amortization = dataset.AmortizationSchedule;
                    amortization.Add(new AmortizationScheduleRecord{
                        AssetId = assetId,
                        ReductionAmount = money + curDatasetPrincipal,
                        ReductionDate = DateUtils.ParseDigitDate(prepayDate)
                    });
                    amortization.Save();

                    //更新PaymentMethod
                    dataset.CollateralCsv.UpdateCellValue(assetId, "PaymentMethod", "");
                    dataset.CollateralCsv.Save();

                    var sumFutureDatasetPrincipal = curDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                        .Where(x => x.AssetId == assetId && x.PaymentDate > dataset.DatasetSchedule.PaymentDate)
                        .Sum(x => x.Principal);

                    var records = new List<AmortizationScheduleRecord>();
                    //当期的新的本金偿付值是 根据等额本金/等额本息计算出的本金值 加上提前偿付金额
                    records.Add(new AmortizationScheduleRecord
                    {
                        AssetId = assetId,
                        ReductionAmount = curDatasetPrincipal + money,
                        ReductionDate = dataset.DatasetSchedule.PaymentDate
                    });

                    //更新下期的本金期末余额（减去提前偿付金额），重新根据等额本金/等额本息计算本金
                    dataset.Next.CollateralCsv.UpdateCellValue(assetId, "PrincipalBalance", (sumFutureDatasetPrincipal - money).ToString());
                    dataset.Next.CollateralCsv.Save();

                    var nextDatasetBasicAnalyticsData = NancyUtils.GetBasicAnalyticsData(project.Instance.ProjectId, null, dataset.DatasetSchedule.Next.AsOfDateBegin);
                    var nextAssetCashflowItems = nextDatasetBasicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems
                        .Where(x => x.AssetId == assetId && x.PaymentDate > dataset.DatasetSchedule.PaymentDate && x.Principal != 0)
                        .OrderBy(x => x.PaymentDate);

                    foreach (var item in nextAssetCashflowItems)
                    {
                        records.Add(new AmortizationScheduleRecord
                        {
                            AssetId = assetId,
                            ReductionAmount = item.Principal,
                            ReductionDate = item.PaymentDate
                        });
                    }

                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);
                    return ActionUtils.Success(viewModel);
                }
                else if (asset.AmortizationType == AmortizationType.SingleAmortization)
                {
                    var nextDatasetAsset = dataset.Next.Assets.Single(x => x.AssetId == assetId);
                    CommUtils.AssertEquals(nextDatasetAsset.SecurityData.PaymentMethod, ZEnums.EPaymentMethod.UNDEFINEDENUM,
                        "无法识别的偿付类型，提前偿付测算失败。");

                    var nextAmortizationRecords = dataset.Next.AmortizationSchedule.SelectByAsset(assetId);
                    CommUtils.AssertEquals(nextAmortizationRecords.Count, 0,
                        "第[" + dataset.Next.DatasetSchedule.PaymentDate.ToString("yyyyMMdd") + "]期已发生提前偿付, 提前偿付测算失败");

                    var records = dataset.AmortizationSchedule.SelectByAsset(assetId);
                    var prepayRecord = new AmortizationScheduleRecord
                    {
                        AssetId = assetId,
                        ReductionAmount = money,
                        ReductionDate = DateUtils.ParseDigitDate(prepayDate)
                    };

                    records.Add(prepayRecord);

                    var sumPrepayAmount = records.Sum(x => x.ReductionAmount);
                    CommUtils.Assert(sumPrepayAmount <= asset.SecurityData.PrincipalBalance,
                        "提前偿付金额[" + sumPrepayAmount + "]大于剩余未偿付金额[" + asset.SecurityData.PrincipalBalance + "]");

                    records.Add(new AmortizationScheduleRecord()
                    {
                        AssetId = assetId,
                        ReductionAmount = asset.SecurityData.PrincipalBalance - sumPrepayAmount,
                        ReductionDate = asset.SecurityData.MaturityDate
                    });

                    dataset.AmortizationSchedule.AddPrepayment(assetId, DateUtils.ParseDigitDate(prepayDate), money, type, distributionDetail);
                    dataset.AmortizationSchedule.Save();

                    dataset.Next.CollateralCsv.UpdateCellValue(assetId, "PrincipalBalance",
                        (asset.SecurityData.PrincipalBalance - sumPrepayAmount).ToString());
                    dataset.Next.CollateralCsv.Save();

                    var viewModel = new PrepayRecordListViewModel(dataset.DatasetSchedule, records);
                    return ActionUtils.Success(viewModel);
                }

                return ActionUtils.Success("无法识别的偿付类型，projectGuid=["
                    + projectGuid + "] + paymentDate=[" + paymentDate + "] + assetId=[" + assetId + "]");
            });
        }

        class PrepayHistory
        {
            public int AssetId { get; set; }
            public double PrepayAmount { get; set; }
            public string PrepayTime { get; set; }
            public string DistributionType { get; set; }
            public string DistributionDetail { get; set; }
            public string Comment { get; set; }
            public string TimeStamp { get; set; }
            public string TimeStampUserName { get; set; }
        }

        private string GetCnDistributionType(string distributionType)
        {
            var type = CommUtils.ParseEnum<PrepayDistrubutionType>(distributionType);
            switch (type)
            {
                case PrepayDistrubutionType.CurrentToLast:
                    return "时间最近优先";
                case PrepayDistrubutionType.LastToCurrent:
                    return "时间最远优先";
                case PrepayDistrubutionType.EqualRatio:
                    return "比例分摊";
                case PrepayDistrubutionType.Average:
                    return "平均摊还";
                case PrepayDistrubutionType.Custom:
                    return "自定义";
            }

            return "未定义";
        }

        [HttpPost]
        public ActionResult GetPrepaymentHistory(string projectGuid, string paymentDate, int assetId)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var dataset = m_dbAdapter.Dataset.GetDataset(project.ProjectId, DateUtils.ParseDigitDate(paymentDate));

                var histories = m_dbAdapter.PaymentHistory.GetAssetPrepaymentHistory(project.ProjectId, dataset.DatasetId, assetId);

                var result = new List<PrepayHistory>();

                Platform.UserProfile.Precache(histories.Select(x => x.TimeStampUserName));
                foreach (var history in histories)
                {
                    var prepayHistory = new PrepayHistory();
                    prepayHistory.AssetId = history.AssetId;
                    prepayHistory.PrepayAmount = double.Parse(history.PrepayAmount.ToString("n2"));
                    prepayHistory.PrepayTime = history.PrepayTime.ToString("yyyy-MM-dd");
                    prepayHistory.DistributionType = GetCnDistributionType(history.DistributionType);
                    prepayHistory.DistributionDetail = history.DistributionDetail;
                    prepayHistory.Comment = (history.Comment == null ? string.Empty : history.Comment);
                    prepayHistory.TimeStamp = history.TimeStamp.ToString();
                    prepayHistory.TimeStampUserName = Platform.UserProfile.GetDisplayRealNameAndUserName(history.TimeStampUserName);
                    result.Add(prepayHistory);
                }

                return ActionUtils.Success(result);
            });
        }

        private void LogEditProduct(EditProductType type, int? projectId, string description, string comment)
        {
            m_dbAdapter.Project.NewEditProductLog(type, projectId, description, comment);
        }

        private bool IsEqualPmtOrEqualPrin(AmortizationType amortizationType)
        {
            return amortizationType == AmortizationType.EqualPmt
                || amortizationType == AmortizationType.EqualPrin;
        }

        private bool IsEqualPmtOrEqualPrin(ZEnums.EPaymentMethod paymentMethod)
        {
            return paymentMethod == ZEnums.EPaymentMethod.EqualPmt
                || paymentMethod == ZEnums.EPaymentMethod.EqualPrin;
        }
    }
}