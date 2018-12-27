using ChineseAbs.ABSManagement.Chart;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class PaymentHistorySecurityController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string projectGuid, string paymentDay)
        {
            var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
            var project = projectLogicModel.Instance;

            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(DateUtils.ParseDigitDate(paymentDay));
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance,
                "Dataset数据加载失败，projectGuid=[" + projectGuid + "] paymentDay=[" + paymentDay + "]");
            var dataset = datasetSchedule.Dataset.Instance;

            var notes = m_dbAdapter.Dataset.GetNotes(project.ProjectId);
            var cnabsNotes = projectLogicModel.Notes;
            var noteDict = Toolkit.GetNoteDictionary(project, notes, cnabsNotes);

            var dealSchedule = NancyUtils.GetDealSchedule(project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
            var datasetViewModel = Toolkit.GetDatasetViewModel(dataset, dealSchedule.PaymentDates, noteDict, noteDatas);

            
            var datasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(project, dataset.AsOfDate);
            VariablesHelper helper = new VariablesHelper(datasetFolder);
            var variables = helper.GetVariablesByDate(dataset.PaymentDate.Value);
            var rateResetRecords = InterestRateUtils.RateResetRecords(variables);

            //计算当期浮动利率
            datasetViewModel.NoteDatas.ForEach(x => x.CurrentCouponRate = InterestRateUtils.CalculateCurrentCouponRate(x.NoteInfo.CouponString, rateResetRecords));

            var viewModel = new CashflowDatasetViewModel();
            viewModel.Dataset = datasetViewModel;
            viewModel.ProjectGuid = projectGuid;

            //添加今天之前所有已上传模型的支付日
            var nowDate = DateTime.Today;
            viewModel.ValidPaymentDays = datasetSchedule.SelectPaymentDates(
                x => x.PaymentDate <= nowDate
                && x.Dataset != null && x.Dataset.Instance != null);

            if (project.CnabsDealId.HasValue)
            {
                viewModel.AllPaymentDays = m_dbAdapter.Model.GetPaymentDates(project.CnabsDealId.Value);
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

        #region Get chart data series

        [HttpPost]
        public ActionResult GetPaymentPercentChart(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                CommUtils.Assert(project.CnabsDealId.HasValue, "CNABS deal id is null.");

                DealService dealService = new DealService();
                var dealData = dealService.GetDealData(project.CnabsDealId.Value);
                CommUtils.Assert(dealData.ClosingDate.HasValue, "查找计息日失败");
                var closingDate = dealData.ClosingDate;

                //Load note info
                var notes = m_dbAdapter.Dataset.GetNotes(project.ProjectId);
                var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;
                var noteDict = Toolkit.GetNoteDictionary(project, notes, cnabsNotes);
                var noteInfos = notes.ConvertAll(x => noteDict[x.NoteId]);

                //Load dataset info
                var datasetViewModels = new List<DatasetViewModel>();
                var dealSchedule = NancyUtils.GetDealSchedule(project.ProjectId);
                var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(project.ProjectId);
                foreach (var dataset in datasets)
                {
                    var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
                    var datasetViewModel = Toolkit.GetDatasetViewModel(dataset, dealSchedule.PaymentDates, noteDict, noteDatas);
                    datasetViewModels.Add(datasetViewModel);
                }

                datasetViewModels.Reverse();

                var dataSeries = new DataSeries();
                dataSeries.name = "汇总";
                dataSeries.data = new List<Vector>();
                dataSeries.data.Add(new Vector(closingDate.Value, 100f));

                var sumNotional = noteInfos.Sum(x => x.Notional);
                var endingBalance = sumNotional;

                if (sumNotional.HasValue && sumNotional.Value != 0)
                {
                    foreach (var datasetViewModel in datasetViewModels)
                    {
                        var detail = datasetViewModel.SumPaymentDetail;
                        endingBalance -= detail.PrincipalPaid;
                        dataSeries.data.Add(new Vector(datasetViewModel.PaymentDay.Value, (endingBalance.Value / sumNotional.Value) * 100));
                    }
                }

                var dataSeriesList = new List<DataSeries>();
                dataSeriesList.Add(dataSeries);

                Dictionary<string, List<Vector>> series = new Dictionary<string, List<Vector>>();
                Dictionary<string, decimal> endingBalances = new Dictionary<string, decimal>();
                foreach (var noteInfo in noteInfos)
                {
                    var key = noteInfo.Name;
                    series[key] = new List<Vector>();
                    endingBalances[key] = noteInfo.Notional.Value;
                    series[key].Add(new Vector(closingDate.Value, 100d));
                }

                foreach (var datasetViewModel in datasetViewModels)
                {
                    foreach (var noteData in datasetViewModel.NoteDatas)
                    {
                        var key = noteData.NoteInfo.Name;
                        endingBalances[key] -= noteData.PaymentDetail.PrincipalPaid.Value;
                        series[key].Add(new Vector(datasetViewModel.PaymentDay.Value, (100 * endingBalances[key] / noteData.NoteInfo.Notional.Value)));
                    }
                }

                foreach (var key in series.Keys)
                {
                    var ds = new DataSeries();
                    ds.name = key;
                    ds.data = series[key];
                    dataSeriesList.Add(ds);
                }
                
                return ActionUtils.Success(dataSeriesList);
            });
        }

        [HttpPost]
        public ActionResult GetNotionalChart(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                var dataSeriesList = GetChartByDataset(projectGuid, paymentDay, x => (double)x.PrincipalPaid.Value);
                return ActionUtils.Success(dataSeriesList);
            });
        }

        [HttpPost]
        public ActionResult GetInterestChart(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                var dataSeriesList = GetChartByDataset(projectGuid, paymentDay, x => (double)x.InterestPaid.Value);
                return ActionUtils.Success(dataSeriesList);
            });
        }

        private List<DataSeries> GetChartByDataset(string projectGuid, string paymentDay, Func<PaymentDetail, double> selector)
        {
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(project.ProjectId);
            var subDatasets = datasets.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value.ToString("yyyyMMdd") == paymentDay).ToList();
            CommUtils.AssertEquals(subDatasets.Count, 1, "加载Dataset (project=" + project.Name + ", paymentDay=" + paymentDay + ") 失败");

            var dataset = subDatasets.Single();

            var notes = m_dbAdapter.Dataset.GetNotes(project.ProjectId);
            var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;
            var noteDict = Toolkit.GetNoteDictionary(project, notes, cnabsNotes);
            var noteInfos = notes.ConvertAll(x => noteDict[x.NoteId]);

            //Load dataset info
            var dealSchedule = NancyUtils.GetDealSchedule(project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
            var datasetViewModel = Toolkit.GetDatasetViewModel(dataset, dealSchedule.PaymentDates, noteDict, noteDatas);

            var dataSeriesList = new List<DataSeries>();
            foreach (var noteData in datasetViewModel.NoteDatas)
            {
                var ds = new DataSeries();
                ds.name = noteData.NoteInfo.Name;
                ds.data = new List<Vector>();
                ds.data.Add(new Vector(selector(noteData.PaymentDetail)));
                dataSeriesList.Add(ds);
            }

            return dataSeriesList;
        }

        #endregion
    }
}