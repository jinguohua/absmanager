using ChineseAbs.ABSManagement.Utils;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class EnterpriseUserController : BaseController
    {
        [HttpPost]
        public ActionResult GetAllAuthedUsers()
        {
            return ActionUtils.Json(() =>
            {
                var accounts = m_dbAdapter.Authority.GetAllAuthedAccount();
                var result = accounts.OrderBy(x => x.RealName, CommUtils.StringComparerCN)
                    .ThenBy(x => x.UserName)
                    .Select(x => new {
                        userName = x.UserName,
                        realName = x.RealName,
                        isCurrentUser = IsCurrentUser(x.UserName)
                    });

                return ActionUtils.Success(result);
            });
        }
    }
}