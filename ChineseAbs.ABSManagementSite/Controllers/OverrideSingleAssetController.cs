using ChineseAbs.ABSManagement.LogicModels.OverrideSingleAsset;
using ChineseAbs.ABSManagement.Utils;
using System.Web.Mvc;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class OverrideSingleAssetController : BaseController
    {
        [HttpPost]
        public ActionResult OverridePrincipal(string projectGuid, string paymentDay,
            int assetId, double principal, string comment)
        {
            return ActionUtils.Json(() =>
            {
                var osa = GetOverrideSingleAsset(projectGuid, paymentDay, assetId);
                osa.OverridePrincipal(assetId, principal, comment);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult OverridePrincipalBalance(string projectGuid, string paymentDay,
            int assetId, double principalBalance, string comment)
        {
            return ActionUtils.Json(() =>
            {
                var osa = GetOverrideSingleAsset(projectGuid, paymentDay, assetId);
                osa.OverridePrincipalBalance(assetId, principalBalance, comment);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult OverrideInterest(string projectGuid, string paymentDay,
            int assetId, double interest, string comment)
        {
            return ActionUtils.Json(() =>
            {
                var osa = GetOverrideSingleAsset(projectGuid, paymentDay, assetId);
                osa.OverrideInterest(assetId, interest, comment);

                return ActionUtils.Success("");
            });
        }

        private OverrideSingleAssetLogicModel GetOverrideSingleAsset(
            string projectGuid, string paymentDay, int assetId)
        {
            CommUtils.Assert(DateUtils.IsDate(paymentDay), "无法解析[{0}]为日期", paymentDay);
            var paymentDate = DateUtils.Parse(paymentDay).Value;

            var project = Platform.GetProject(projectGuid);

            var dataset = project.DealSchedule.GetByPaymentDay(paymentDate).Dataset;
            var dealModel = dataset.DealModel;
            var osa = dealModel.OverrideSingleAsset;
            var assets = dataset.Assets;
            CommUtils.Assert(assets.Any(x => x.AssetId == assetId), "找不到资产[AssetId={0}]", assetId);
            return osa;
        }

        [HttpPost]
        public ActionResult ClearAllOverrideHistory(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(DateUtils.IsDate(paymentDay), "无法解析[{0}]为日期", paymentDay);
                var paymentDate = DateUtils.Parse(paymentDay).Value;

                var project = Platform.GetProject(projectGuid);

                var osa = project.DealSchedule.GetByPaymentDay(paymentDate)
                    .Dataset.DealModel.OverrideSingleAsset;

                osa.ClearAllOverrideHistory(project.Instance.ProjectId, paymentDate);
                return ActionUtils.Success("");
            });
        }
    }
}