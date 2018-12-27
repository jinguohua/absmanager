using ChineseAbs.ABSManagement.Chart;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class PaymentHistoryController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string projectGuid)
        {
            //Load project info
            var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(projectIds);

            //Init view model
            var viewModel = new PaymentHistoryViewModel();

            Project project = null;
            if (string.IsNullOrEmpty(projectGuid))
            {
                if (projects.Count == 0)
                {
                    return View(viewModel);
                }

                project = projects[0];
            }
            else
            {
                project = projects.Single(x => x.ProjectGuid == projectGuid);
            }


            //Load project Model info
            if (project.ModelId > 0)
            {
                project.Model = m_dbAdapter.Project.GetModel(project.ModelId);
            }

            viewModel.CurrentProject = project;
            viewModel.Datasets = new List<DatasetViewModel>();
            viewModel.Projects = projects.ConvertAll(Toolkit.ConvertProject);

            //Load note info
            var notes = m_dbAdapter.Dataset.GetNotes(project.ProjectId);
            var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;
            var noteDict = Toolkit.GetNoteDictionary(project, notes, cnabsNotes);
            notes.ForEach(x => CommUtils.Assert(noteDict.ContainsKey(x.NoteId),
                "找不到Note信息，noteId={0}，noteName={1}，ProjectId={2}，SecurityCode={3}，ShortName={4}",
                x.NoteId, x.NoteName, x.ProjectId, x.SecurityCode, x.ShortName));

            viewModel.NoteInfos = notes.ConvertAll(x => noteDict[x.NoteId]);

            var logicModel = new ProjectLogicModel(CurrentUserName, project.ProjectGuid);

            DateTime[] paymentDates;
            try
            {
                paymentDates = logicModel.DealSchedule.DurationPeriods.Select(x => x.PaymentDate).ToArray();
            }
            catch(ApplicationException)
            {
                //没有偿付模型
                return View(viewModel);
            }
            var datasetSchedule = logicModel.DealSchedule.GetByPaymentDay(DateTime.Now);
            //Load dataset info
            while (datasetSchedule != null)
            {
                if (datasetSchedule.Dataset.Instance != null)
                {
                    if (datasetSchedule.Dataset.ReinvestmentCsv != null)
                    {
                        //不处理循环购买类产品的偿付历史相关
                        viewModel.HasReinvestmentInfo = true;
                        return View(viewModel);
                    }

                    var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(datasetSchedule.Dataset.Instance.DatasetId);
                    var datasetViewModel = Toolkit.GetDatasetViewModel(datasetSchedule.Dataset.Instance, paymentDates, noteDict, noteDatas);
                    viewModel.Datasets.Add(datasetViewModel);
                }

                datasetSchedule = datasetSchedule.Previous;
            }

            var allPaymentDays = new List<DateTime>();
            if (project.CnabsDealId.HasValue)
            {
                allPaymentDays = m_dbAdapter.Model.GetPaymentDates(project.CnabsDealId.Value);
            }
            else
            {
                if (logicModel.DealSchedule.Instanse != null)
                {
                    allPaymentDays = logicModel.DealSchedule.Instanse.PaymentDates.ToList();
                }
            }

            //Load asset cashflow data
            try
            {
                viewModel.AssetDatasets = new List<AssetDatasetViewModel>();
                viewModel.AssetDatasets = GetAssetCashflow(project.ProjectId, paymentDates, allPaymentDays);
            }
            catch (ApplicationException e)
            {
                viewModel.ExceptionMessage = e.Message + System.Environment.NewLine + e.StackTrace;
            }

            Dictionary<string, List<Vector>> series = new Dictionary<string, List<Vector>>();
            series["本金"] = new List<Vector>();
            series["利息"] = new List<Vector>();

            foreach (var assetDataset in viewModel.AssetDatasets)
            {
                var sumInfo = assetDataset.SumAsset;
                series["本金"].Add(new Vector(assetDataset.PaymentDay, decimal.Parse(sumInfo.Principal.ToString("n2"))));
                series["利息"].Add(new Vector(assetDataset.PaymentDay, decimal.Parse(sumInfo.Interest.ToString("n2"))));
            }

            var dataSeriesList = new List<DataSeries>();
            foreach (var key in series.Keys)
            {
                var ds = new DataSeries();
                ds.name = key;
                ds.data = series[key];
                dataSeriesList.Add(ds);
            }

            viewModel.AssetChartData = JsonConvert.SerializeObject(dataSeriesList);

            return View(viewModel);
        }

        private List<AssetDatasetViewModel> GetAssetCashflow(int projectId, DateTime[] paymentDates, List<DateTime> allPaymentDays)
        {
            var project = m_dbAdapter.Project.GetProjectById(projectId);

            var nowDate = DateTime.Today;

            List<AssetDatasetViewModel> viewModel = new List<AssetDatasetViewModel>();
            foreach (var paymentDate in paymentDates)
            {
                var dataset = m_dbAdapter.Dataset.GetDataset(projectId, paymentDate);
                if (dataset == null)
                {
                    continue;
                }

                var basicAnalyticsData = NancyUtils.GetBasicAnalyticsData(projectId, null, dataset.AsOfDate);

                AssetDatasetViewModel assetViewModel = new AssetDatasetViewModel();

                assetViewModel.Assets = new List<AssetViewModel>();
                var assetItems = basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems.Where(x => x.PaymentDate == paymentDate);
                foreach (var assetItem in assetItems)
                {
                    assetViewModel.Assets.Add(new AssetViewModel
                    {
                        AssetId = assetItem.AssetId,
                        Interest = (decimal)assetItem.Interest,
                        Principal = (decimal)assetItem.Principal,
                        Name = assetItem.AssetId.ToString()
                    });
                }

                assetViewModel.PaymentDay = paymentDate;
                assetViewModel.ReCalculateSumAsset();
                assetViewModel.Sequence = CommUtils.GetSequence(allPaymentDays, paymentDate);

                viewModel.Add(assetViewModel);

                if (paymentDate >= nowDate)
                {
                    break;
                }
            }

            return viewModel;
        }

        [HttpPost]
        public ActionResult UpdatePaymentHistoryAssetCashflow(string projectGuid, string paymentDate, int assetId,
            decimal principal, decimal interest, string comment)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrEmpty(comment), "[说明]不能为空");
                CommUtils.Assert(DateUtils.IsDigitDate(paymentDate), "Parse payment date failed [" + paymentDate + "].");
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var assetPaymentInfo = m_dbAdapter.PaymentHistory.GetCurrentAssetPaymentInfo(project.ProjectId, DateUtils.ParseDigitDate(paymentDate), assetId);

                var now = DateTime.Now;
                var history = new AssetCashflowHistory();
                history.ProjectId = project.ProjectId;
                history.PaymentDate = DateUtils.ParseDigitDate(paymentDate);
                history.AssetId = assetId;
                history.AssetName = assetPaymentInfo.AssetName;
                history.HandleType = AssetCashflowHandleType.UserEdit;
                history.Comment = comment;
                history.TimeStamp = now;
                history.TimeStampUserName = CurrentUserName;

                if (principal != assetPaymentInfo.Principal)
                {
                    history.FieldType = AssetCashflowFieldType.Pricipal;
                    history.FieldValue = principal.ToString();
                    m_dbAdapter.PaymentHistory.NewAssetCashflowHistory(history);
                }

                if (interest != assetPaymentInfo.Interest)
                {
                    history.FieldType = AssetCashflowFieldType.Interest;
                    history.FieldValue = interest.ToString();
                    m_dbAdapter.PaymentHistory.NewAssetCashflowHistory(history);
                }

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult GetProjectNameByGuid(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                return ActionUtils.Success(project.Name);
            });
        }
    }
}