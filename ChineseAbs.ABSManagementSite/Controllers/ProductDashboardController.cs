using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension;
using ChineseAbs.ABSManagementSite.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ChineseAbs.ABSManagement.Object;
using ChineseAbs.ABSManagement.LogicModels.DealModel;
using System.Data;
using CNABS.Mgr.Deal.Model;
using CNABS.Mgr.Deal.Utils;
using ChineseAbs.ABSManagement.Models.DealModel;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ProductDashboardController : BaseController
    {
        public ActionResult Index()
        {
             return View();
        }

        [HttpPost]
        public ActionResult GetAssetCashflowDataTable(string projectGuid, string paymentDay)
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
                if (!DateTime.TryParse(paymentDay,out paymentDate))
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = "偿付期错误，请刷新页面后重试"
                    };
                    return ActionUtils.Success(errorResult);
                }

                var handler = new TaskExAssetCashflow(CurrentUserName, null);
                var viewModel = new AssetCashflowStatisticInfo();
                var result = handler.GetACFTableByProject(viewModel, projectGuid, paymentDate);

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetCashflowDataTable(string projectGuid, string paymentDay)
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

                try
                {
                    var viewModel = new CashflowViewModel();
                    var result = GetCFTableByProject(viewModel, projectGuid, paymentDate);

                    return ActionUtils.Success(result);
                }
                catch (ApplicationException ae)
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = ae.Message
                    };
                    return ActionUtils.Success(errorResult);
                }
            });
        }

        private object GetCFTableByProject(CashflowViewModel viewModel, string projectGuid, DateTime paymentDate)
        {
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

            var projectLogicModel = Platform.GetProject(projectGuid);
            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));

            //同步数据库中参数到模型文件
            var variableDateset = m_dbAdapter.CashflowVariable.GetByPaymentDay(project.ProjectId, paymentDate);
            UpdateVariablesFromCurrPeriodDBToModel(datasetSchedule.Dataset, variableDateset, project.ProjectId, paymentDate);

            var cashflowTableViewModel = new CashflowDetailTableViewModel();
            cashflowTableViewModel.CurrentPaymentDate = Toolkit.DateToString(datasetSchedule.PaymentDate);

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
            var cashflowViewModel = new CashflowViewModel();
            cashflowViewModel.PredictInterestCollection = double.Parse(assetViewModel.TotalCurrentInterestCollection.ToString("n2"));
            cashflowViewModel.PredictPricipalCollection = double.Parse(assetViewModel.TotalCurrentPrinCollection.ToString("n2"));
            cashflowViewModel.OverridableVariables = SelectOverridableVariables(datasetSchedule.Dataset);

            var cashflowDt = ABSDealUtils.GetCashflowDt(datasetSchedule, assetCashflowVariable,
                cashflowViewModel.PredictInterestCollection, cashflowViewModel.PredictPricipalCollection,
                out double currInterest, out double currPrincipal);

            cashflowViewModel.CurrentInterestCollection = currInterest;
            cashflowViewModel.CurrentPricipalCollection = currPrincipal;

            cashflowDt = ABSDealUtils.CleanAndTranslateCashflowTable(cashflowDt);

            var columnNames = new List<string>();
            for (int i = 0; i < cashflowDt.Columns.Count; ++i)
            {
                var columnName = cashflowDt.Columns[i].ColumnName.ToString();
                columnNames.Add(columnName);
            }

            viewModel.ColHeader = columnNames;
            string testFailRemind = null;

            var rowsNum = cashflowDt.Rows.Count;
            if (cashflowDt != null && rowsNum > 0)
            {
                var equalRowsNumber = 0;
                var prevAsset = cashflowDt.Rows[0][0].ToString();

                for (int i = 0; i < rowsNum; i++)
                {
                    DataRow dr = cashflowDt.Rows[i];

                    var row = dr.ItemArray.ToList().ConvertAll(x => x.ToString());
                    viewModel.CashflowDataResult.Add(row);
                    if (testFailRemind == null)
                    {
                        if (row.Any(x => x.ToString() == "FAIL"))
                        {
                            testFailRemind = "预测将发生违约，启动差额支付";
                        }
                    }
                    var element = dr["类型"] != null ? dr["类型"].ToString() : "";
                    var description = dr["项目描述"] != null ? dr["项目描述"].ToString() : "";

                    if (prevAsset == element)
                    {
                        equalRowsNumber++;
                        if (i == rowsNum - 1)
                        {
                            var info = new Tuple<int, int, int, int>(rowsNum - equalRowsNumber, 0, equalRowsNumber, 1);
                            viewModel.MergeCellsInfo.Add(info);
                        }
                    }
                    else
                    {
                        var info = new Tuple<int, int, int, int>(i - equalRowsNumber, 0, equalRowsNumber, 1);
                        viewModel.MergeCellsInfo.Add(info);

                        equalRowsNumber = 1;
                        prevAsset = cashflowDt.Rows[i][0].ToString();
                    }
                }
            }
            var result = new
            {
                colHeader = viewModel.ColHeader,
                paymentDate = Toolkit.DateToString(paymentDate),
                dataResult = viewModel.CashflowDataResult,
                mergeCellsInfo = viewModel.MergeCellsInfo.ConvertAll(x => new
                {
                    row = x.Item1,
                    col = x.Item2,
                    rowspan = x.Item3,
                    colspan = x.Item4
                }),
                isError = false,
                PredictInterestCollection = cashflowViewModel.PredictInterestCollection,
                PredictPricipalCollection = cashflowViewModel.PredictPricipalCollection,
                CurrentInterestCollection = cashflowViewModel.CurrentInterestCollection,
                CurrentPricipalCollection = cashflowViewModel.CurrentPricipalCollection,
                OverridableVariables = cashflowViewModel.OverridableVariables,
                TestFailRemind = testFailRemind
            };
            return result;
        }

        [HttpPost]
        public ActionResult GetAssetCashflowFile(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(paymentDay), "请选择偿付期");

                DateTime paymentDate;
                CommUtils.Assert(DateTime.TryParse(paymentDay, out paymentDate),"偿付期错误，请刷新页面后重试");

                var handler = new TaskExAssetCashflow(CurrentUserName, null);
                var ms = handler.GetACFTableFileByProject(projectGuid, paymentDate);

                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "AssetCashflowTable.csv", ms);
                return ActionUtils.Success(resource.Guid);
            });
        }

        [HttpPost]
        public ActionResult GetCashflowDetailDataTable(string projectGuid, string paymentDay)
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
                        errorMessage = "解析偿付期[" + paymentDay + "]失败，请刷新页面后重试"
                    };
                    return ActionUtils.Success(errorResult);
                }

                try
                {
                    var result = GetCashflowDetailDataTable(projectGuid, paymentDate);
                    return ActionUtils.Success(result);
                }
                catch (ApplicationException ae)
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = ae.Message
                    };
                    return ActionUtils.Success(errorResult);
                }
            });
        }

        public object GetCashflowDetailDataTable(string projectGuid, DateTime paymentDate)
        {
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

            var projectLogicModel = new ProjectLogicModel(CurrentUserName, project);
            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));

            var dataset = m_dbAdapter.Dataset.GetDatasetByDurationPeriod(project.ProjectId, paymentDate);

            //同步数据库中参数到模型文件
            var variableDateset = m_dbAdapter.CashflowVariable.GetByPaymentDay(project.ProjectId, paymentDate);
            UpdateVariablesFromCurrPeriodDBToModel(datasetSchedule.Dataset, variableDateset, project.ProjectId, paymentDate);

            var cashflowTableViewModel = new CashflowDetailTableViewModel();
            cashflowTableViewModel.CurrentPaymentDate = Toolkit.DateToString(datasetSchedule.PaymentDate);

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
            var cashflowViewModel = new CashflowViewModel();
            cashflowViewModel.PredictInterestCollection = double.Parse(assetViewModel.TotalCurrentInterestCollection.ToString("n2"));
            cashflowViewModel.PredictPricipalCollection = double.Parse(assetViewModel.TotalCurrentPrinCollection.ToString("n2"));
            cashflowViewModel.OverridableVariables = SelectOverridableVariables(datasetSchedule.Dataset);

            var cashflowDt = ABSDealUtils.GetCashflowDt(datasetSchedule, assetCashflowVariable,
                cashflowViewModel.PredictInterestCollection, cashflowViewModel.PredictPricipalCollection,
                out double currInterest, out double currPrincipal);

            cashflowViewModel.CurrentInterestCollection = currInterest;
            cashflowViewModel.CurrentPricipalCollection = currPrincipal;

            cashflowDt = Toolkit.CleanCashflowTable(cashflowDt);
            var revertTable = cashflowDt.Transpose();

            var notes = m_dbAdapter.Dataset.GetNotes(project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);

            Toolkit.GetCashflowDetailData(cashflowTableViewModel, revertTable, notes, noteDatas);
            var result = new
            {
                ProjectCashflowHeader = cashflowTableViewModel.ProjectCashflowHeader,
                SecurityCashflowHeader = cashflowTableViewModel.SecurityCashflowHeader,
                AssetPoolHeader = cashflowTableViewModel.AssetPoolHeader,
                CostHeader = cashflowTableViewModel.CostHeader,
                AccountHeader = cashflowTableViewModel.AccountHeader,
                TriggerEventHeader = cashflowTableViewModel.TriggerEventHeader,
                HomePageHeader = cashflowTableViewModel.HomePageHeader,

                ProjectCashflowList = cashflowTableViewModel.ProjectCashflowList,
                SecurityCashflowList = cashflowTableViewModel.SecurityCashflowList.Keys.ToList().ConvertAll(x =>
                new
                {
                    TableName = x,
                    DataList = cashflowTableViewModel.SecurityCashflowList[x]
                }),
                AssetPoolList = cashflowTableViewModel.AssetPoolList,
                CostList = cashflowTableViewModel.CostList,
                AccountList = cashflowTableViewModel.AccountList,
                TriggerEventList = cashflowTableViewModel.TriggerEventList,
                CurrPeriodCashflowInfoList = cashflowTableViewModel.CurrPeriodCashflowInfoList,
                CashflowEventList = cashflowTableViewModel.CashflowEventList,
                FeePayable = cashflowTableViewModel.FeePayable,
                CurrentPaymentDate = cashflowTableViewModel.CurrentPaymentDate,
                PredictInterestCollection = cashflowViewModel.PredictInterestCollection,
                PredictPricipalCollection = cashflowViewModel.PredictPricipalCollection,
                CurrentInterestCollection = cashflowViewModel.CurrentInterestCollection,
                CurrentPricipalCollection = cashflowViewModel.CurrentPricipalCollection,
                OverridableVariables = cashflowViewModel.OverridableVariables,
                TestFailRemind = cashflowTableViewModel.TestFailRemind
            };

            return result;
        }

        /// <summary>
        /// 从文件中选取可修改变量
        /// </summary>
        private List<CashflowVariablesData> SelectOverridableVariables(DatasetLogicModel dataset)
        {
            var varList = new List<CashflowVariablesData>();
            var records = dataset.Variables.FutureVariables.Where(x => m_overridableVariableNames.Contains(x.Name));
            foreach (var record in records)
            {
                double value;
                if (!double.TryParse(record.Items[dataset.DatasetSchedule.PaymentDate], out value))
                {
                    value = 0;
                }

                varList.Add(new CashflowVariablesData
                {
                    EnName = record.Name,
                    CnName = Translate(record.Name, record.Description),
                    Value = value,
                });
            }
            return varList;
        }

        private readonly string[] m_overridableVariableNames = { "OtherIncomeMax", "Expense", "AuditFee",
                                                                   "RatingFee", "TrusteeFee", "ReserveFee",
                                                                   "PreReserveFee", "中证手续费储备金额" };

        private string Translate(string enName, string description)
        {
            //使用Description或系统翻译EnName作为UI上的中文名
            bool isInvalidDescription = string.IsNullOrEmpty(description) || description.StartsWith("?");
            var cnName = isInvalidDescription ? Toolkit.TranslateEn2CnFutureVariable(enName) : description;
            return cnName;
        }

        private List<CashflowVariable> SaveOverridableVariable(List<CashflowVariablesData> overridableVariables, int projectId, DateTime paymentDay)
        {
            var result = new List<CashflowVariable>();
            var variableDateset = m_dbAdapter.CashflowVariable.GetByPaymentDay(projectId, paymentDay);
            if (variableDateset.Count > 0)
            {
                //删除旧的overridableVariables
                m_dbAdapter.CashflowVariable.Delete(variableDateset);
            }

            foreach (var variable in overridableVariables)
            {
                var cashflowVariable = new CashflowVariable();
                cashflowVariable.ProjectId = projectId;
                cashflowVariable.PaymentDate = paymentDay;
                cashflowVariable.ChineseName = variable.CnName;
                cashflowVariable.EnglishName = variable.EnName;
                cashflowVariable.Value = variable.Value;

                result.Add(m_dbAdapter.CashflowVariable.New(cashflowVariable));
            }

            return result;
        }

        private void UpdateVariablesFromCurrPeriodDBToModel(DatasetLogicModel dataset,
            List<CashflowVariable> variableDateset, int projectId, DateTime paymentDate)
        {
            if (dataset == null)
            {
                return;
            }

            if (variableDateset.Count == 0)
            {
                var tasks = m_dbAdapter.Task.GetTasksByProjectId(projectId, true);
                tasks = tasks.Where(x =>
                    x.TaskExtensionId.HasValue && x.TaskExtension.TaskExtensionType == TaskExtensionType.Cashflow.ToString()
                ).ToList();

                if (tasks.Count > 0)
                {
                    var projectLogicModel = dataset.ProjectLogicModel;
                    var allPaymentDays = projectLogicModel.GetAllPaymentDates(projectLogicModel.DealSchedule.Instanse.PaymentDates);

                    var currPaymentDateIndex = allPaymentDays.IndexOf(paymentDate);
                    if (currPaymentDateIndex != -1)
                    {
                        if (currPaymentDateIndex == 0)
                        {
                            tasks = tasks.Where(x => x.EndTime <= paymentDate).ToList();
                        }
                        else
                        {
                            tasks = tasks.Where(x => x.EndTime > allPaymentDays[currPaymentDateIndex + 1] && x.EndTime <= paymentDate).ToList();
                        }

                        if (tasks.Count > 0)
                        {
                            var task = tasks.First();
                            var dbViewModel = Toolkit.ConvertTaskExtension(task.TaskExtension).Info as CashflowViewModel;
                            if (dbViewModel != null)
                            {
                                var cashflowVariable = SaveOverridableVariable(dbViewModel.OverridableVariables, projectId, paymentDate);

                                UpdateVariablesFromCurrPeriodDBToModel(dataset, cashflowVariable, projectId, paymentDate);
                            }

                        }
                    }
                }
            }

            //更新FutureVariables.csv
            foreach (var keyVal in variableDateset)
            {
                dataset.Variables.UpdateVariableValue(keyVal.EnglishName, dataset.DatasetSchedule.PaymentDate, keyVal.Value.ToString());
            }

            //更新CurrentVariables.csv
            //设置 OtherIncome = 当期的OtherIncomeMax + 前一期模型的OtherIncome
            var otherIncomeMaxKeyVal = variableDateset.SingleOrDefault(x => x.EnglishName.Equals("OtherIncomeMax", StringComparison.CurrentCultureIgnoreCase));
            if (otherIncomeMaxKeyVal != null)
            {
                var key = "OtherIncome";
                var otherIncomeValue = otherIncomeMaxKeyVal.Value;

                if (dataset.Previous != null && dataset.Previous.Instance != null)
                {
                    //当前一期Dataset存在时，如果前一期的CurrentVariablesCsv不存在，不处理前一期表中的OtherIncome
                    if (dataset.Previous.Variables.CurrentVariables.Count> 0)
                    {
                        //var prevColumnDate = dataset.Previous.Variables..DateColumns.First();
                        foreach (var record in dataset.Previous.Variables.CurrentVariables)
                        {
                            if (record.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                double prevOtherIncome = 0;
                                if (double.TryParse(record.Value, out prevOtherIncome))
                                {
                                    otherIncomeValue += prevOtherIncome;
                                    break;
                                }
                            }
                        }
                    }
                }

                var currentCsv = dataset.Variables.CurrentVariables;

                CommUtils.Assert(currentCsv.Count > 0,
                    $"第{DateUtils.DateToString(paymentDate)}期模型中，currentCSV不包含任何偿付期列");
                var date = dataset.Variables.Asofdate;

                dataset.Variables.UpdateVariableValue(key, date, otherIncomeValue.ToString(), "其它收益");
                dataset.Variables.Save();
            }
        }

        [HttpPost]
        public ActionResult SaveVariables(string projectGuid, string paymentDay, string varList)
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

                try
                {
                    SaveCurrPeriodCashflowVariable(varList, projectGuid, paymentDate);
                    return ActionUtils.Success("");
                }
                catch (ApplicationException ae)
                {
                    var errorResult = new
                    {
                        isError = true,
                        errorMessage = ae.Message
                    };
                    return ActionUtils.Success(errorResult);
                }
            });
        }

        private List<CashflowVariablesData> SaveCurrPeriodCashflowVariable(string varList, string projectGuid, DateTime paymentDate)
        {
            var projectLogicModel = Platform.GetProject(projectGuid);
            var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
            CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "当期模型不存在");
            var project = projectLogicModel.Instance;

            //检查并转换varList => overridableVariableList
            var overridableVariables = CheckAndConvertOverridableVariableList(datasetSchedule.Dataset, varList);
            SaveOverridableVariable(overridableVariables, project.ProjectId, paymentDate);

            //保存当期资产池本金与利息
            //SaveCurrPeriodInterestPricipal(project.ProjectId, paymentDate, firstCollectionInterest, firstCollectionPrincipal);
            //ToDo:log


            return new List<CashflowVariablesData>();
        }

        private List<CashflowVariablesData> CheckAndConvertOverridableVariableList(DatasetLogicModel dataset, string varList)
        {
            var overridableVariables = new List<CashflowVariablesData>();
            var dictNameDescription = new Dictionary<string, string>();
            foreach (var record in dataset.Variables.FutureVariables)
            {
                dictNameDescription[record.Name] = record.Description;
            }
            Func<string, string> getDescription = x => dictNameDescription.ContainsKey(x) ? dictNameDescription[x] : null;

            var overridableVariablesStu = CommUtils.FromJson<CashflowVariableStu[]>(varList).ToList();

            foreach (var overridableVariable in overridableVariablesStu)
            {
                double value = 0.0;
                var cnName = Translate(overridableVariable.Name, getDescription(overridableVariable.Name));
                CommUtils.Assert(double.TryParse(overridableVariable.Value, out value) && value >= 0, "[{0}]必须是非负合法数字", cnName);

                overridableVariables.Add(new CashflowVariablesData
                {
                    EnName = overridableVariable.Name,
                    CnName = cnName,
                    Value = value
                });
            }
            return overridableVariables;
        }

        [HttpPost]
        public ActionResult SaveInterestPricipal(string projectGuid, string paymentDay, string principal, string interest, bool enableOverride)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(paymentDay), "请选择偿付期");
                CommUtils.Assert(DateTime.TryParse(paymentDay, out var paymentDate), "偿付期错误，请刷新页面后重试");
                CommUtils.Assert(double.TryParse(principal, out var overridePrincipal) && overridePrincipal >= 0, "[{0}]必须是非负合法数字", interest);
                CommUtils.Assert(double.TryParse(interest, out var overrideInterest) && overrideInterest >= 0, "[{0}]必须是非负合法数字", principal);

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                SaveCurrPeriodInterestPricipal(project.ProjectId, paymentDate, overrideInterest, overridePrincipal, enableOverride);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult GetInterestPrinSettingInfo(string projectGuid, string paymentDay)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(paymentDay), "请选择偿付期");
                CommUtils.Assert(DateTime.TryParse(paymentDay, out var paymentDate), "偿付期错误，请刷新页面后重试");

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var assetCashflowVariable = m_dbAdapter.AssetCashflowVariable.GetByProjectIdPaymentDay(project.ProjectId, paymentDate);
                if (assetCashflowVariable != null)
                {
                    var result = new
                    {
                        enableOverride = assetCashflowVariable.EnableOverride,
                        interest = double.Parse(assetCashflowVariable.InterestCollection.ToString("n2")),
                        principal = double.Parse(assetCashflowVariable.PricipalCollection.ToString("n2"))
                    };

                    return ActionUtils.Success(result);
                }

                return ActionUtils.Success("");
            });
        }

        //保存当期资产池本金与利息
        private void SaveCurrPeriodInterestPricipal(int projectId, DateTime paymentDay,
            double firstCollectionInterest, double firstCollectionPrincipal, bool enableOverride)
        {
            var obj = m_dbAdapter.AssetCashflowVariable.GetByProjectIdPaymentDay(projectId, paymentDay);

            if (obj != null)
            {
                obj.InterestCollection = firstCollectionInterest;
                obj.PricipalCollection = firstCollectionPrincipal;
                obj.EnableOverride = enableOverride;

                m_dbAdapter.AssetCashflowVariable.Update(obj);
            }
            else
            {
                var newObj = new AssetCashflowVariable();
                newObj.ProjectId = projectId;
                newObj.PaymentDate = paymentDay;
                newObj.InterestCollection = firstCollectionInterest;
                newObj.PricipalCollection = firstCollectionPrincipal;
                newObj.EnableOverride = enableOverride;

                m_dbAdapter.AssetCashflowVariable.New(newObj);
            }
        }
    }
}