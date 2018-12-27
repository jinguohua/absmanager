using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using ChineseAbs.ABSManagement.Object;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension;
using ChineseAbs.ABSManagementSite.Models;
using CNABS.Mgr.Deal.Utils;
using SFL.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Common
{
    public static class Toolkit
    {
        public static string GenereteGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string FormatHtmlText(string text)
        {
            return text.Replace(" ", "&nbsp;")
                .Replace(Environment.NewLine, "<br>")
                .Replace("\r", "<br>")
                .Replace("\n", "<br>");
        }

        public static string GetTaskStatusStyle(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Skipped:
                    return "text-align:center;";
                case TaskStatus.Waitting:
                case TaskStatus.Running:
                    return "text-align:center;color:#3995cd";
                case TaskStatus.Finished:
                    return "text-align:center;color:#55FF55";
                default:
                    return "text-align:center;color:#ff4A4A";
            }
        }

        public static string GetTaskExCheckListStatusStyle(string checkListStatus)
        {
            var status = CommUtils.ParseEnum<TaskExCheckType>(checkListStatus);
            switch (status)
            {
                case TaskExCheckType.Checked:
                    return "color:#55FF55";
                case TaskExCheckType.Unchecked:
                    return "color:#ff4A4A";
                default:
                    return "";
            }
        }

        public static string ToCnString(ProjectStatus status)
        {
            switch (status)
            {
                case ProjectStatus.Undefined:
                    return "未定义";
                case ProjectStatus.CollectInfo:
                    return "收集信息";
                case ProjectStatus.InObservation:
                    return "观察";
                case ProjectStatus.Normal:
                    return "正常";
                default:
                    throw new ApplicationException("未定义的ProjectStatus");
            }
        }

        public static string ToCnString(DmsFileSeriesTemplateType templateType)
        {
            switch (templateType)
            {
                case DmsFileSeriesTemplateType.None:
                    return "无";
                case DmsFileSeriesTemplateType.IncomeDistributionReport:
                    return "收益分配报告";
                case DmsFileSeriesTemplateType.SpecialPlanTransferInstruction:
                    return "专项计划划款指令";
                case DmsFileSeriesTemplateType.CashInterestRateConfirmForm:
                    return "兑付兑息确认表";
                case DmsFileSeriesTemplateType.InterestPaymentPlanApplication:
                    return "付息方案申请";
                default:
                    throw new ApplicationException("未定义的DmsFileSeriesTemplateType");
            }
        }

        public static string LongTitleDisplay(string origin, int maxlen)
        {
            if (origin.Length < maxlen)
            {
                return origin;
            }
            else
            {
                return origin.Substring(0,maxlen-2) + "...";
            }
        }

        public static string PaymentFrequency(int paymentPerYear)
        {
            switch (paymentPerYear)
            {
                case 26:
                    return "两周付";
                case 12:
                    return "月付";
                case 4:
                    return "季付";
                case 2:
                    return "半年付";
                case 1:
                    return "年付";
                default:
                    return "未知的付款频率";
            }
        }
        
        public static string PaymentFrequency(string s)
        {
            switch (s){
                case "TwoWeekly":
                    return "两周付";
                
                case "Monthly":
                    return "月付";
                
                case "Quarterly":
                    return "季付";
                
                case "SemiAnnually":
                    return "半年付";
                
                case "Annually":
                    return "年付";
                case "1":
                    return "年付";
                default:
                    return "未知的付款频率";
            }
        }

        public static string AccrualMethod(ZEnums.EAccrualMethod accrualMethod) 
        {
            switch (accrualMethod)
            {
                case ZEnums.EAccrualMethod.UNDEFINEDENUM:
                    return "UNDEFINED";
                case ZEnums.EAccrualMethod.Actual360:
                    return "Act/360";
                case ZEnums.EAccrualMethod.Actual365:
                    return "ACT/365";
                case ZEnums.EAccrualMethod.Thirty360:
                case ZEnums.EAccrualMethod.Thirty360BMA:
                case ZEnums.EAccrualMethod.Thirty360European:
                case ZEnums.EAccrualMethod.Thirty360Italian:
                case ZEnums.EAccrualMethod.Thirty360USA:
                    return "30/360";
                case ZEnums.EAccrualMethod.ActualActual:
                case ZEnums.EAccrualMethod.ActualActualAFB:
                case ZEnums.EAccrualMethod.ActualActualBond:
                case ZEnums.EAccrualMethod.ActualActualEuro:
                case ZEnums.EAccrualMethod.ActualActualHistorical:
                case ZEnums.EAccrualMethod.ActualActualISDA:
                case ZEnums.EAccrualMethod.ActualActualISMA:
                    return "Act/Act";
                default:
                    return "undefined AccrualMethod";
            }
 
        }

        public static string ToCnString(TaskStatus status)
        {
            return TranslateUtils.ToCnString(status);
        }

        public static string ConvertCompareSign(string compareSign)
        {
            switch (compareSign)
            {
                case "NotCompare":
                    return "不比较";
                case "Equal":
                    return "=";
                case "NotEqual":
                    return "≠";
                case "GreaterThan":
                    return ">";
                case "LessThan":
                    return "<";
                case "GreateThanEqual":
                    return "≥";
                case "LessThanEqual":
                    return "≤";
                default:
                    throw new ApplicationException("未定义的CompareSign");
            }
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string DateTimeToString(DateTime? dateTime)
        {
            return dateTime == null ? "-" : DateTimeToString(dateTime.Value);
        }

        public static string DateToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string TimeToString(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        public static string DateToString(DateTime? dateTime)
        {
            return dateTime.HasValue ? DateToString(dateTime.Value) : "-";
        }

        public static string TimeToString(DateTime? dateTime)
        {
            return dateTime.HasValue ? TimeToString(dateTime.Value) : "-";
        }

        public static string ToString(int? value)
        {
            return value.HasValue ? value.Value.ToString() : "-";
        }

        public static string ToString(string value)
        {
            return string.IsNullOrEmpty(value) ? "-" : value;
        }


        public static string ToString(double? value)
        {
            return value.HasValue ? value.Value.ToString() : "-";
        }

        public static string ToPercentString(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("p2") : "-";
        }

        public static string To10KString(decimal? value)
        {
            return value.HasValue ? (value.Value / 10000m).ToString("n2") : "-";
        }

        public static PageInfo ConvertPageInfo<T>(Page<T> page) where T : new()
        {
            PageInfo pageInfo = new PageInfo
            {
                CurrentPage = page.CurrentPage,
                ItemsPerPage = page.ItemsPerPage,
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            };

            return pageInfo;
        }
        
        public static TemplateTimeViewModel ConvertTemplateTime(TemplateTime templateTime)
        {
            var view = new TemplateTimeViewModel()
            {
                TemplateTimeId = templateTime.TemplateTimeId,
                TemplateId = templateTime.TemplateId,
                TemplateTimeName = templateTime.TemplateTimeName,
                BeginTime = templateTime.BeginTime,
                EndTime = templateTime.EndTime,
                TimeSpan = templateTime.TimeSpan,
                TimeSpanUnit = templateTime.TimeSpanUnit.ToString(),
                TemplateTimeType = templateTime.TemplateTimeType.ToString(),
                SearchDirection = templateTime.SearchDirection.ToString(),
                HandleReduplicate = templateTime.HandleReduplicate.ToString()
            };
            return view;
        }

        public static TaskTemplateViewModel ConvertTemplateTask(TemplateTask templateTask)
        {
            var view =  new TaskTemplateViewModel
            {
                Id = templateTask.TemplateTaskId,
                Name = templateTask.TemplateTaskName,
                BeginDate = ToString(templateTask.BeginDate),
                TriggerDate = ToString(templateTask.TriggerDate),
                PrevTaskNames = ToString(CommUtils.Join(templateTask.PrevIds.ConvertAll(x => x.ToString()))),
                ExtensionName = templateTask.TemplateTaskExtensionName,
                PrevTaskIds = new List<int>(),
                Detail = templateTask.TemplateTaskDetail,
                Target = templateTask.TemplateTaskTarget,
            };
            view.PrevTaskIds.AddRange(templateTask.PrevIds);
            return view;
        }

        public static ProjectViewModel ConvertProject(Project project)
        {
            ProjectViewModel projectView = new ProjectViewModel
            {
                Id = project.ProjectId,
                Guid = project.ProjectGuid,
                ProjectName = project.Name,
                ProjectStatus = ProjectStatus.Normal,
                NextCalcDate = DateToString(project.NextCalcDate),
                //下个偿付日应该按当前日期更新
                NextPaymentDate = DateToString(project.NextPaymentDate),
                ProjectType = project.DealType,
                Message = 0
            };

            if (project.ProjectDealInfo != null)
            {
                var dealInfo = project.ProjectDealInfo;
                //这里的CurrentPaymentDate会在cnabs更新后更新
                //projectView.NextPaymentDate = DateToString(dealInfo.CurrentPaymentDate);
                projectView.PersonInCharge = dealInfo.Issuer;
                projectView.Originator = dealInfo.Originator;
            }

            return projectView;
        }

        public static TaskViewModel ConvertTask(Task task)
        {
            TaskViewModel taskView = new TaskViewModel();
            taskView.Id = task.TaskId.ToString();
            taskView.ProjectName = task.ProjectName;
            taskView.TaskName = task.Description.Trim();
            taskView.StartTime = DateToString(task.StartTime);
            taskView.EndTime = DateToString(task.EndTime);
            taskView.ShortCode = task.ShortCode;
            taskView.PrevTasksNames = ToString(task.PreTaskIds);
            taskView.Status = task.TaskStatus;
            taskView.PrevTaskShortCodeArray = new List<string>();
            if (task.PrevTaskShortCodeArray != null)
            {
                taskView.PrevTaskShortCodeArray.AddRange(task.PrevTaskShortCodeArray);
            }

            taskView.TaskDetail = task.TaskDetail;
            taskView.TaskTarget = task.TaskTarget;
            taskView.TaskHandler = task.TaskHandler;

            if (task.TaskExtension != null)
            {
                taskView.TaskExtension = ConvertTaskExtension(task.TaskExtension);
            }

            return taskView;
        }

        public static TaskStatusHistoryViewModel ConvertTaskStatusHistory(TaskStatusHistory obj)
        {
            TaskStatusHistoryViewModel view = new TaskStatusHistoryViewModel();
            view.TaskStatusHistoryId = obj.TaskStatusHistoryId;
            view.TaskId = obj.TaskId;
            view.PrevStatus = obj.PrevStatusId;
            view.NewStatus = obj.NewStatusId;
            view.TimeStamp = obj.TimeStamp;
            view.TimeStampUserName = obj.TimeStampUserName;
            view.Comment = obj.Comment;
            return view;
        }

        public static CashflowViewModel ConvertTaskExtensionCashflow(CashflowViewModel obj)
        {
            return new CashflowViewModel()
            {
                OverridableVariables = obj.OverridableVariables,
                PaymentDates = obj.PaymentDates
            };
        }

        public static string ConvertTaskExType(string taskExTypeName)
        {
            var taskExType = CommUtils.ParseEnum<TaskExtensionType>(taskExTypeName);
            switch (taskExType)
            {
                case TaskExtensionType.AssetCashflow:
                    return "资产端现金流";
                case TaskExtensionType.Cashflow:
                    return "证券端现金流";
                case TaskExtensionType.Document:
                    return "文档上传下载";
                case TaskExtensionType.RecyclingPaymentDate:
                    return "确认账户余额";
                case TaskExtensionType.CheckList:
                    return "工作要点检查";
                case TaskExtensionType.DemoJianYuanReport:
                    return "建元Demo（上传Excel生成报告）";
                default:
                    return "-";
            }
        }

        public static string ConvertTaskExCheckType(string taskExCheckType)
        {
            var checkType = CommUtils.ParseEnum<TaskExCheckType>(taskExCheckType);
            switch (checkType)
            {
                case TaskExCheckType.Checked:
                    return "已检查";
                case TaskExCheckType.Unchecked:
                    return "未检查";
                default:
                    throw new ApplicationException("未定义的检查类型");
            }
        }

        public static TaskExtensionViewModel ConvertTaskExtension(TaskExtension obj)
        {
            TaskExtensionViewModel view = new TaskExtensionViewModel();
            view.Id = obj.TaskExtensionId;
            view.Name = obj.TaskExtensionName;
            view.Type = obj.TaskExtensionType;
            var info = obj.TaskExtensionInfo;
            if (!string.IsNullOrEmpty(info))
            {
                if (view.Type == "Cashflow")
                {
                    view.Info = ConvertTaskExtensionCashflow(CommUtils.FromJson<CashflowViewModel>(info));
                }
            }
            view.Status = obj.TaskExtensionStatus;
            view.Handler = obj.TaskExtensionHandler;
            view.HandleTime = obj.TaskExtensionHandleTime;

            return view;
        }

        public static NoteInfoViewModel GetNoteInfoViewModel(Note note)
        {
            var noteInfoView = new NoteInfoViewModel();
            noteInfoView.Name = note.NoteName;
            noteInfoView.ShortName = note.ShortName;
            noteInfoView.SecurityCode = note.SecurityCode;
            noteInfoView.Notional = note.Notional;
            noteInfoView.CouponString = note.CouponString;
            noteInfoView.ExpectedMaturityDate = note.ExpectedMaturityDate;
            noteInfoView.AccrualMethon = note.AccrualMethod;
            return noteInfoView;
        }

        #region Payment history

        public static DatasetViewModel GetDatasetViewModel(Dataset dataset, DateTime[] paymentDates, Dictionary<int, NoteInfoViewModel> noteDict, List<NoteData> noteDatas)
        {
            var datasetViewModel = new DatasetViewModel();
            datasetViewModel.NoteDatas = new List<NoteDataViewModel>();

            foreach (var noteData in noteDatas)
            {
                if (noteData.HasValue)
                {
                    var noteDetailView = new NoteDataViewModel();
                    noteDetailView.PaymentDetail = new PaymentDetail
                    {
                        PrincipalPaid = noteData.PrincipalPaid,
                        InterestPaid = noteData.InterestPaid,
                        EndingBalance = noteData.EndingBalance
                    };

                    noteDetailView.NoteInfo = noteDict[noteData.NoteId];
                    noteDetailView.CurrentCouponRate = "-";

                    datasetViewModel.NoteDatas.Add(noteDetailView);
                }
            }

            datasetViewModel.SumPaymentDetail = new PaymentDetail
            {
                PrincipalPaid = noteDatas.Sum(x => x.PrincipalPaid),
                InterestPaid = noteDatas.Sum(x => x.InterestPaid),
                EndingBalance = noteDatas.Sum(x => x.EndingBalance)
            };

            datasetViewModel.PaymentDay = dataset.PaymentDate;
            if (dataset.PaymentDate.HasValue && paymentDates != null)
            {
                datasetViewModel.Sequence = paymentDates.ToList().IndexOf(dataset.PaymentDate.Value) + 1;
            }

            return datasetViewModel;
        }

        /// <summary>
        /// 获取 noteId到 NoteInfoViewModel的映射
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, NoteInfoViewModel> GetNoteDictionary(Project project, List<Note> notes, List<Note> cnabsNotes)
        {
            var noteDict = new Dictionary<int, NoteInfoViewModel>();

            foreach (var cnabsNote in cnabsNotes)
            {
                var subNotes = notes.Where(x => x.ShortName == cnabsNote.ShortName).ToList();
                if (subNotes.Count == 1)
                {
                    noteDict[subNotes[0].NoteId] = Toolkit.GetNoteInfoViewModel(cnabsNote);
                }
            }

            return noteDict;
        }

        #endregion

        /// <summary>
        /// 将AssetDataTable转换为AssetCashflowStatisticInfo
        /// </summary>
        /// <param name="dtAssetCashflow">AssetDataTable</param>
        /// <param name="specifyDate">指定统计信息位于第几期</param>
        /// <returns></returns>
        public static AssetCashflowStatisticInfo GetAssetCashflow(DataTable dtAssetCashflow, DateTime? specifyDate = null)
        {
            int currentDatasetColumnIndex = -1;
            if (!specifyDate.HasValue)
            {
                specifyDate = new DateTime(1970, 1, 1);
            }

            for (int i = 0; i < dtAssetCashflow.Columns.Count; ++i)
            {
                DateTime temp;
                if (DateTime.TryParse(dtAssetCashflow.Columns[i].ColumnName, out temp))
                {
                    if (temp >= specifyDate.Value)
                    {
                        currentDatasetColumnIndex = i;
                        break;
                    }
                }
            }

            var rt = new AssetCashflowStatisticInfo();
            var dt = ABSDealUtils.CleanAndTranslateAssetCashflowTable(dtAssetCashflow);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.AsEnumerable())
                {
                    var element = new AssetCashflowElementData();
                    element.AssetId = dr["资产"] != null ? dr["资产"].ToString() : "";
                    element.Description = dr["项目"] != null ? dr["项目"].ToString() : "";

                    if (element.AssetId == "总计")
                    {
                        if (element.Description == "总利息")
                        {
                            rt.TotalCurrentInterestCollection = double.Parse(dr[currentDatasetColumnIndex].ToString());
                        }
                        else if (element.Description == "总本金")
                        {
                            rt.TotalCurrentPrinCollection = double.Parse(dr[currentDatasetColumnIndex].ToString());
                        }
                    }
                }
            }
            return rt;
        }

        public static void GetCashflow(CashflowViewModel vm, DataTable dtCashflow)
        {
            var dt = ABSDealUtils.CleanAndTranslateCashflowTable(dtCashflow);
            if (dt != null && dt.Rows.Count > 0)
            {
                var paymentDateList = new List<DateTime>();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName != "项目描述" && dc.ColumnName != "类型")
                    {
                        paymentDateList.Add(DateTime.Parse(dc.ColumnName));
                    }
                }
                vm.PaymentDates = paymentDateList.OrderBy(d => d.Date).ToArray();

                foreach (DataRow dr in dt.AsEnumerable().Where(r => !r["类型"].ToString().Equals("") && !r["类型"].ToString().Equals("-")))
                {
                    var element = new CashflowElementData();
                    element.Type = dr["类型"] != null ? dr["类型"].ToString() : "";
                    element.Description = dr["项目描述"] != null ? dr["项目描述"].ToString() : "";
                    for (int i = 2; i < dt.Columns.Count; i++)
                    {
                        if (dr[i].ToString() == "FAIL")
                        {
                            vm.TestFailRemind = "预测将发生违约，启动差额支付";
                        }
                        element.Values.Add(new DateValuePair() { PaymentDate = DateTime.Parse(dt.Columns[i].ColumnName), Value = dr[i].ToString() });
                    }
                    vm.CashflowList.Add(element);
                }

            }
        }

        public static void GetCashflowDetailData(CashflowDetailTableViewModel viewModel, DataTable dtCashflow, List<Note> note, List<NoteData> noteDatas)
        {
            var dictColumn = new Dictionary<string, List<string>>();
            dictColumn["Collateral"] = new List<string>() { "n0" };
            dictColumn["Fees"] = new List<string>() { "n0" };
            dictColumn["Reserve Accounts"] = new List<string>() { "n0" };
            dictColumn["Tests"] = new List<string>() { "n0" };
            dictColumn["Notes"] = new List<string>() { "n0" };

            foreach (var dc in dtCashflow.AsEnumerable())
            {
                if (dc[0].ToString() == "Category")
                {
                    for (int i = 0; i < dc.ItemArray.Length; i++)
                    {
                        var type = dc.ItemArray[i].ToString();
                        if (dictColumn.Keys.Contains(type))
                        {
                            var columnName = dtCashflow.Columns[i].ToString();
                            dictColumn[type].Add(columnName);
                        }
                    }
                }
            }
            var categoryTable = TranslateTableEnToCn(dtCashflow.DefaultView.ToTable(false, dictColumn["Collateral"].Distinct().ToArray()));
            var feesTable = TranslateTableEnToCn(dtCashflow.DefaultView.ToTable(false, dictColumn["Fees"].Distinct().ToArray()));
            var accountTable = TranslateTableEnToCn(dtCashflow.DefaultView.ToTable(false, dictColumn["Reserve Accounts"].Distinct().ToArray()));
            var testsTable = TranslateTableEnToCn(dtCashflow.DefaultView.ToTable(false, dictColumn["Tests"].Distinct().ToArray()));

            var notesTableEn = dtCashflow.DefaultView.ToTable(false, dictColumn["Notes"].Distinct().ToArray());
            var notesTable = TranslateNotesTableEnToCn(notesTableEn);

            //获取当期的中证手续费应付
            DateTime time;

            var descriptionRows = dtCashflow.AsEnumerable().Where(x => x[0].ToString() == "Description");
            var currPeriodRows = dtCashflow.AsEnumerable().Where(x =>
                DateTime.TryParse(x[0].ToString(), out time)
                && time.ToString("yyyy-MM-dd") == viewModel.CurrentPaymentDate);
            if (currPeriodRows.Count() == 1)
            {
                var descriptionRow = descriptionRows.First();
                var currPeriodRow = currPeriodRows.First();

                for (int i = 1; i < dtCashflow.Columns.Count; i++)
                {
                    if (descriptionRow[i].ToString().Contains("中证手续费应付"))
                    {
                        viewModel.FeePayable = currPeriodRow[i].ToString();
                        break;
                    }
                }
            }

            //获取当期的触发事件
            var eventDescriptionRows = testsTable.AsEnumerable().Where(x => x[0].ToString() == "Description");
            var eventDataRows = testsTable.AsEnumerable().Where(x => 
                DateTime.TryParse(x[0].ToString(), out time)
                && time.ToString("yyyy-MM-dd") == viewModel.CurrentPaymentDate);
            if (eventDataRows.Count() == 1)
            {
                var eventDescriptionRow = eventDescriptionRows.First();
                var eventDataRow = eventDataRows.First();

                for (int i = 1; i < testsTable.Columns.Count; i++)
                {
                    viewModel.CashflowEventList.Add(new CashflowHomePageEvent
                    {
                        EventKey = eventDescriptionRow[i].ToString(),
                        EventValue = eventDataRow[i].ToString()
                    });
                }
            }

            //获取当期的资产池与证券信息。
            GetHomePageAssetPool(viewModel, categoryTable);
            GetHomePageAssetSecurity(viewModel, notesTableEn, note, noteDatas);

            viewModel.TestFailRemind = CheckTestFailRemind(testsTable);
            viewModel.AssetPoolHeader = GetTableHeader(categoryTable);
            viewModel.CostHeader = GetTableHeader(feesTable);
            viewModel.AccountHeader = GetTableHeader(accountTable);
            viewModel.TriggerEventHeader = GetTableHeader(testsTable);

            viewModel.AssetPoolList = GetCashflowTableData(categoryTable);
            viewModel.AccountList = GetCashflowTableData(accountTable);
            viewModel.CostList = GetCashflowTableData(feesTable);
            viewModel.TriggerEventList = GetCashflowTableData(testsTable);

            //获取产品现金流表 (本金支付 + 收到的利息)
            var projectCashflowData= GetProjectCashflowData(notesTable);
            viewModel.ProjectCashflowHeader = projectCashflowData.Item1;
            viewModel.ProjectCashflowList = projectCashflowData.Item2;

            //获取证券现金流表
            viewModel.SecurityCashflowList = GetSecurityCashflowData(notesTable);
            viewModel.SecurityCashflowHeader = new List<string>() { "偿付日期", "期初本金", "本金偿付", "利息偿付", "总和" };
            viewModel.HomePageHeader = new List<string>() { "", "期初本金", "本金兑付", "利息兑付", "期末本金", "每份兑付本金", "每份兑付利息" };
        }

        private static string CheckTestFailRemind(DataTable table)
        {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                DataRow dr = table.Rows[rowIndex];
                DateTime time;
                if (dr[0] == null || !DateTime.TryParse(dr[0].ToString(), out time))
                {
                    continue;
                }

                for (int i = 1; i < table.Columns.Count; i++)
                {
                    var value = dr[i].ToString();
                    if (value == "FAIL")
                    {
                        return "预测将发生违约，启动差额支付";
                    }
                }

            }
            return null;
        }

        private static void GetHomePageAssetSecurity(CashflowDetailTableViewModel viewModel,
            DataTable notesTable, List<Note> notes, List<NoteData> noteDatas)
        {
            DateTime time;
            var currPaymentDataRows = notesTable.AsEnumerable().Where(x =>
                DateTime.TryParse(x[0].ToString(), out time)
                && time.ToString("yyyy-MM-dd") == viewModel.CurrentPaymentDate);

            var shortNames = notes.ConvertAll(x => x.ShortName);
            var dictUnitCount = notes.ToDictionary(x => x.ShortName, y=> (int)(y.Notional.Value / 100));

            var dict = new Dictionary<String, string> {
                { "Beginning Principal Outstanding", "期初本金" },
                { "Principal Received", "本金支付" },
                { "Beginning Interest Due", "预计利息金额" },
                { "Interest Received", "收到的利息" },
                { "Deferred Interest Received", "收到逾期利息" },
                { "Interest Deferred", "未偿利息" },
                { "Equity Distribution", "次级收入" },
                { "CsdcFee", "中证手续费" },
            };

            if (currPaymentDataRows.Count() == 1)
            {
                foreach (var shortName in shortNames)
                {
                    var data = new CashflowData();
                    data.RowName = (shortName == "Sub" ? "次级" : ("优先级" + shortName));
                    var currPaymentDataRow = currPaymentDataRows.First();

                    var dictRowData = new Dictionary<string, double>();
                    dict.Keys.ToList().ForEach(x => dictRowData[x] = 0.0);

                    for (int i = 0; i < notesTable.Columns.Count; i++)
                    {
                        var type = notesTable.Rows[0][i].ToString();
                        var description = notesTable.Rows[1][i].ToString();

                        var note = shortNames.SingleOrDefault(x => description.StartsWith(x + "."));
                        if (type == "Notes" && !string.IsNullOrWhiteSpace(note)
                            && shortName == note)
                        {
                            var keyword = description.Substring(note.Length);
                            dictRowData[keyword.Substring(1)] = double.Parse(currPaymentDataRow[i].ToString());
                        }
                    }
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "期初本金", Value = dictRowData["Beginning Principal Outstanding"].ToString("n2") });
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "本金兑付", Value = dictRowData["Principal Received"].ToString("n2") });
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "利息兑付", Value = (dictRowData["Interest Received"] + dictRowData["Deferred Interest Received"]).ToString("n2") });
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "期末本金", Value = (dictRowData["Beginning Principal Outstanding"] - dictRowData["Principal Received"]).ToString("n2") });
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "每份兑付本金", Value = (dictRowData["Principal Received"] / dictUnitCount[shortName]).ToString("n3") });
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "每份兑付利息", Value = ((dictRowData["Interest Received"] + dictRowData["Deferred Interest Received"]) / dictUnitCount[shortName]).ToString("n3") });

                    viewModel.CurrPeriodCashflowInfoList.Add(data);
                }
            }
        }

        private static void GetHomePageAssetPool(CashflowDetailTableViewModel viewModel, DataTable assetPoolTable)
        {
            DateTime time;
            var descriptionRows = assetPoolTable.AsEnumerable().Where(x => x[0].ToString() == "Description");
            var currPaymentDataRows = assetPoolTable.AsEnumerable().Where(x =>
                DateTime.TryParse(x[0].ToString(), out time)
                && time.ToString("yyyy-MM-dd") == viewModel.CurrentPaymentDate);

            if (currPaymentDataRows.Count() == 1)
            {
                var descriptionRow = descriptionRows.First();
                var currPaymentDataRow = currPaymentDataRows.First();

                var performing = 0.0;
                var interestCollection = 0.0;
                var principalCollection = 0.0;

                for (int i = 1; i < assetPoolTable.Columns.Count; i++)
                {
                    var description = descriptionRow[i].ToString();
                    var value = currPaymentDataRow[i].ToString();

                    if (description == "剩余本金")
                    {
                        performing = double.Parse(value);
                    }
                    if (description == "本金收入账户")
                    {
                        principalCollection = double.Parse(value);
                    }
                    if (description == "利息收入账户")
                    {
                        interestCollection = double.Parse(value);
                    }
                }

                var data = new CashflowData();
                data.RowName = "资产池";
                data.Values.Add(new CashflowDateValuePair() { ColumnName = "期初本金", Value = (performing + principalCollection).ToString("n2") });
                data.Values.Add(new CashflowDateValuePair() { ColumnName = "本金兑付", Value = principalCollection.ToString("n2") });
                data.Values.Add(new CashflowDateValuePair() { ColumnName = "利息兑付", Value = interestCollection.ToString("n2") });
                data.Values.Add(new CashflowDateValuePair() { ColumnName = "期末本金", Value = performing.ToString("n2") });
                data.Values.Add(new CashflowDateValuePair() { ColumnName = "每份兑付本金", Value = "-" });
                data.Values.Add(new CashflowDateValuePair() { ColumnName = "每份兑付利息", Value = "-" });

                viewModel.CurrPeriodCashflowInfoList.Add(data);
            }
        }

        private static List<string> GetTableHeader(DataTable dtCashflow)
        {
            var categoryNameList = new List<string>();
            categoryNameList.Add("偿付日期");
            categoryNameList.AddRange(dtCashflow.Rows[1].ItemArray.ToList().ConvertAll(x => x.ToString()));

            if (categoryNameList.Contains("Description"))
            {
                categoryNameList.Remove("Description");
            }

            categoryNameList = categoryNameList.Distinct().ToList();

            return categoryNameList;
        }

        private static Dictionary<string, List<CashflowData>> GetSecurityCashflowData(DataTable dtCashflow)
        {
            var result = new Dictionary<string, List<CashflowData>>();

            var categoryNameList = dtCashflow.Rows[0].ItemArray.ToList().ConvertAll(x => x.ToString());

            if (categoryNameList.Contains("Category"))
            {
                categoryNameList.Remove("Category");
            }
            categoryNameList = categoryNameList.Distinct().ToList();
            categoryNameList.ForEach(x => result[x] = new List<CashflowData>());

            foreach (var categoryName in categoryNameList)
            {
                var table = new List<CashflowData>();
                for (int rowIndex = 0; rowIndex < dtCashflow.Rows.Count; rowIndex++)
                {
                    DataRow dr = dtCashflow.Rows[rowIndex];
                    DateTime time;
                    if (dr[0] == null || !DateTime.TryParse(dr[0].ToString(), out time))
                    {
                        continue;
                    }

                    var rowName = time.ToString("yyyy-MM-dd");
                    var data = new CashflowData();
                    data.RowName = rowName;

                    var receiveInterest = 0.0;
                    var receiveOverdueInterest = 0.0;
                    var principalPay = 0.0;

                    for (int i = 1; i < dtCashflow.Columns.Count; i++)
                    {
                        if (categoryName == dtCashflow.Rows[0][i].ToString())
                        {
                            var columnName = dtCashflow.Rows[1][i].ToString();

                            if (columnName == "期初本金")
                            {
                                var value = dr[i].ToString();
                                data.Values.Add(new CashflowDateValuePair() { ColumnName = columnName, Value = value });
                            }
                            if (columnName == "本金支付")
                            {
                                var value = dr[i].ToString();
                                principalPay = Double.Parse(value);
                                data.Values.Add(new CashflowDateValuePair() { ColumnName = columnName, Value = value });
                            }
                            if (columnName == "收到的利息")
                            {
                                receiveInterest = Double.Parse(dr[i].ToString());
                            }
                            if (columnName == "收到逾期利息")
                            {
                                receiveOverdueInterest = Double.Parse(dr[i].ToString());
                            }
                        }
                    }

                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "利息偿付", Value = (receiveInterest + receiveOverdueInterest).ToString("n2") });
                    data.Values.Add(new CashflowDateValuePair() { ColumnName = "总和", Value = (receiveInterest + receiveOverdueInterest + principalPay).ToString("n2") });
                    table.Add(data);
                }
                result[categoryName] = table;
            }

            return result;
        }

        private static Tuple<List<string>, List<CashflowData>> GetProjectCashflowData(DataTable dtCashflow)
        {
            var result = new List<CashflowData>();
            var categoryNameList = dtCashflow.Rows[0].ItemArray.ToList().ConvertAll(x => x.ToString());

            if (categoryNameList.Contains("Category"))
            {
                categoryNameList.Remove("Category");
            }
            categoryNameList = categoryNameList.Distinct().ToList();

            for (int rowIndex = 0; rowIndex < dtCashflow.Rows.Count; rowIndex++)
            {
                DataRow dr = dtCashflow.Rows[rowIndex];
                DateTime time;
                if (dr[0] == null || !DateTime.TryParse(dr[0].ToString(), out time))
                {
                    continue;
                }

                var rowName = time.ToString("yyyy-MM-dd");
                var data = new CashflowData();
                data.RowName = rowName;

                foreach (var categoryName in categoryNameList)
                {
                    var principal = 0.0;
                    var interest = 0.0;

                    for (int i = 1; i < dtCashflow.Columns.Count; i++)
                    {
                        if (categoryName == dtCashflow.Rows[0][i].ToString())
                        {
                            var columnName = dtCashflow.Rows[1][i].ToString();

                            if (columnName == "本金支付")
                            {
                                principal = Double.Parse(dtCashflow.Rows[rowIndex][i].ToString());
                            }
                            if (columnName == "收到的利息")
                            {
                                interest = Double.Parse(dtCashflow.Rows[rowIndex][i].ToString());
                            }
                        }
                    }

                    data.Values.Add(new CashflowDateValuePair() { ColumnName = categoryName, Value = (principal + interest).ToString("n2") });
                }

                result.Add(data);
            }

            var tableHeader = new List<string>();
            tableHeader.Add("偿付日期");
            tableHeader.AddRange(categoryNameList);

            return new Tuple<List<string>, List<CashflowData>>(tableHeader, result);
        }

        private static List<CashflowData> GetCashflowTableData(DataTable dtCashflow)
        {
            var result = new List<CashflowData>();

            for (int rowIndex = 0; rowIndex < dtCashflow.Rows.Count; rowIndex++)
            {
                DataRow dr = dtCashflow.Rows[rowIndex];
                DateTime time;
                if (dr[0] == null || !DateTime.TryParse(dr[0].ToString(), out time))
                {
                    continue;
                }

                var rowName = time.ToString("yyyy-MM-dd");
                var data = new CashflowData();
                data.RowName = rowName;

                for (int i = 1; i < dtCashflow.Columns.Count; i++)
                {
                    var type = dtCashflow.Rows[0][i].ToString();
                    var columnName = dtCashflow.Rows[1][i].ToString();
                    var translateName = m_dictSubCategories.Keys.Contains(columnName) ?
                        columnName.Replace(columnName, m_dictSubCategories[columnName]) : columnName;
                    var value = dr[i].ToString();

                    data.Values.Add(new CashflowDateValuePair() { ColumnName = translateName, Value = value });
                }

                result.Add(data);
            }

            return result;
        }

        private static DataTable TranslateNotesTableEnToCn(DataTable cashflowDt)
        {
            var result = cashflowDt.Copy();
            var notes = new List<string>();

            for (int i = 0; i < result.Columns.Count; i++)
            {
                var type = result.Rows[0][i].ToString();
                var description = result.Rows[1][i].ToString();
                var key = ".Principal Received";
                if (type == "Notes" && description.EndsWith(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    var note = description.Substring(0, description.Length - key.Length);
                    notes.Add(note);
                }
            }

            for (int i = 0; i < result.Columns.Count; i++)
            {
                var type = result.Rows[0][i].ToString();
                var description = result.Rows[1][i].ToString();

                var note = notes.SingleOrDefault(x => description.StartsWith(x + "."));
                if (type == "Notes" && !string.IsNullOrWhiteSpace(note))
                {
                    var keyword = description.Substring(note.Length);
                    result.Rows[0][i] = (note == "Sub" ? "次级" : ("优先级" + note));
                    result.Rows[1][i] = Translate(keyword.Substring(1));
                }
            }

            return result;
        }

        private static string Translate(string content)
        {
            var dict = new Dictionary<String, string> {
                { "Beginning Principal Outstanding", "期初本金" },
                { "Principal Received", "本金支付" },
                { "Beginning Interest Due", "预计利息金额" },
                { "Interest Received", "收到的利息" },
                { "Deferred Interest Received", "收到逾期利息" },
                { "Interest Deferred", "未偿利息" },
                { "Equity Distribution", "次级收入" },
                { "CsdcFee", "中证手续费" },
            };
            if (dict.ContainsKey(content))
            {
                return dict[content];
            }

            return content;
        }

        private static DataTable TranslateTableEnToCn(DataTable table)
        {
            var typeList = table.Rows[0].ItemArray.ToList().ConvertAll(x => x.ToString());
            var descriptionList = table.Rows[1].ItemArray.ToList().ConvertAll(x => x.ToString());
            foreach (var dc in table.AsEnumerable())
            {
                if (dc[0].ToString() == "Category")
                {
                    typeList = dc.ItemArray.ToList().ConvertAll(x => x.ToString());
                    for (int i = 0; i < typeList.Count; i++)
                    {
                        foreach (var key in m_dictCategories.Keys)
                        {
                            if (typeList[i].Contains(key))
                            {
                                dc[i] = dc[i].ToString().Replace(key, m_dictCategories[key]);
                            }
                        }
                    }
                }
                if (dc[0].ToString() == "Description")
                {
                    descriptionList = dc.ItemArray.ToList().ConvertAll(x => x.ToString());

                    for (int i = 0; i < descriptionList.Count; i++)
                    {
                        foreach (var key in m_dictSubCategories.Keys)
                        {
                            if (descriptionList[i].Contains(key))
                            {
                                dc[i] = dc[i].ToString().Replace(key, m_dictSubCategories[key]);
                            }
                        }
                    }
                }
            }

            return table;
        }

        public static Dictionary<int, string> AddAssetIdToRepeatedCNName(DataTable table, List<AssetLogicModel> assets)
        {
            var rowSpansDic = SetRowSpansDic(table);
            List<string> acfTableCNNames = new List<string>();
            for (int iRow = 0; iRow < table.Rows.Count; iRow++)
            {
                var cnName = table.Rows[iRow][0].ToString();
                if (rowSpansDic[iRow]["rowSpan"] != 0 && cnName != string.Empty)
                {
                    acfTableCNNames.Add(cnName);
                }
            }

            var assetsCNNames = assets.Select(x => x.SecurityData.SecurityName).ToList();
            var diffAcfTableCNNames = acfTableCNNames.Distinct().ToList();
            var diffAssetsCNNames = assetsCNNames.Distinct().ToList();
            CommUtils.Assert(diffAcfTableCNNames.Count == diffAssetsCNNames.Count, "资产表格行数与资产数量不相等");

            var assetIdNameMap = new Dictionary<int, string>();

            object lastName = string.Empty;
            int iAssetsRow = 0;
            for (int iRow = 0; iRow < table.Rows.Count; iRow++)
            {
                var acfTableRow = table.Rows[iRow];
                CommUtils.Assert(acfTableRow.ItemArray.Length > 0, "");
                if (table.Rows[iRow][0].ToString() == string.Empty)
                {
                    continue;
                }
                if (rowSpansDic[iRow]["rowSpan"] != 0)
                {
                    iAssetsRow++;
                    lastName = acfTableRow[0].ToString();
                }

                CommUtils.Assert(iAssetsRow >= 1 && iAssetsRow < assets.Count + 1, "资产assets下标[{0}]越界({1}-{2})", iAssetsRow, 1, assets.Count + 1);
                var assetId = assets[iAssetsRow - 1].AssetId;
                var acfTableCNName = acfTableCNNames[iAssetsRow - 1];
                var assetsCNName = assetsCNNames[iAssetsRow - 1];

                var acfTableCNNameCount = acfTableCNNames.FindAll(x => x == acfTableCNName).ToList().Count();
                var assetsCNNameCount = assetsCNNames.FindAll(x => x == assetsCNName).ToList().Count();
                CommUtils.Assert(acfTableCNNameCount == assetsCNNameCount, "资产[{0}]数量有误", acfTableCNName);

                if (acfTableCNNameCount > 1)
                {
                    if (rowSpansDic[iRow]["rowSpan"] != 0)
                    {
                        lastName = acfTableCNName + "(" + assetId.ToString() + ")";
                        assets[iAssetsRow - 1].SecurityData.SecurityName = lastName.ToString();
                    }
                    table.Rows[iRow][0] = lastName;
                    table.Rows[iRow][0] = lastName;

                }

                assetIdNameMap[assetId] = table.Rows[iRow][0].ToString();
            }

            return assetIdNameMap;
        }

        public static Dictionary<int, Dictionary<string, int>> SetRowSpansDic(DataTable table)
        {
            CommUtils.Assert(table.Rows.Count > 0, "表table不能为空");
            var prevRow = table.Rows[0];
            var prevRowId = 0;
            var names = new List<String>();
            var rowSpansDic = new Dictionary<int, Dictionary<string, int>>();
            rowSpansDic[0] = new Dictionary<string, int>();

            for (int i = 1; i < table.Rows.Count; i++)
            {
                rowSpansDic[i] = new Dictionary<string, int>();
                rowSpansDic[i]["rowSpan"] = 1;
                var row = table.Rows[i];

                var categoryOfRow = row[0].ToString();
                var categoryOfPrevRow = prevRow[0].ToString();
                var nameOfRow = row[1].ToString();

                if (categoryOfRow == categoryOfPrevRow && (!names.Contains(nameOfRow)))
                {
                    if (!rowSpansDic[prevRowId].Keys.Contains("rowSpan"))
                    {
                        rowSpansDic[prevRowId]["rowSpan"] = 1;
                    }

                    rowSpansDic[prevRowId]["rowSpan"] += 1;

                    rowSpansDic[i]["rowSpan"] = 0;
                }
                else
                {
                    names = new List<String>();
                    prevRow = row;
                    prevRowId = i;
                }
                names.Add(nameOfRow);
            }

            return rowSpansDic;
        }

        public static DataTable CleanCashflowTable(DataTable cashflowDt)
        {
            DataTable rt = new DataTable();

            rt = cashflowDt.AsEnumerable().Where(x =>
                    (!m_ignoredCategories.Contains(x[0].ToString()) && !m_ignoredSubCategories.Contains(x[1].ToString()))
                    || (x[0].ToString() == "Variables" && x[1].ToString() == "中证手续费应付")
                    ).CopyToDataTable();

            var variableIndex = rt.IndexOfRow(x => x[0].ToString() == "Variables" && x[1].ToString() == "中证手续费应付");
            var accountIndex = rt.IndexOfRow(x => x[0].ToString() == "Reserve Accounts");
            if (variableIndex != -1 && accountIndex != -1)
            {
                var rowVariable = rt.Rows[variableIndex];
                DataRow newRow = rt.NewRow();
                // We "clone" the row
                newRow.ItemArray = rowVariable.ItemArray;
                // We remove the old and insert the new
                rt.Rows.Remove(rowVariable);
                rt.Rows.InsertAt(newRow, accountIndex);
            }

            for (int i = rt.Columns.Count - 1; i >= 0; i--)
            {
                var c = rt.Columns[i];
                if (c.ColumnName.Equals("Total"))
                {
                    rt.Columns.Remove(c);
                }
            }

            rt.RemoveAllRow(x => x[1].ToString() == "TotalExpense.End Balance"
                || x[1].ToString() == "OtherIncome.End Balance");

            for (int i = rt.Rows.Count - 1; i >= 0; i += -1)
            {
                DataRow row = rt.Rows[i];
                if ((row[0] == null || row[0].ToString() == "") && (row[1] == null || row[1].ToString() == ""))
                {
                    rt.Rows.Remove(row);
                }
                else if (string.IsNullOrEmpty(row[0].ToString()) && string.IsNullOrEmpty(row[1].ToString()))
                {
                    rt.Rows.Remove(row);
                }
            }

            return rt;
        }

        [Obsolete]
        private static void TranslateEn2CnVariable(DataSet dsVariables)
        {
            foreach (DataRow row in dsVariables.Tables["CurrentVariables"].Rows)
            {
                foreach (var key in m_dicCurrentVariables.Keys)
                {
                    if (row[0].ToString().IndexOf(key) > -1)
                    {
                        row[0] = row[0].ToString().Replace(key, m_dicCurrentVariables[key]);
                    }
                }
            }
            foreach (DataRow row in dsVariables.Tables["FutureVariables"].Rows)
            {
                foreach (var key in m_dicFutureVariables.Keys)
                {
                    if (row[0].ToString().IndexOf(key) > -1)
                    {
                        row[0] = row[0].ToString().Replace(key, m_dicFutureVariables[key]);
                    }
                }
            }
        }

        public static string TranslateEn2CnFutureVariable(string name)
        {
            foreach (var key in m_dicFutureVariables.Keys)
            {
                name = name.Replace(key, m_dicFutureVariables[key]);
            }
            return name;
        }

        public static string TranslateEn2CnVariable(string key)
        {
            var rt = key;
            if (key.Contains("Collateral") || key.Equals("InvestmentAccount"))
            {
                foreach (var cn in m_dicCurrentVariables.Keys)
                {
                    rt = rt.Replace(cn, m_dicCurrentVariables[cn]);
                }
            }
            else
            {
                foreach (var cn in m_dicFutureVariables.Keys)
                {
                    rt = rt.Replace(cn, m_dicFutureVariables[cn]);
                }
            }
            return rt;
        }

        private static string[] m_ignoredCategories = new string[] {
            "Floating Indexes",
            "Test Detail",
            "Variables",
            "Macros"
        };

        private static string[] m_ignoredSubCategories = new string[] {
            "TransferToPrincipal.End Balance",
            "Margin_1",
            "Margin_2",
            "Margin_3",
            "Guarantee_1",
            "Guarantee_2",
            "Guarantee_3",
            "InvestmentAccount.End Balance"
        };

        private static Dictionary<string, string> m_dictCategories = new Dictionary<string, string>()
        {
            {"Floating Indexes","参考利率"},
            {"Collateral","资产池"},
            {"Fees","费用"},
            {"Reserve Accounts","账户"},
            {"Accounts","账户"},
            {"Notes","支持证券"},
            {"Tests","事件结果"},
            {"Test Detail","事件计算"},
            {"Variables","自定义变量"},
            {"Macros","宏变量"}
        };

        private static Dictionary<string, string> m_dictSubCategories = new Dictionary<string, string>()
        {
            {"APB","剩余本金"},
            {"Loss.Loss","实际损失"},
            {"Loss","损失"},
            {"InterestCollection","利息收入账户"},
            {"PrincipalCollection.Unscheduled","非计划本金收入"},
            {"PrincipalCollection","本金收入账户"},
            {"MarginAccount","保证金"},
            {"Margin","差额支付事件"},
            {"Guarantee","担保事件"},
            {".Amount Due","预计支付"},
            {".Received","的实际支付"},
            {".Beginning Deferred","的逾期金额"},
            {".Deferred Received","的逾期金额支付"},
            {".Deferred Interest Received","收到逾期利息"},
            {".Interest Deferred","的未偿利息"},
            {".Interest On Deferred Interest Received","收到逾期复利"},
            {".End Balance","余额"},
            {".Beginning Principal Outstanding","的期初本金"},
            {".Principal Received","的本金支付"},
            {".Beginning Interest Due","的预计利息金额"},
            {".Interest Received","收到的利息"},
            {".Equity Distribution","的次级收入"},
            {".CsdcFee", "中证手续费" },
            {"EOD_TEST","违约事件测试"},
            {"EOD","违约事件"},
            {"Turbo","加速清偿事件"},
            {"TransferToPrincipal","转入本金"},
            {"Reinvestment", "循环购买"},
            {"ExpenseAccount", "费用账户"},
            //{"InvestmentAccount","合格投资收益" },
            {"ReserveAmount", "留存账户"},
            {"Sub","次级" },
            {"Trustee","托管" },
            {"Service","服务" },
            {"Fee","费" }
        };

        private static Dictionary<string, string> m_dicAssetCashflowType = new Dictionary<string, string>()
        {
            {"Loss","损失"},
            {"Interest","利息"},
            {"Principal","本金"},
            {"Performing","剩余本金"},
            {"Defaulted","违约"},
            {"Received","合计"},
            {"Total ","总"},
            {"Total Received","合计"},
            {"Fee","费用" }
        };


        private static string[] m_assetIgnoredCashflowType = new string[] 
        {
            "Total Loss PV",
            "Cumulative Defaulted"
        };


        private static Dictionary<string, string> m_dicCurrentVariables = new Dictionary<string, string>()
        {
            {".Loss.Amount","损失"},
            {".InterestCollection","利息回收款"},
            {".PrincipalCollection","本金回收款"},
            {"Collateral","资产池"},
            {"InvestmentAccount", "当期合格投资收益" }
        };

        private static Dictionary<string, string> m_dicFutureVariables = new Dictionary<string, string>()
        {
            {"Fee","费用"},
            {"Expense","其他费用"},
            {"Audit","审计"},
            {"Rating","跟踪评级"},
            {"Trustee","托管" },
            {"OtherIncomeMax","其它收益" },
            {"Reserve","留存" },
        };

        private static Dictionary<string, string> m_dicCurrentVariablesCn2En = new Dictionary<string, string>()
        {
            {"损失",".Loss.Amount"},
            {"利息回收款",".InterestCollection"},
            {"本金回收款",".PrincipalCollection"},
            {"资产池","Collateral"},
            {"当期合格投资收益","InvestmentAccount"}
        };

        private static Dictionary<string, string> m_dicFutureVariablesCn2En = new Dictionary<string, string>()
        {
            {"其他费用","Expense"},
            {"费用","Fee"},
            {"审计","Audit"},
            {"跟踪评级","Rating"},
            {"托管","Trustee" },
            {"留存","Reserve" },
        };
    }
}