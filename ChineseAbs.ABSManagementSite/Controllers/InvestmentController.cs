using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class InvestmentController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string projectGuid)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateInvestment(string projectGuid, string name, double money,
           string yieldDue, string startTime, string endTime, string description)
        {
            return ActionUtils.Json(() =>
            {
                ValidateUtils.Name(name, "投资标的");
                CommUtils.AssertHasContent(startTime, "[开始时间]不能为空");
                CommUtils.AssertHasContent(endTime, "[到期时间]不能为空");
                CommUtils.Assert(money <= 1000000000000, "[投资金额]不能大于10,000亿元");
                CommUtils.Assert(money > 0, "[投资金额]必须大于0元");

                var valStartTime = DateTime.Parse(startTime);
                var valEndTime = DateTime.Parse(endTime);
                CommUtils.Assert(DateTime.Compare(valEndTime, valStartTime) > 0, "[到期时间]必须大于[开始时间]");

                var investment = new Investment();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                investment.ProjectId = project.ProjectId;
                investment.Name = name;
                investment.Money = money;
                investment.StartTime = valStartTime;
                investment.EndTime = valEndTime;
                investment.Description = description;
                investment.YieldDue = null;

                if (!string.IsNullOrWhiteSpace(yieldDue))
                {
                    var percentValue = 0.0;
                    if (yieldDue.Contains('%'))
                    {
                        CommUtils.Assert(double.TryParse(yieldDue.Substring(0, yieldDue.Length - 1), out percentValue), "预计收益率必须为数字");
                    }
                    else 
                    {
                        CommUtils.Assert(double.TryParse(yieldDue, out percentValue), "预计收益率必须为数字");
                    }
                    CommUtils.Assert(percentValue >= 365.00 * (-1) / (valEndTime - valStartTime).TotalDays, "预计收益率过低，请重新填写");
                    investment.YieldDue = percentValue / 100;
                }

                m_dbAdapter.Investment.NewInvestment(investment);
                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult GetInvestments(string projectGuid, int? page, int? pageSize)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var investments = m_dbAdapter.Investment.GetInvestmentsByProjectId(project.ProjectId);
                var investmentIds = investments.ConvertAll(x => x.Id).ToList();
                var investmentsOfPage = m_dbAdapter.Investment.GetInvestments(page ?? 1, pageSize ?? 10, investmentIds);
                var result = new
                {
                    Investments = investmentsOfPage.Items.ConvertAll(x => new
                    {
                        guid = x.Guid,
                        name = x.Name,
                        description = Toolkit.ToString(x.Description),
                        money = x.Money.ToString("n2"),
                        gains = (x.Gains.HasValue ? x.Gains.Value.ToString("n2") : "-"),
                        yield = (x.Yield.HasValue ? x.Yield.Value.ToString("P") : "-"),
                        yieldDue = (x.YieldDue.HasValue ? CommUtils.Percent(x.YieldDue.Value, 1) : "-"),
                        gainsDue = x.YieldDue.HasValue ? InterestRateUtils.CalculateGains(x.YieldDue.Value, x.Money, x.EndTime, x.StartTime).ToString("n2") : "-",
                        startTime = Toolkit.DateToString(x.StartTime),
                        endTime = Toolkit.DateToString(x.EndTime),
                        accountingTime = Toolkit.DateToString(x.AccountingTime),
                        status = x.Gains.HasValue ? "Finished" : "Running",
                        reminderInfo = m_dbAdapter.MessageReminding.GetResultByUid(x.Guid),
                    }).ToList(),
                    StatisticInfo = new
                    {
                        totalMoney = investments.Sum(x => x.Money).ToString("n2"),
                        totalGains = investments.Sum(x => x.Gains ?? 0).ToString("n2"),
                        totalCount = investments.Count.ToString("n0"),
                        totalFinished = investments.Sum(x => x.Gains.HasValue ? 1 : 0).ToString("n0"),
                        totalRunning = investments.Sum(x => x.Gains.HasValue ? 0 : 1).ToString("n0")
                    }
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult ClearingInvestment(string investmentGuid, string endTime, double gains, string accountingTime)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(investmentGuid, "Investment guid不能为空");
                CommUtils.AssertHasContent(endTime, "[到期时间]不能为空");
                CommUtils.AssertHasContent(accountingTime, "[到账时间]不能为空");
                CommUtils.Assert(gains <= 1000000000000, "[收益金额]不能大于10,000亿元");
                CommUtils.Assert(gains >= -1000000000000, "[收益金额]不能小于-10,000亿元");

                var valEndTime = DateTime.Parse(endTime);
                var valAccountingTime = DateTime.Parse(accountingTime);
                CommUtils.Assert(DateTime.Compare(valAccountingTime, valEndTime) >= 0, "[到账时间]不能小于[到期时间]");

                var investment = m_dbAdapter.Investment.GetInvestment(investmentGuid);
                CommUtils.Assert(DateTime.Compare(valEndTime, investment.StartTime) > 0, "[到期时间]必须大于[开始时间]");
                CommUtils.Assert(!(gains < 0 && System.Math.Abs(gains) > investment.Money), "[收益金额]不能亏损超过[投资金额]");

                investment.Gains = gains;
                investment.EndTime = valEndTime;
                investment.AccountingTime = valAccountingTime;
                investment.Yield = InterestRateUtils.CalculateYield(investment.Gains.Value, investment.Money, investment.EndTime, investment.StartTime);
                var result = m_dbAdapter.Investment.UpdateInvestment(investment);

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult RemoveInvestment(string investmentGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(investmentGuid, "Investment guid不能为空");

                var investment = m_dbAdapter.Investment.GetInvestment(investmentGuid);
                var result = m_dbAdapter.Investment.RemoveInvestment(investment);
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult ModifyInvestment(string investmentGuid, string name, string description,
            double money, string yieldDue, double? gains, string startTime, string endTime, string accountingTime)
        {
            return ActionUtils.Json(() =>
            {
                ValidateUtils.Name(name, "投资标的");
                CommUtils.AssertHasContent(startTime, "[开始时间]不能为空");
                CommUtils.AssertHasContent(endTime, "[到期时间]不能为空");
                CommUtils.Assert(money <= 1000000000000, "[投资金额]不能大于10,000亿元");
                CommUtils.Assert(money > 0, "[投资金额]必须大于0元");

                var valStartTime = DateTime.Parse(startTime);
                var valEndTime = DateTime.Parse(endTime);
                CommUtils.Assert(DateTime.Compare(valEndTime, valStartTime) > 0, "[到期时间]必须大于[开始时间]");

                var investment = m_dbAdapter.Investment.GetInvestment(investmentGuid);
                investment.Name = name;
                investment.Description = description;
                investment.Money = money;
                investment.StartTime = valStartTime;
                investment.EndTime = valEndTime;
                investment.YieldDue = null;

                if (investment.Gains.HasValue)
                {
                    CommUtils.AssertNotNull(gains, "[收益金额]不能为空");
                    CommUtils.AssertHasContent(accountingTime, "[到账时间]不能为空");
                    CommUtils.Assert(gains <= 1000000000000, "[收益金额]不能大于10,000亿元");
                    CommUtils.Assert(gains >= -1000000000000, "[收益金额]不能小于-10,000亿元");
                    CommUtils.Assert(!(gains < 0 && System.Math.Abs(gains.Value) > investment.Money), "[收益金额]不能亏损超过[投资金额]");

                    var valAccountingTime = DateTime.Parse(accountingTime);
                    CommUtils.Assert(DateTime.Compare(valAccountingTime, valEndTime) >= 0, "[到账时间]不能小于[到期时间]");

                    investment.AccountingTime = valAccountingTime;
                    investment.Gains = gains;
                    investment.Yield = InterestRateUtils.CalculateYield(investment.Gains.Value, investment.Money, investment.EndTime, investment.StartTime);
                }

                if (!string.IsNullOrWhiteSpace(yieldDue) && yieldDue != "-")
                {
                    var percentValue = 0.0;
                    if (yieldDue.Contains('%'))
                    {
                        CommUtils.Assert(double.TryParse(yieldDue.Substring(0, yieldDue.Length - 1), out percentValue), "预计收益率必须为数字");
                    }
                    else
                    {
                        CommUtils.Assert(double.TryParse(yieldDue, out percentValue), "预计收益率必须为数字");
                    }
                    CommUtils.Assert(percentValue >= 365.00 * (-1) / (valEndTime - valStartTime).TotalDays, "预计收益率过低，请重新填写");
                    investment.YieldDue = percentValue / 100;
                }

                var result = m_dbAdapter.Investment.UpdateInvestment(investment);
                return ActionUtils.Success(result);
            });
        }




    }
}