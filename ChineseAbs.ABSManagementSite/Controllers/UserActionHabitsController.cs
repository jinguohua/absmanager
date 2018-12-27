using ChineseAbs.ABSManagement.Utils;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class UserActionHabitsController : BaseController
    {
        [HttpPost]
        public ActionResult Load()
        {
            return ActionUtils.Json(() =>
            {
                var editAcf = Platform.User.ActionHabits.EditAssetCashflow;
                var result = new
                {
                    EditAssetCashflow = new
                    {
                        EditPrincipal = new
                        {
                            AutoSyncPrincipalBalance = editAcf.EditPrincipal.AutoSyncPrincipalBalance,
                        },
                        EditPrincipalBalance = new
                        {
                            AutoSyncPrincipal = editAcf.EditPrincipalBalance.AutoSyncPrincipal,
                        }
                    }
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult SaveEditAssetCashflowSetting(string autoSyncPrincipalBalance, string autoSyncPrincipal)
        {
            return ActionUtils.Json(() =>
            {
                var editAcf = Platform.User.ActionHabits.EditAssetCashflow;
                editAcf.EditPrincipal.AutoSyncPrincipalBalance = CommUtils.ParseBool(autoSyncPrincipalBalance);
                editAcf.EditPrincipalBalance.AutoSyncPrincipal = CommUtils.ParseBool(autoSyncPrincipal);
                Platform.User.ActionHabits.Save();
                return ActionUtils.Success(1);
            });
        }
    }
}