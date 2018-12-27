using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using CNABS.Mgr.Deal.Model;
using CNABS.Mgr.Deal.Model.Result;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class SimulationController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProjectsHasDealModel(string projectType)
        {
            return ActionUtils.Json(() =>
            {
                //TODO:
                CommUtils.AssertEquals(projectType, "存续期", "目前仅支持获取存续期产品列表");

                var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var projects = m_dbAdapter.Project.GetProjectsHasDealModel(projectIds);

                var datasets = m_dbAdapter.Dataset.GetDatasetByProjectIds(projects.Select(x => x.ProjectId))
                    .Where(x => x.PaymentDate.HasValue)
                    .OrderBy(x => x.ProjectId).ThenBy(x => x.PaymentDate.Value);

                var validProjects = new List<Project>();
                foreach (var dataset in datasets)
                {
                    if (validProjects.Any(x => x.ProjectId == dataset.ProjectId))
                    {
                        continue;
                    }

                    var project = projects.Single(x => x.ProjectId == dataset.ProjectId);

                    if (project.Model == null)
                    {
                        project.Model = m_dbAdapter.Project.GetModel(project.ModelId);
                    }

                    var modelFolder = WebConfigUtils.RootFolder + project.Model.ModelFolder + "\\";
                    var asOfDateFolder = modelFolder + dataset.AsOfDate;
                    if (Directory.Exists(asOfDateFolder))
                    {
                        validProjects.Add(project);
                    }
                }

                var result = validProjects.ConvertAll(x => new
                {
                    name = x.Name,
                    guid = x.ProjectGuid
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetAsOfDateList(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var logicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                if (!logicModel.HasDealModel)
                {
                    return ActionUtils.Success(new List<object> { }); 
                }

                var periods = logicModel.DealSchedule.DurationPeriods;
                var validPeriods = periods.Where(x => x.Dataset != null && x.Dataset.Instance != null);
                var result = validPeriods.Select(x => new {
                    AsOfDate = Toolkit.DateToString(DateUtils.Parse(x.Dataset.Instance.AsOfDate).Value).Replace("-", ""),
                    PaymentDay = Toolkit.DateToString(x.PaymentDate),
                    Sequence = x.Sequence,
                    ProjectGuid = projectGuid
                }).ToList();

                return ActionUtils.Success(result);
            });
        }

        internal enum TriggerOption
        {
            Auto = 0,
            Trigger = 1,
            NotTrigger = -1,
        }

        private TriggerOption ConvertTriggerOption(string csvField)
        {
            int triggerOption;
            CommUtils.Assert(int.TryParse(csvField, out triggerOption) && triggerOption < 2 && triggerOption > -2,
                "解析事件参数[{0}]失败，有效值：-1,0,1", csvField);
            return (TriggerOption)triggerOption;
        }

        [HttpPost]
        public ActionResult GetModelInfo(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var logicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var periods = logicModel.DealSchedule.DurationPeriods;
                var validPeriods = periods.Where(x => x.Dataset != null && x.Dataset.Instance != null
                    && DateUtils.Parse(x.Dataset.Instance.AsOfDate).Value == DateUtils.Parse(asOfDate).Value);

                CommUtils.AssertEquals(validPeriods.Count(), 1, "查找模型失败asOfDate={0}", asOfDate);

                var paymentDay = validPeriods.Single().PaymentDate;

                var strAsOfDate = asOfDate.Replace("-", "");
                var datasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(logicModel.Instance, strAsOfDate);


                var futureVariablesCsv = new VariablesHelper(datasetFolder);
                futureVariablesCsv.Load();
                var key = "trigger_";
                var triggerRecords = futureVariablesCsv.FutureVariables.Where(record => record.Name.Trim().ToLower().StartsWith(key))
                    .Select(record => new
                    {
                        key = record.Name.Substring(key.Length),
                        name = record.Description,
                        value = ConvertTriggerOption(record.Items.First(x => x.Key == paymentDay).Value).ToString()
                    });


                double value;
                var variables = futureVariablesCsv.FutureVariables.Where(x => m_overridableVariableNames.Contains(x.Name))
                    .Select(x => new {
                        name = x.Name,
                        description = Translate(x.Name, x.Description),
                        value = double.TryParse(x.Items[paymentDay], out value) ? value : 0
                    });

                var result = new
                {
                    variables = variables,
                    triggerItems = triggerRecords
                };

                return ActionUtils.Success(result);
            });
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

        private Dictionary<string, TriggerOption> ParseTriggerOption(string triggerOption)
        {
            var dict = new Dictionary<string, TriggerOption>();
            var triggerOptionItems = CommUtils.Split(triggerOption);
            foreach (var trigger in triggerOptionItems)
            {
                var keyValue = CommUtils.Split(trigger, new [] {"^"});
                CommUtils.AssertEquals(keyValue.Length, 2, "解析triggerOption[{0}]失败", triggerOption);
                dict[keyValue[0]] = CommUtils.ParseEnum<TriggerOption>(keyValue[1]);
            }
            return dict;
        }

        private Dictionary<string, string> ParseVariables(string variables)
        {
            var dict = new Dictionary<string, string>();
            var variableItems = CommUtils.Split(variables);
            foreach (var variable in variableItems)
            {
                var keyValue = CommUtils.Split(variable, new[] { "^" });
                CommUtils.AssertEquals(keyValue.Length, 2, "解析失败,variable[{0}]不能为空", keyValue);
                dict[keyValue[0]] = keyValue[1];
            }
            return dict;
        }

        [HttpPost]
        public ActionResult Run(string projectGuid, string asOfDate, string triggerOption, string variables,
            string prepaymentSetGuid, string assetDefaultSetGuid, string interestRateAdjustmentSetGuid)
        {
            return ActionUtils.Json(() =>
            {
                var absDealResult = GetCalculateCashflowTable(projectGuid, asOfDate, triggerOption,variables,
                    prepaymentSetGuid, assetDefaultSetGuid, interestRateAdjustmentSetGuid);

                StringBuilder json = new StringBuilder();
                json.Append("[{");
                json.Append("\"cashflow\":");
                json.Append(absDealResult.Cf.ToJson());
                json.Append(",\"assetCashflow\":");
                json.Append(absDealResult.Acf.ToJson());
                json.Append("}]");

                return ActionUtils.Success(json.ToString());
            });
        }

        //导出证券端现金流表对比测算结果
        [HttpPost]
        public ActionResult GetCashflowCompareResultFile(string prevTableJson, string currTableJson)
        {
            return ActionUtils.Json(() =>
            {
                if (string.IsNullOrWhiteSpace(prevTableJson))
                {
                    prevTableJson = currTableJson;
                }
                var prevTable = prevTableJson.ToDataTable();
                var currTable = currTableJson.ToDataTable();

                var logic = new SimulationLogicModel(CurrentUserName);

                var newTable = logic.MergeTable(currTable, prevTable);
                var ms = logic.GenerateCompareResultExcel(newTable, currTable, prevTable);

                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "CashflowTable.xlsx", ms);
                return ActionUtils.Success(resource.Guid);
            });
        }

        //导出资产端现金流表对比测算结果
        [HttpPost]
        public ActionResult GetAssetCashflowCompareResultFile(string prevTableJson, string currTableJson)
        {
            return ActionUtils.Json(() =>
            {
                if (string.IsNullOrWhiteSpace(prevTableJson))
                {
                    prevTableJson = currTableJson;
                }
                var prevTable = prevTableJson.ToDataTable();
                var currTable = currTableJson.ToDataTable();

                var logic = new SimulationLogicModel(CurrentUserName);

                var newTable = logic.MergeTable(currTable, prevTable);
                var ms = logic.GenerateCompareResultExcel(newTable, currTable, prevTable);

                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "AssetCashflowTable.xlsx", ms);
                return ActionUtils.Success(resource.Guid);
            });
        }

        //获取前一期模型的CurrentVariable.csv文件中的OtherIncome的值
        private double GetPreviousCurrentVariableCsvOtherIncome(DatasetScheduleLogicModel period)
        {
            double prevCurVarCsvOtherIncome = 0.0;
            if (period.Previous != null && period.Previous.Dataset != null && period.Previous.Dataset.HasDealModel)
            {
                prevCurVarCsvOtherIncome = period.Previous.Dataset.Variables.GetVariable<double>("OtherIncome");
            }

            return prevCurVarCsvOtherIncome;
        }

        private ABSDealResult GetCalculateCashflowTable(string projectGuid, string asOfDate, string triggerOption, string variables,
            string prepaymentSetGuid, string assetDefaultSetGuid, string interestRateAdjustmentSetGuid)
        {
            var logicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
            var periods = logicModel.DealSchedule.DurationPeriods;
            var validPeriods = periods.Where(x => x.Dataset != null && x.Dataset.Instance != null
                && DateUtils.Parse(x.Dataset.Instance.AsOfDate).Value == DateUtils.Parse(asOfDate).Value);

            CommUtils.AssertEquals(validPeriods.Count(), 1, "查找模型失败asOfDate={0}", asOfDate);

            var variableDict = ParseVariables(variables);
            foreach (var key in variableDict.Keys)
            {
                var value = variableDict[key];
                double tempValue = 0.0;
                CommUtils.Assert(double.TryParse(value, out tempValue) && tempValue >= 0, "{0}[{1}]必须输入非负数字", key, value);
            }

            CommUtils.Assert(variableDict.ContainsKey("CDR"), "请输入变量CDR");
            CommUtils.Assert(variableDict.ContainsKey("CPR"), "请输入变量CPR");

            double cdr = double.Parse(variableDict["CDR"]);
            double cpr = double.Parse(variableDict["CPR"]);
            CommUtils.Assert(cdr >= 0 && cdr <= 1, "CDR[{0}]必须是0~1之间的数字", cdr);
            CommUtils.Assert(cpr >= 0 && cpr <= 1, "CPR[{0}]必须是0~1之间的数字", cpr);

            var temporaryFolder = m_dbAdapter.Dataset.GetTemporaryFolder();
            if (!Directory.Exists(temporaryFolder))
            {
                try
                {
                    Directory.CreateDirectory(temporaryFolder);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Create temporary folder failed! Exception: " + e.Message);
                }
            }

            temporaryFolder = Path.Combine(temporaryFolder, DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss fff]") + "-[" + CurrentUserName + "]-[" + Guid.NewGuid().ToString() + "]");
            Directory.CreateDirectory(temporaryFolder);

            var period = validPeriods.Single();
            var strAsOfDate = asOfDate.Replace("-", "");
            var srcDatasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(logicModel.Instance, strAsOfDate);
            var destDatasetFolder = Path.Combine(temporaryFolder, strAsOfDate);
            if (!Directory.Exists(destDatasetFolder))
            {
                try
                {
                    Directory.CreateDirectory(destDatasetFolder);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Create folder failed! Exception: " + e.Message);
                }
            }

            var ymlFilePath = m_dbAdapter.Dataset.GetYmlFilePath(logicModel.Instance);

            ParallelUtils.StartUntilFinish(
                () => FileUtils.Copy(ymlFilePath, Path.Combine(temporaryFolder, "script.yml")),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "collateral.csv"), Path.Combine(destDatasetFolder, "collateral.csv")),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "AmortizationSchedule.csv"), Path.Combine(destDatasetFolder, "AmortizationSchedule.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "PromisedCashflow.csv"), Path.Combine(destDatasetFolder, "PromisedCashflow.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "Reinvestment.csv"), Path.Combine(destDatasetFolder, "Reinvestment.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "CombinedVariables.csv"), Path.Combine(destDatasetFolder, "CombinedVariables.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "PastVariables.csv"), Path.Combine(destDatasetFolder, "PastVariables.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "CurrentVariables.csv"), Path.Combine(destDatasetFolder, "CurrentVariables.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDatasetFolder, "FutureVariables.csv"), Path.Combine(destDatasetFolder, "FutureVariables.csv"), false)
            );

            var variableHelper = new VariablesHelper(destDatasetFolder);
            variableHelper.Load();

            //设置用户指定的事件触发
            var triggerDict = ParseTriggerOption(triggerOption);
            foreach (var trigger in triggerDict)
            {
                variableHelper.UpdateVariableValue("Trigger_" + trigger.Key, period.PaymentDate, ((int)trigger.Value).ToString());
            }

            foreach (var variable in variableDict)
            {
                variableHelper.UpdateVariableValue(variable.Key, period.PaymentDate, variable.Value);
            }

            //设置用户指定利率
            if (!string.IsNullOrWhiteSpace(interestRateAdjustmentSetGuid))
            {
                var interestRateSet = m_dbAdapter.InterestRateAdjustmentSet.GetByGuid(interestRateAdjustmentSetGuid);
                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateSet.Id);
                if (adjustments.Count > 0)
                {
                    var rateTypeList = adjustments.Select(x => x.InterestRateType).Distinct().ToList();
                    foreach (var rateType in rateTypeList)
                    {
                        var rowName = rateType.ToString() + ".Reset";

                        var adjustmentsByRateType = adjustments.Where(x => x.InterestRateType == rateType);
                        var dates = variableHelper.DateColumns.Where(o => o >= variableHelper.Asofdate);
                        foreach (var paymentDate in dates)
                        {
                            var adjustment = adjustmentsByRateType.LastOrDefault(x => x.AdjustmentDate <= paymentDate);
                            if (adjustment == null)
                            {
                                adjustment = adjustmentsByRateType.First();
                            }

                            variableHelper.UpdateVariableValue(rowName, paymentDate, adjustment.InterestRate.ToString());
                        }
                    }
                }
            }

            foreach (var variable in variableDict)
            {
                if (variable.Key.Equals("OtherIncomeMax", StringComparison.CurrentCultureIgnoreCase))
                {
                    double newOtherIncome = 0.0;
                    double.TryParse(variable.Value, out newOtherIncome);

                    DateTime paymentDateInCurrentVariableCsv;
                    if (period.Previous == null)
                    {
                        paymentDateInCurrentVariableCsv = period.PaymentDate;
                    }
                    else
                    {
                        paymentDateInCurrentVariableCsv = period.Previous.PaymentDate;

                        //设置 OtherIncome = 当期的OtherIncomeMax + 前一期模型的OtherIncome
                        double prevCurVarCsvOtherIncome = GetPreviousCurrentVariableCsvOtherIncome(period);
                        newOtherIncome += prevCurVarCsvOtherIncome;
                    }

                    variableHelper.UpdateVariableValue("OtherIncome", paymentDateInCurrentVariableCsv, newOtherIncome.ToString("#.##"));
                }
            }
            variableHelper.Save();

            //设置违约
            if (!string.IsNullOrWhiteSpace(assetDefaultSetGuid))
            {
                var assetDefaultSet = m_dbAdapter.AssetDefaultSet.GetByGuid(assetDefaultSetGuid);
                var assetDefaults = m_dbAdapter.AssetDefault.GetByAssetDefaultSetId(assetDefaultSet.Id);
                if (assetDefaults.Count != 0)
                {
                    var collateralPath = Path.Combine(destDatasetFolder, "collateral.csv");
                    var collateralCsv = new CollateralCsv();
                    collateralCsv.Load(collateralPath);

                    foreach (var assetDefault in assetDefaults)
                    {
                        collateralCsv.UpdateCellValue(assetDefault.AssetId, "DefaultDate", Toolkit.DateToString(assetDefault.AssetDefaultDate));
                        collateralCsv.UpdateCellValue(assetDefault.AssetId, "RecoveryRate", CommUtils.Percent(assetDefault.RecoveryRate));
                        collateralCsv.UpdateCellValue(assetDefault.AssetId, "RecoveryLag", assetDefault.RecoveryLag.ToString("#.##"));
                    }

                    collateralCsv.Save();
                }
            }

            var fileNameLength = Path.GetFileName(ymlFilePath).Length;
            var ymlFolder = ymlFilePath.Substring(0, ymlFilePath.Length - fileNameLength);

            //检查模型文件AmortizationSchedule.CSV
            DealModelUtils.CheckAmortizationSchedule(period.Dataset, asOfDate);

            var absDeal = new ABSDeal(ymlFolder, destDatasetFolder);
            absDeal.Setting.Cdr = cdr;
            absDeal.Setting.Cpr = cpr;

            ParallelUtils.Start(() => Directory.Delete(temporaryFolder, true));

            return absDeal.Result;
        }

        //导出资产端现金流
        [HttpPost]
        public ActionResult GetAssetCashflowFile(string tableJson)
        {
            return ActionUtils.Json(() =>
            {
                var table = tableJson.ToDataTable();

                var ms = ToExcelMemoryStream(table, "AssetCashflowTable.xlsx");

                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "AssetCashflowTable.xlsx", ms);
                return ActionUtils.Success(resource.Guid);
            });
        }

        //导出证券端现金流
        [HttpPost]
        public ActionResult GetCashflowFile(string tableJson)
        {
            return ActionUtils.Json(() =>
            {
                var table = tableJson.ToDataTable();

                var ms = ToExcelMemoryStream(table, "CashflowTable.xlsx");
                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "CashflowTable.xlsx", ms);

                return ActionUtils.Success(resource.Guid);
            });
        }

        private MemoryStream ToExcelMemoryStream(DataTable table, string fileName)
        {
            var tempFolder = CommUtils.CreateTemporaryFolder(CurrentUserName);
            var tempFilePath = Path.Combine(tempFolder, fileName);
            ExcelUtils.TableToExcel(table, tempFilePath, true);

            var buffer = System.IO.File.ReadAllBytes(tempFilePath);
            CommUtils.DeleteFolderAync(tempFolder);

            return new MemoryStream(buffer);
        }

        private string GetCsvFile(DataTable translateCashflowDt,string fileName)
        {
            var ms = translateCashflowDt.ToCsvMemoryStream(CurrentUserName);
            var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, fileName, ms);
            return resource.Guid.ToString();
        }

        [HttpPost]
        public ActionResult GetAssetDefaultSet(string assetDefaultSetGuid)
        {
            return ActionUtils.Json(() =>
            {
                var assetDefaultSet = m_dbAdapter.AssetDefaultSet.GetByGuid(assetDefaultSetGuid);
                var assetDefaults = m_dbAdapter.AssetDefault.GetByAssetDefaultSetId(assetDefaultSet.Id);

                var result = new
                {
                    name = assetDefaultSet.Name,
                    guid = assetDefaultSet.Guid,
                    createTime = Toolkit.DateTimeToString(assetDefaultSet.CreateTime),
                    createUserName = assetDefaultSet.CreateUserName,

                    assetDefaults = assetDefaults.Select(x => new
                    {
                        guid = x.Guid,
                        assetId = x.AssetId,
                        assetDefaultDate = Toolkit.DateToString(x.AssetDefaultDate),
                        recoveryRate = x.RecoveryRate,
                        recoveryLag = x.RecoveryLag,
                        recoveryDate = Toolkit.DateToString(x.RecoveryDate),
                        createUserName = x.CreateUserName,
                        createTime = Toolkit.DateTimeToString(x.CreateTime),
                    })
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreateAssetDefaultSet(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(asOfDate);
                var logicModel = Platform.GetProject(projectGuid);
                var datasetSchedule = logicModel.DealSchedule.GetByAsOfDate(date);
                CommUtils.Assert(datasetSchedule.Dataset.HasDealModel,
                    "查找模型失败, projectGuid={0},asOfDate={1}", projectGuid, asOfDate);

                var assetDefaultSet = m_dbAdapter.AssetDefaultSet.New(logicModel.Instance.ProjectId,
                    datasetSchedule.PaymentDate, "[Simulation]");

                var result = new
                {
                    name = assetDefaultSet.Name,
                    guid = assetDefaultSet.Guid,
                    createTime = Toolkit.DateTimeToString(assetDefaultSet.CreateTime),
                    createUserName = assetDefaultSet.CreateUserName
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult AddAssetDefault(string assetDefaultSetGuid, int assetId,
            string assetDefaultDate, string recoveryRate, double recoveryLag)
        {
            return ActionUtils.Json(() =>
            {
                var rate = MathUtils.ParseDouble(recoveryRate);
                CommUtils.Assert(rate >= 0, "回收率不能小于0");
                CommUtils.Assert(recoveryLag > 0, "回收间隔必须大于0");
                var defaultDate = DateUtils.ParseDigitDate(assetDefaultDate);

                var assetDefaultSet = m_dbAdapter.AssetDefaultSet.GetByGuid(assetDefaultSetGuid);
                CommUtils.Assert(IsCurrentUser(assetDefaultSet.CreateUserName), "当前用户{0}不是违约信息创建者{1}",
                    CurrentUserName, assetDefaultSet.CreateUserName);

                var logicModel = Platform.GetProject(assetDefaultSet.ProjectId);
                var dataset = logicModel.DealSchedule.GetByPaymentDay(assetDefaultSet.PaymentDate).Dataset;
                CommUtils.Assert(dataset.HasDealModel, "找不到偿付模型ProjectGuid={0} PaymentDate={1}",
                    logicModel.Instance.ProjectGuid, assetDefaultSet.PaymentDate);

                var asset = dataset.Assets.FirstOrDefault(x => x.AssetId == assetId);
                CommUtils.AssertNotNull(asset, "找不到AssetId={0}", assetId);

                var assetDefaults = m_dbAdapter.AssetDefault.GetByAssetDefaultSetId(assetDefaultSet.Id);
                CommUtils.AssertNot(assetDefaults.Any(x => x.AssetId == assetId), "资产[{0}({1})]已设置违约，无法重复设置",
                    asset.SecurityData.SecurityName, assetId);

                var recoveryDate = defaultDate.AddYears((int)recoveryLag).AddDays(365 * (recoveryLag - (int)recoveryLag));
                var assetDefault = m_dbAdapter.AssetDefault.New(assetDefaultSet.Id, assetId, defaultDate, rate, recoveryLag, recoveryDate);

                var result = new {
                    guid = assetDefault.Guid,
                    assetId = assetDefault.AssetId,
                    assetDefaultDate = Toolkit.DateToString(assetDefault.AssetDefaultDate),
                    recoveryRate = assetDefault.RecoveryRate,
                    recoveryLag = assetDefault.RecoveryLag,
                    recoveryDate = Toolkit.DateToString(assetDefault.RecoveryDate),
                    createUserName = assetDefault.CreateUserName,
                    createTime = Toolkit.DateTimeToString(assetDefault.CreateTime)
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult RemoveAssetDefault(string assetDefaultGuid)
        {
            return ActionUtils.Json(() =>
            {
                var assetDefault = m_dbAdapter.AssetDefault.GetByGuid(assetDefaultGuid);
                CommUtils.Assert(IsCurrentUser(assetDefault.CreateUserName), "当前用户{0}不是违约信息创建者{1}",
                    CurrentUserName, assetDefault.CreateUserName);

                var result = m_dbAdapter.AssetDefault.Remove(assetDefault);
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetPrepaymentSet(string prepaymentSetGuid)
        {
            return ActionUtils.Json(() =>
            {
                var prepaymentSet = m_dbAdapter.PrepaymentSet.GetByGuid(prepaymentSetGuid);
                var prepayments = m_dbAdapter.Prepayment.GetByPrepaymentSetId(prepaymentSet.Id);

                var result = new
                {
                    name = prepaymentSet.Name,
                    guid = prepaymentSet.Guid,
                    createTime = Toolkit.DateTimeToString(prepaymentSet.CreateTime),
                    createUserName = prepaymentSet.CreateUserName,

                    prepayments = prepayments.Select(x => new {
                        guid = x.Guid,
                        assetId = x.AssetId,
                        prepayDate = Toolkit.DateToString(x.PrepayDate),
                        originDate = Toolkit.DateToString(x.OriginDate),
                        money = x.Money,
                        createUserName = x.CreateUserName,
                        createTime = Toolkit.DateTimeToString(x.CreateTime),
                    })
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreatePrepaymentSet(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(asOfDate);
                var logicModel = Platform.GetProject(projectGuid);
                var datasetSchedule = logicModel.DealSchedule.GetByAsOfDate(date);
                CommUtils.Assert(datasetSchedule.Dataset.HasDealModel,
                    "查找模型失败, projectGuid={0},asOfDate={1}", projectGuid, asOfDate);

                var prepaymentSet = m_dbAdapter.PrepaymentSet.New(logicModel.Instance.ProjectId,
                    datasetSchedule.PaymentDate, "[Simulation]");

                var result = new
                {
                    name = prepaymentSet.Name,
                    guid = prepaymentSet.Guid,
                    createTime = Toolkit.DateTimeToString(prepaymentSet.CreateTime),
                    createUserName = prepaymentSet.CreateUserName
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult AddPrepayment(string prepaymentSetGuid, int assetId, string originDates, string prepayDate, double money)
        {
            return ActionUtils.Json(() =>
            {
                var originDateArray = CommUtils.Split(originDates).Select(x => DateUtils.ParseDigitDate(x));

                CommUtils.AssertEquals(originDateArray.Distinct().Count(), originDateArray.Count(), "originDates中有重复的日期");

                var date = DateUtils.ParseDigitDate(prepayDate);
                var prepaymentSet = m_dbAdapter.PrepaymentSet.GetByGuid(prepaymentSetGuid);
                CommUtils.Assert(IsCurrentUser(prepaymentSet.CreateUserName), "无法添加早偿信息，当前用户不是创建者[{0}]", prepaymentSet.CreateUserName);

                var logicModel = Platform.GetProject(prepaymentSet.ProjectId);
                var dataset = logicModel.DealSchedule.GetByPaymentDay(prepaymentSet.PaymentDate).Dataset;
                var asset = dataset.GetAssetById(assetId);

                CommUtils.Assert(originDateArray.Count() > 0, "请选择预计偿付日期");

                foreach (var originDate in originDateArray)
                {
                    CommUtils.Assert(date < originDate, "早偿日期 {0} 不在预计偿付日期 {1} 之前", date, originDate);
                    CommUtils.Assert(asset.Amortization.AmortizationRecords.Any(x => x.Date == originDate),
                        "未找到预计偿付日{0}", Toolkit.DateToString(originDate));
                }

                if (originDateArray.Count() > 1)
                {
                    var sum = asset.Amortization.AmortizationRecords.Where(x => originDateArray.Contains(x.Date)).Sum(x => x.Money);
                    CommUtils.Assert(MathUtils.MoneyEQ(sum, money), "早偿金额 {0} 和预计偿付金额 {1} 不相等", money, sum);
                }
                else
                {
                    var originDate = originDateArray.First();
                    var sum = asset.Amortization.AmortizationRecords.Where(x => x.Date == originDate).Sum(x => x.Money);
                    CommUtils.Assert(MathUtils.MoneyLTE(money, sum), "早偿金额 {0} 大于预计偿付金额 {1}", money, sum);
                }

                var prepayments = new List<Prepayment>();
                foreach (var originDate in originDateArray)
                {
                    var originMoney = asset.Amortization.AmortizationRecords.Single(x => x.Date == originDate).Money;

                    //多选时，money是多期预计偿付金额之和
                    //单选时，money小于等于单期预计偿付金额
                    var prepayMoney = Math.Min(originMoney, money);
                    var prepayment = m_dbAdapter.Prepayment.New(prepaymentSet.Id, assetId, date, originDate, prepayMoney);
                    prepayments.Add(prepayment);
                }

                var result = prepayments.Select(x => new
                {
                    prepaymentSetGuid = prepaymentSet.Guid,
                    assetId = x.AssetId,
                    prepayDate = Toolkit.DateToString(x.PrepayDate),
                    originDate = Toolkit.DateToString(x.OriginDate),
                    money = x.Money,
                    createUserName = x.CreateUserName,
                    createTime = Toolkit.DateTimeToString(x.CreateTime),
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult RemovePrepayment(string prepaymentGuids)
        {
            return ActionUtils.Json(() =>
            {
                var guids = CommUtils.Split(prepaymentGuids);
                var prepayments = m_dbAdapter.Prepayment.GetByGuids(guids);
                foreach (var prepayment in prepayments)
                {
                    CommUtils.Assert(IsCurrentUser(prepayment.CreateUserName), "无法删除早偿信息，当前用户不是创建者");
                }
                
                foreach (var prepayment in prepayments)
                {
                    m_dbAdapter.Prepayment.Remove(prepayment);
                }

                return ActionUtils.Success(prepayments.Count);
            });
        }

        public class AssetPaymentViewModel
        {
            public AssetPaymentViewModel()
            {
                Payments = new List<PaymentViewModel>();
            }

            public List<PaymentViewModel> Payments { get; set; }
        }

        public class PaymentViewModel
        {
            public PaymentViewModel()
            {
                Prepayments = new List<PrepaymentViewModel>();
            }

            public DateTime Date { get; set; }

            public string DateStr { get { return Toolkit.DateToString(Date); } }

            public double Money { get; set; }

            public List<PrepaymentViewModel> Prepayments { get; set; }

            public double SumPrepaymentMoney { get { return Prepayments.Sum(x => x.Money); } }

            public bool IsAssetDefault { get; set; }

            public AssetDefaultViewModel AssetDefault { get; set; }
        } 

        public class AssetDefaultViewModel
        {
            public string Guid { get; set; }

            public string AssetDefaultDate { get; set; }
            
            public double RecoveryLag { get; set; }
            
            public double RecoveryRate { get; set; }

            public string RecoveryDate { get; set; }

            public int AssetId { get; set; }
        }

        public class PrepaymentViewModel
        {
            public string PrepaymentGuid { get; set; }

            public DateTime OriginDate { get; set; }

            public string OriginDateStr { get { return Toolkit.DateToString(OriginDate); } }

            public double Money { get; set; }
        }

        [HttpPost]
        public ActionResult GetAmortizationByAsset(string projectGuid, string asOfDate, int assetId,
            string prepaymentSetGuid, string assetDefaultSetGuid)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(asOfDate);
                var logicModel = Platform.GetProject(projectGuid);
                var dataset = logicModel.DealSchedule.GetByAsOfDate(date).Dataset;
                CommUtils.Assert(dataset.Assets.Any(x => x.AssetId == assetId),
                    "找不到资产： projectGuid={0}, asOfDate={1}, assetId={2}", projectGuid, asOfDate, assetId);

                var asset = dataset.Assets.First(x => x.AssetId == assetId);
                var records = asset.Amortization.AmortizationRecords;

                List<Prepayment> prepayments = new List<Prepayment>();
                if (!string.IsNullOrWhiteSpace(prepaymentSetGuid))
                {
                    var prepaymentSet = m_dbAdapter.PrepaymentSet.GetByGuid(prepaymentSetGuid);
                    prepayments = m_dbAdapter.Prepayment.GetByPrepaymentSetId(prepaymentSet.Id);
                    prepayments = prepayments.Where(x => x.AssetId == assetId).ToList();
                }

                AssetPaymentViewModel viewModel = new AssetPaymentViewModel();
                foreach (var record in records)
                {
                    viewModel.Payments.Add(new PaymentViewModel{
                        Date = record.Date,
                        Money = record.Money,
                    });
                }

                var newDatePrepayments = new List<Prepayment>();

                //将提前偿付数据整合到预计偿付数据中
                foreach (var prepayment in prepayments)
                {
                    var findPaymentDate = false;
                    foreach (var payment in viewModel.Payments)
                    {
                        if (prepayment.OriginDate == payment.Date)
                        {
                            payment.Money -= prepayment.Money;
                            payment.Prepayments.Add(new PrepaymentViewModel {
                                Money = 0 - prepayment.Money,
                                OriginDate = prepayment.OriginDate,
                                PrepaymentGuid = prepayment.Guid
                            });
                        }
                        
                        if (prepayment.PrepayDate == payment.Date)
                        {
                            payment.Money += prepayment.Money;
                            payment.Prepayments.Add(new PrepaymentViewModel
                            {
                                Money = prepayment.Money,
                                OriginDate = prepayment.OriginDate,
                                PrepaymentGuid = prepayment.Guid
                            });

                            findPaymentDate = true;
                        }
                    }

                    if (!findPaymentDate)
                    {
                        newDatePrepayments.Add(prepayment);
                    }
                }

                //将新增早偿日期的的数据整合到预计偿付数据中
                foreach (var prepayment in newDatePrepayments)
                {
                    var payment = viewModel.Payments.SingleOrDefault(x => x.Date == prepayment.PrepayDate);
                    if (payment == null)
                    {
                        payment = new PaymentViewModel
                        {
                            Date = prepayment.PrepayDate,
                            Money = prepayment.Money,
                        };

                        viewModel.Payments.Add(payment);
                    }
                    else
                    {
                        payment.Money += prepayment.Money;
                    }

                    payment.Prepayments.Add(new PrepaymentViewModel {
                        Money = prepayment.Money,
                        OriginDate = prepayment.OriginDate,
                        PrepaymentGuid = prepayment.Guid
                    });
                }

                viewModel.Payments.RemoveAll(x => MathUtils.MoneyEQ(x.Money, 0));
                viewModel.Payments = viewModel.Payments.OrderBy(x => x.Date).ToList();

                //增加违约信息
                if (!string.IsNullOrWhiteSpace(assetDefaultSetGuid))
                {
                    var assetDefaultSet = m_dbAdapter.AssetDefaultSet.GetByGuid(assetDefaultSetGuid);
                    CommUtils.Assert(IsCurrentUser(assetDefaultSet.CreateUserName), "当前用户{0}不是违约信息(guid={1})的创建者{2}",
                        CurrentUserName, assetDefaultSet.Guid, assetDefaultSet.CreateUserName);

                    var assetDefaults = m_dbAdapter.AssetDefault.GetByAssetDefaultSetId(assetDefaultSet.Id);
                    var assetDefault = assetDefaults.SingleOrDefault(x => x.AssetId == assetId);
                    if (assetDefault != null)
                    {
                        viewModel.Payments.RemoveAll(x => x.Date >= assetDefault.AssetDefaultDate);

                        var payment = new PaymentViewModel
                        {
                            Date = assetDefault.AssetDefaultDate,
                            IsAssetDefault = true,
                            Money = 0,
                            AssetDefault = new AssetDefaultViewModel
                            {
                                Guid = assetDefault.Guid,
                                AssetId = assetId,
                                AssetDefaultDate = Toolkit.DateToString(assetDefault.AssetDefaultDate),
                                RecoveryDate = Toolkit.DateToString(assetDefault.RecoveryDate),
                                RecoveryRate = assetDefault.RecoveryRate,
                                RecoveryLag = assetDefault.RecoveryLag,
                            }
                        };

                        viewModel.Payments.Add(payment);
                    }
                }

                return ActionUtils.Success(viewModel);
            });
        }

        [HttpPost]
        public ActionResult CreateInterestRateAdjustmentSet(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var date = DateUtils.ParseDigitDate(asOfDate);
                var logicModel = Platform.GetProject(projectGuid);
                var datasetSchedule = logicModel.DealSchedule.GetByAsOfDate(date);
                CommUtils.Assert(datasetSchedule.Dataset.HasDealModel,
                    "查找模型失败, projectGuid={0},asOfDate={1}", projectGuid, asOfDate);

                var rateResets = datasetSchedule.Dataset.Variables.GetInterestRateResets();

                var interestRateAdjustmentSet = m_dbAdapter.InterestRateAdjustmentSet.New(logicModel.Instance.ProjectId,
                    datasetSchedule.PaymentDate, "[Simulation]");

                CommUtils.Assert(rateResets.Sum(x => x.ResetRecords.Count) <= 100, "可疑数据，检测到单期数据利率调整数大于100");

                foreach (var rateReset in rateResets)
                {
                    foreach (var resetRecord in rateReset.ResetRecords)
                    {
                        m_dbAdapter.InterestRateAdjustment.New(interestRateAdjustmentSet.Id,
                            rateReset.Code, resetRecord.Date, resetRecord.InterestRate);
                    }
                }

                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustmentSet.Id);
                adjustments = adjustments.OrderBy(x => x.InterestRateType).ThenBy(x => x.AdjustmentDate).ToList();
                var result = new
                {
                    name = interestRateAdjustmentSet.Name,
                    guid = interestRateAdjustmentSet.Guid,
                    createTime = Toolkit.DateTimeToString(interestRateAdjustmentSet.CreateTime),
                    createUserName = interestRateAdjustmentSet.CreateUserName,
                    adjustments = adjustments.Select(x => new { 
                        guid = x.Guid,
                        code = x.InterestRateType.ToString(),
                        name = x.InterestRateName,
                        adjuestmentDate = Toolkit.DateToString(x.AdjustmentDate),
                        interestRate = x.InterestRate,
                        createTime = Toolkit.DateTimeToString(x.CreateTime),
                        createUserName = x.CreateUserName
                    }),
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetInterestRateAdjustmentSet(string interestRateAdjustmentSetGuid)
        {
            return ActionUtils.Json(() =>
            {
                var interestRateAdjustmentSet = m_dbAdapter.InterestRateAdjustmentSet.GetByGuid(interestRateAdjustmentSetGuid);
                var interestRateAdjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustmentSet.Id);

                CommUtils.Assert(IsCurrentUser(interestRateAdjustmentSet.CreateUserName),
                    "无法获取利率调整信息，当前用户不是创建者[{0}]", interestRateAdjustmentSet.CreateUserName);

                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustmentSet.Id);
                adjustments = adjustments.OrderBy(x => x.InterestRateType).ThenBy(x => x.AdjustmentDate).ToList();
                var result = new
                {
                    name = interestRateAdjustmentSet.Name,
                    guid = interestRateAdjustmentSet.Guid,
                    createTime = Toolkit.DateTimeToString(interestRateAdjustmentSet.CreateTime),
                    createUserName = interestRateAdjustmentSet.CreateUserName,
                    adjustments = adjustments.Select(x => new
                    {
                        guid = x.Guid,
                        code = x.InterestRateType.ToString(),
                        name = x.InterestRateName,
                        adjuestmentDate = Toolkit.DateToString(x.AdjustmentDate),
                        interestRate = x.InterestRate,
                        createTime = Toolkit.DateTimeToString(x.CreateTime),
                        createUserName = x.CreateUserName
                    }),
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetInterestRateAdjustments(string interestRateAdjustmentSetGuid, string interestRateCode)
        {
            return ActionUtils.Json(() =>
            {
                var interestRateAdjustmentSet = m_dbAdapter.InterestRateAdjustmentSet.GetByGuid(interestRateAdjustmentSetGuid);
                CommUtils.Assert(IsCurrentUser(interestRateAdjustmentSet.CreateUserName),
                    "无法删除利率调整，当前用户不是创建者[{0}]", interestRateAdjustmentSet.CreateUserName);

                var interestRateType = CommUtils.ParseEnum<PrimeInterestRate>(interestRateCode);

                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustmentSet.Id);
                adjustments = adjustments.Where(x => x.InterestRateType == interestRateType).ToList();
                CommUtils.Assert(adjustments.Count > 0, "找不到InterestRateType={0}", interestRateType.ToString());

                var result = adjustments.Select(x => new
                {
                    guid = x.Guid,
                    code = x.InterestRateType.ToString(),
                    name = x.InterestRateName,
                    adjuestmentDate = Toolkit.DateToString(x.AdjustmentDate),
                    interestRate = x.InterestRate,
                    createTime = Toolkit.DateTimeToString(x.CreateTime),
                    createUserName = x.CreateUserName
                });

                return ActionUtils.Success(result);
            });
        }

        
        [HttpPost]
        public ActionResult RemoveInterestRateAdjustment(string interestRateAdjustmentSetGuid,
            string interestRateCode, string date)
        {
            return ActionUtils.Json(() =>
            {
                var interestRateAdjustmentSet = m_dbAdapter.InterestRateAdjustmentSet.GetByGuid(interestRateAdjustmentSetGuid);
                CommUtils.Assert(IsCurrentUser(interestRateAdjustmentSet.CreateUserName),
                    "无法删除利率调整，当前用户不是创建者[{0}]", interestRateAdjustmentSet.CreateUserName);

                var interestRateType = CommUtils.ParseEnum<PrimeInterestRate>(interestRateCode);
                var adjustmentDate = DateUtils.ParseDigitDate(date);

                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustmentSet.Id);
                adjustments = adjustments.Where(x => x.InterestRateType == interestRateType).ToList();
                CommUtils.Assert(adjustments.Count > 0, "找不到InterestRateType={0}", interestRateType.ToString());

                var result = RemoveRateAdjustment(adjustments, adjustmentDate);
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult UpdateInterestRateAdjustment(string interestRateAdjustmentGuid, string date, string interestRate)
        {
            return ActionUtils.Json(() =>
            {
                var interestRateAdjustment = m_dbAdapter.InterestRateAdjustment.GetByGuid(interestRateAdjustmentGuid);
                CommUtils.Assert(IsCurrentUser(interestRateAdjustment.CreateUserName),
                    "无法更新利率调整，当前用户不是创建者[{0}]", interestRateAdjustment.CreateUserName);

                var adjustmentDate = DateUtils.ParseDigitDate(date);
                var rateValue = MathUtils.ParseDouble(interestRate);
                var interestRateType = interestRateAdjustment.InterestRateType;

                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustment.InterestRateAdjustmentSetId);
                adjustments = adjustments.Where(x => x.InterestRateType == interestRateType).ToList();
                CommUtils.Assert(adjustments.Count > 0, "找不到InterestRateType={0}", interestRateType.ToString());
                RemoveRateAdjustment(adjustments, interestRateAdjustment.AdjustmentDate);

                adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustment.InterestRateAdjustmentSetId);
                adjustments = adjustments.Where(x => x.InterestRateType == interestRateType).ToList();
                var result = AddRateAdjustment(adjustments, interestRateAdjustment.InterestRateAdjustmentSetId,
                    interestRateType, adjustmentDate, rateValue);

                return ActionUtils.Success(result);
            });
        }

        private int RemoveRateAdjustment(List<InterestRateAdjustment> adjustments, DateTime adjustmentDate)
        {
            CommUtils.Assert(adjustments.Count != 1, "至少保留一条利率信息");

            var sameDateAdjustment = adjustments.FirstOrDefault(x => x.AdjustmentDate == adjustmentDate);
            CommUtils.AssertNotNull(sameDateAdjustment, "找不到利率调整信息：date={0}", adjustmentDate);

            //前一个和后一个相邻日期利率相同，删除后一个相邻日期
            var previousAdjustment = adjustments.LastOrDefault(x => x.AdjustmentDate < adjustmentDate);
            var nextAdjustment = adjustments.FirstOrDefault(x => x.AdjustmentDate > adjustmentDate);
            if (previousAdjustment != null && nextAdjustment != null
                && MathUtils.Equals(previousAdjustment.InterestRate, nextAdjustment.InterestRate))
            {
                m_dbAdapter.InterestRateAdjustment.Remove(nextAdjustment);
            }

            var result = m_dbAdapter.InterestRateAdjustment.Remove(sameDateAdjustment);
            return result;
        }

        private string AddRateAdjustment(List<InterestRateAdjustment> adjustments, int interestRateAdjustmentSetId,
            PrimeInterestRate interestRateType, DateTime adjustmentDate, double rateValue)
        {
            //已经有相同日期的利率调整，更新该日利率
            var sameDateAdjustment = adjustments.FirstOrDefault(x => x.AdjustmentDate == adjustmentDate);
            if (sameDateAdjustment != null)
            {
                sameDateAdjustment.InterestRate = rateValue;
                m_dbAdapter.InterestRateAdjustment.Update(sameDateAdjustment);

                //和后一个相邻日期利率相同，删除后一个相邻日期
                var nextAdjustment = adjustments.FirstOrDefault(x => x.AdjustmentDate > adjustmentDate);
                if (nextAdjustment != null && MathUtils.Equals(nextAdjustment.InterestRate, rateValue))
                {
                    m_dbAdapter.InterestRateAdjustment.Remove(nextAdjustment);
                }

                //和前一个相邻日期利率相同，删除本条记录
                var previousAdjustment = adjustments.LastOrDefault(x => x.AdjustmentDate < adjustmentDate);
                if (previousAdjustment != null && MathUtils.Equals(previousAdjustment.InterestRate, rateValue))
                {
                    m_dbAdapter.InterestRateAdjustment.Remove(sameDateAdjustment);
                }

                return sameDateAdjustment.Guid;
            }
            else
            {
                //和前一个相邻日期利率相同，没有变化
                var previousAdjustment = adjustments.LastOrDefault(x => x.AdjustmentDate < adjustmentDate);
                if (previousAdjustment != null && MathUtils.Equals(previousAdjustment.InterestRate, rateValue))
                {
                    return previousAdjustment.Guid;
                }

                //和后一个相邻日期利率相同，更新后一个相邻日期到输入日期
                var nextAdjustment = adjustments.FirstOrDefault(x => x.AdjustmentDate > adjustmentDate);
                if (nextAdjustment != null && MathUtils.Equals(nextAdjustment.InterestRate, rateValue))
                {
                    nextAdjustment.AdjustmentDate = adjustmentDate;
                    m_dbAdapter.InterestRateAdjustment.Update(nextAdjustment);
                    return nextAdjustment.Guid;
                }

                //增加一条利率调整
                var adjustment = m_dbAdapter.InterestRateAdjustment.New(interestRateAdjustmentSetId,
                    interestRateType, adjustmentDate, rateValue);
                return adjustment.Guid;
            }
        }

        [HttpPost]
        public ActionResult AddInterestRateAdjustment(string interestRateAdjustmentSetGuid, 
            string interestRateCode, string date, string interestRate)
        {
            return ActionUtils.Json(() =>
            {
                var interestRateAdjustmentSet = m_dbAdapter.InterestRateAdjustmentSet.GetByGuid(interestRateAdjustmentSetGuid);
                CommUtils.Assert(IsCurrentUser(interestRateAdjustmentSet.CreateUserName),
                    "无法增加利率调整，当前用户不是创建者[{0}]", interestRateAdjustmentSet.CreateUserName);

                var interestRateType = CommUtils.ParseEnum<PrimeInterestRate>(interestRateCode);
                var adjustmentDate = DateUtils.ParseDigitDate(date);
                var rateValue = MathUtils.ParseDouble(interestRate);

                var adjustments = m_dbAdapter.InterestRateAdjustment.GetByInterestRateAdjustmentSetId(interestRateAdjustmentSet.Id);
                adjustments = adjustments.Where(x => x.InterestRateType == interestRateType).ToList();
                CommUtils.Assert(adjustments.Count > 0, "找不到InterestRateType={0}", interestRateType.ToString());

                var result = AddRateAdjustment(adjustments, interestRateAdjustmentSet.Id, interestRateType, adjustmentDate, rateValue);
                return ActionUtils.Success(result);
            });
        }
    }
}