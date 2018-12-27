using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DealModelController : BaseController
    {
        [HttpPost]
        public ActionResult GetAssetInfo(string projectGuid, string paymentDate, int assetId)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(paymentDate);
                var logicModel = Platform.GetProject(projectGuid);
                var datasetSchedule = logicModel.DealSchedule.GetByPaymentDay(date);
                var dataset = datasetSchedule.Dataset;

                CommUtils.Assert(dataset.HasDealModel, "找不到第{0}期模型", date);

                var acfTable = dataset.DealModel.AssetCashflowDt;
                var assets = dataset.Assets;
                Toolkit.AddAssetIdToRepeatedCNName(acfTable, assets);

                var assetList = assets.Where(x => x.AssetId == assetId).ToList();
                CommUtils.AssertEquals(assetList.Count, 1, "查找资产失败，AssetId={0}", assetId);

                var result = new CollateralAssetViewModel(assetList.Single());
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetAssetsInfo(string projectGuid, string asOfDate, int? page, int? pageSize)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(asOfDate);
                var logicModel = Platform.GetProject(projectGuid);
                var dataset = logicModel.DealSchedule.GetByAsOfDate(date).Dataset;

                var assets = dataset.Assets.Select(x => new CollateralAssetViewModel(x)).ToList();
                var pageInfo = new PageInfo(assets.Count, pageSize, page);

                var result = new
                {
                    pageInfo = pageInfo ,
                    assetsInfo = PageUtils.GetRange(assets, pageInfo)
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetInterestRateInfo(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(asOfDate);
                var logicModel = Platform.GetProject(projectGuid);
                var datasetSchedule = logicModel.DealSchedule.GetByAsOfDate(date);
                CommUtils.Assert(datasetSchedule.Dataset.HasDealModel,
                    "查找模型失败, projectGuid={0},asOfDate={1}", projectGuid, asOfDate);

                var rateResets = datasetSchedule.Dataset.Variables.GetInterestRateResets();

                var viewModel = new PrimeInterestRateViewModel();
                foreach (var rateReset in rateResets)
                {
                    var category = new InterestRateCategoryViewModel();
                    category.Code = rateReset.Code.ToString();
                    category.Name = CommUtils.ToCnString(rateReset.Code);
                    category.CurrentInterestRate = rateReset.CurrentInterestRate;
                    foreach (var resetRecord in rateReset.ResetRecords)
                    {
                        var adjuestment = new InterestRateViewModel();
                        adjuestment.AdjustDate = resetRecord.Date;
                        adjuestment.InterestRate = resetRecord.InterestRate;
                        category.Adjustments.Add(adjuestment);
                    }

                    viewModel.Categories.Add(category);
                }

                return ActionUtils.Success(viewModel);
            });
        }
    }
}