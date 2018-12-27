using ChineseAbs.ABSManagement.LogicModels.OverrideSingleAsset;
using ChineseAbs.ABSManagement.Object;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.CalcService.Data.NancyData.Cashflows;
using CNABS.Mgr.Deal.Model;
using CNABS.Mgr.Deal.Model.Result;
using CNABS.Mgr.Deal.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DealController : BaseController
    {
        [HttpPost]
        public ActionResult UpdateDealModelSetting(string projectGuid, bool enablePredict)
        {
            return ActionUtils.Json(() =>
            {
                var project = Platform.GetProject(projectGuid);
                project.EnablePredictMode = enablePredict;
                ABSDealUtils.UpdateDatasetByPredictModel(project);

                return ActionUtils.Success(null);
            });
        }

        [HttpPost]
        public ActionResult GetAssetCashFlowDataDetail(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                if (string.IsNullOrWhiteSpace(paymentDay))
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = "请选择偿付期"
                    };
                    return ActionUtils.Success(errorResult);
                }

                DateTime paymentDate;
                if (!DateTime.TryParse(paymentDay, out paymentDate))
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = "偿付期错误，请刷新页面后重试"
                    };
                    return ActionUtils.Success(errorResult);
                }

                var result = GetAssetCashflowTableDetail(projectGuid, paymentDate);

                return ActionUtils.Success(result);
            });
        }

        private object GetAssetCashflowTableDetail(string projectGuid, DateTime paymentDate)
        {
            var dictDeterminationDates = new Dictionary<DateTime, DateTime>();
            var allAssetDetail = new NancyBasicAssetCashflow();

            var projectLogicModel = Platform.GetProject(projectGuid);

            ABSDeal absDeal = null;
            try
            {
                var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
                paymentDate = datasetSchedule.PaymentDate;

                CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(paymentDate));
                CommUtils.Assert(datasetSchedule.Dataset.HasDealModel, "找不到第{0}期模型", Toolkit.DateToString(paymentDate));

                absDeal = new ABSDeal(datasetSchedule.Dataset.DealModel);

                dictDeterminationDates = ABSDealUtils.GetDeterminationDatesByPaymentDates(absDeal);

                //获取当期所有的资产明细
                var asOfDate = datasetSchedule.Dataset.Instance.AsOfDate;
                allAssetDetail = NancyUtils.GetUnaggregateAssetCashflowByPath(projectLogicModel.Instance.ProjectId, asOfDate);
            }
            catch (ApplicationException ae)
            {
                var errorResult = new
                {
                    isError = true,
                    errorMessage = ae.Message,
                    stackTrace = ae.StackTrace,
                };
                return errorResult;
            }

            var dictAllAssetDetail = allAssetDetail.BasicAssetCashflowItems.ToList().GroupBy(x => x.AssetId).ToDictionary(x => x.Key, y => y.ToList());

            var dataSetResult = new Dictionary<int, List<AssetCashflowData>>();
            var dictRowSpan = new Dictionary<int, List<int>>();

            var osaDict = new Dictionary<DateTime, OverrideSingleAssetLogicModel>();

            var osaPrincipalRecords = m_dbAdapter.OverrideSingleAssetPrincipal.GetByProject(projectLogicModel.Instance.ProjectId);
            var osaPrincipalBalanceRecords = m_dbAdapter.OverrideSingleAssetPrincipalBalance.GetByProject(projectLogicModel.Instance.ProjectId);
            var osaInterestRecords = m_dbAdapter.OverrideSingleAssetInterest.GetByProject(projectLogicModel.Instance.ProjectId);

            //获取每笔资产的明细
            dictAllAssetDetail.Keys.ToList().ForEach(x =>
            {
                var currAssetDetailList = dictAllAssetDetail[x];
                dictRowSpan[x] = new List<int>();

                var dictCurrAssetList = new Dictionary<DateTime, List<NancyBasicAssetCashflowItem>>();
                var data = new List<AssetCashflowData>();

                var prevDateTime = DateTime.Parse("1753-01-02");

                foreach (var currPaymentDate in dictDeterminationDates.Keys)
                {
                    var currCompareTime = dictDeterminationDates[currPaymentDate];
                    dictCurrAssetList[currCompareTime] = currAssetDetailList.Where(item => prevDateTime < item.PaymentDate && item.PaymentDate <= currCompareTime).ToList();

                    var currAssetList = dictCurrAssetList[currCompareTime];

                    var assetId = x;

                    OverrideSingleAssetLogicModel osa = null;
                    if (osaDict.ContainsKey(currPaymentDate))
                    {
                        osa = osaDict[currPaymentDate];
                    }
                    else
                    {
                        osa = new OverrideSingleAssetLogicModel(projectLogicModel, currPaymentDate,
                            osaPrincipalRecords, osaInterestRecords, osaPrincipalBalanceRecords);
                        osaDict[currPaymentDate] = osa;
                    }

                    var hasOsa = osa.HasOverrideRecords(assetId);

                    if (currAssetList.Count > 0)
                    {
                        dictRowSpan[x].Add(hasOsa ? 1 : (currAssetList.Count + 1));
                        var rowName = Toolkit.DateToString(currCompareTime);

                        for (int i = 0; i < currAssetList.Count; i++)
                        {
                            var ditail = currAssetList[i];

                            var currACFRow = new AssetCashflowData();
                            currACFRow.RowName = rowName;

                            currACFRow.AddValue("资产回款日", Toolkit.DateToString(ditail.PaymentDate));
                            currACFRow.AddValue("利息", ditail.Interest.ToString("n2"));
                            currACFRow.AddValue("本金", ditail.Principal.ToString("n2"));
                            currACFRow.AddValue("合计", (ditail.Interest + ditail.Principal).ToString("n2"));
                            currACFRow.AddValue("剩余本金", ditail.Performing.ToString("n2"));
                            currACFRow.AddValue("损失", ditail.Loss.ToString("n2"));
                            currACFRow.AddValue("违约", ditail.Defaulted.ToString("n2"));

                            if (!hasOsa)
                            {
                                data.Add(currACFRow);
                            }
                        }

                        var total = new AssetCashflowData();
                        total.RowName = rowName;
                        var interest = currAssetList.Sum(value => value.Interest);
                        var principal = currAssetList.Sum(value => value.Principal);
                        var performing = currAssetList.Last().Performing;
                        if (osa.GetInterest(assetId) != null)
                        {
                            interest = osa.GetInterest(assetId).Interest;
                        }

                        if (osa.GetPrincipal(assetId) != null)
                        {
                            principal = osa.GetPrincipal(assetId).Principal;
                        }

                        if (osa.GetPrincipalBalance(assetId) != null)
                        {
                            performing = osa.GetPrincipalBalance(assetId).PrincipalBalance;
                        }

                        total.AddValue("资产回款日", "总计");
                        total.AddValue("利息", interest.ToString("n2"));
                        total.AddValue("本金", principal.ToString("n2"));
                        total.AddValue("合计", (interest + principal).ToString("n2"));
                        total.AddValue("剩余本金", performing.ToString("n2"));
                        total.AddValue("损失", currAssetList.Sum(value => value.Loss).ToString("n2"));
                        total.AddValue("违约", currAssetList.Sum(value => value.Defaulted).ToString("n2"));
                        data.Add(total);
                    }
                    prevDateTime = currCompareTime;
                }
                dataSetResult[x] = (data);
            });

            //获取所有的资产的中文名
            var dictAssetIdCNName = absDeal.Assets.GetAssetIdNameMap();

            var detailTableHeader = new List<string>() { "归集日", "资产回款日", "利息", "本金", "合计", "剩余本金", "损失", "违约" };
            return new
            {
                tableHeader = detailTableHeader,
                assetIdCNName = dictAllAssetDetail.Keys.ToList().ConvertAll(x => new
                {
                    tableKey = x,
                    tableValues = dictAssetIdCNName[x]
                }),
                dataSets = dataSetResult.Keys.ToList().ConvertAll(x => new
                {
                    tableKey = x,
                    tableValues = dataSetResult[x],
                    rowSpanList = dictRowSpan[x]
                }),
                isError = false
            };
        }

        [HttpPost]
        public ActionResult GetCurrPeriodAssetCashflowTable(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                if (string.IsNullOrWhiteSpace(paymentDay))
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = "请选择偿付期"
                    };
                    return ActionUtils.Success(errorResult);
                }

                DateTime paymentDate;
                if (!DateTime.TryParse(paymentDay, out paymentDate))
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = "偿付期错误，请刷新页面后重试"
                    };
                    return ActionUtils.Success(errorResult);
                }

                var result = GetCurrPeriodAssetCashflowTable(projectGuid, paymentDate);
                return ActionUtils.Success(result);
            });
        }

        public object GetCurrPeriodAssetCashflowTable(string projectGuid, DateTime paymentDate)
        {
            try
            {
                var projectLogicModel = Platform.GetProject(projectGuid);
                var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
                CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));

                paymentDate = datasetSchedule.PaymentDate;

                var osa = datasetSchedule.Dataset.DealModel.OverrideSingleAsset;

                var dealModel = datasetSchedule.Dataset.DealModel;
                var absDeal = new ABSDeal(dealModel.YmlFolder, dealModel.DsFolder);

                //获取预测的资产池当期本金与利息
                var oldCurrPeriodAcfDataset = absDeal.Result.AcfResult.Dataset.Single(x => x.PaymentDay == paymentDate);
                var predictInterePrincipal = new CurrPerdictPrincipalInterest();
                predictInterePrincipal.CurrPerdictInterest = double.Parse(oldCurrPeriodAcfDataset.Sum.Interest.ToString("n2"));
                predictInterePrincipal.CurrPerdictPrincipal = double.Parse(oldCurrPeriodAcfDataset.Sum.Principal.ToString("n2"));

                //获取覆盖资产池的本金、利息
                var assetCashflowVariable = m_dbAdapter.AssetCashflowVariable.GetByProjectIdPaymentDay(projectLogicModel.Instance.ProjectId, paymentDate);

                var assetIdNameMap = absDeal.Assets.GetAssetIdNameMap();
                var osaResults = absDeal.Result.AcfResult.MergeOsa(datasetSchedule.Dataset.DealModel.OverrideSingleAsset);
                var currPeriodDataTable = absDeal.Result.AcfResult.ExtractCurrPeriodAssetSummaryTable(paymentDate);
                var currPeriodAcfDataset = absDeal.Result.AcfResult.Dataset.Single(x => x.PaymentDay == paymentDate);

                //获取当前表格的本金、利息
                var currInterePrincipal = new CurrPerdictPrincipalInterest();
                currInterePrincipal.CurrPerdictInterest = double.Parse(currPeriodAcfDataset.Sum.Interest.ToString("n2"));
                currInterePrincipal.CurrPerdictPrincipal = double.Parse(currPeriodAcfDataset.Sum.Principal.ToString("n2"));

                List<int> PrevOsaList = new List<int>();
                if (datasetSchedule.Previous != null)
                {
                    if (datasetSchedule.Previous.Dataset.HasDealModel)
                    {
                        var prevOsa = datasetSchedule.Previous.Dataset.DealModel.OverrideSingleAsset;

                        datasetSchedule.Dataset.Assets.ForEach(x =>
                        {
                            if (prevOsa.GetPrincipal(x.AssetId) != null
                                || prevOsa.GetPrincipalBalance(x.AssetId) != null
                                || prevOsa.GetInterest(x.AssetId) != null)
                            {
                                PrevOsaList.Add(x.AssetId);
                            }
                        });
                    }
                }

                var columnNames = new List<string>() { "资产", "利息", "本金", "剩余本金", "损失", "违约", "合计", "费用" };
                var result = new
                {
                    paymentDate = Toolkit.DateToString(paymentDate),
                    currPeriodTableHeader = columnNames,
                    isError = false,
                    handsonData = currPeriodDataTable.ToHandson(),
                    overrideSingleAssetData = osaResults,
                    overrideSingleAssetDataLast = PrevOsaList,
                    assetIdNameMap = assetIdNameMap.Keys.ToList()
                        .Select(x => new
                        {
                            assetId = x,
                            assetName = assetIdNameMap[x]
                        }),
                    userCustomCashflow = absDeal.Info.UseCustomCashflow,
                    predictInterePrincipal = predictInterePrincipal,
                    currInterePrincipal = currInterePrincipal,
                    assetCashflowVariable = assetCashflowVariable == null ? null : new
                    {
                        interest = double.Parse(assetCashflowVariable.InterestCollection.ToString("n2")),
                        principal = double.Parse(assetCashflowVariable.PricipalCollection.ToString("n2")),
                        enableOverride = assetCashflowVariable.EnableOverride
                    },
                };
                return result;
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

        [HttpPost]
        public ActionResult GetCashflowFile(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(paymentDay), "请选择偿付期");

                DateTime paymentDate;
                CommUtils.Assert(DateTime.TryParse(paymentDay, out paymentDate), "偿付期错误，请刷新页面后重试");

                var cashflowTable = GetCashFlowTable(projectGuid, paymentDate);
                var ms = ExcelUtils.ToExcelMemoryStream(cashflowTable, "CashflowTable.xlsx", CurrentUserName);

                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "CashflowTable.xlsx", ms);
                return ActionUtils.Success(resource.Guid);
            });
        }

        private DataTable GetCashFlowTable(string projectGuid, DateTime paymentDate)
        {
            var projectLogicModel = Platform.GetProject(projectGuid);
            var project = projectLogicModel.Instance;

            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(paymentDate));

            //获取预测的资产池当期本金与利息
            var acfTable = datasetSchedule.Dataset.DealModel.AssetCashflowDt;
            for (int i = acfTable.Columns.Count - 1; i >= 0; i--)
            {
                var c = acfTable.Columns[i];
                DateTime date;
                if (DateTime.TryParse(c.ColumnName, out date))
                {
                    if (date < paymentDate)
                    {
                        acfTable.Columns.Remove(c);
                    }
                }
            }

            var assetViewModel = Toolkit.GetAssetCashflow(acfTable, paymentDate);

            //判断本金与利息是否被覆盖
            var assetCashflowVariable = m_dbAdapter.AssetCashflowVariable.GetByProjectIdPaymentDay(project.ProjectId, paymentDate);
            var predictInterestCollection = double.Parse(assetViewModel.TotalCurrentInterestCollection.ToString("n2"));
            var predictPricipalCollection = double.Parse(assetViewModel.TotalCurrentPrinCollection.ToString("n2"));

            var cashflowDt = ABSDealUtils.GetCashflowDt(datasetSchedule, assetCashflowVariable,
                predictInterestCollection, predictPricipalCollection,
                out double currInterest, out double currPrincipal);

            cashflowDt = ABSDealUtils.CleanAndTranslateCashflowTable(cashflowDt);

            return cashflowDt;
        }
    }
}