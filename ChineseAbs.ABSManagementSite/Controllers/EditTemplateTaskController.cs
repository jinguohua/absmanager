using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Filters;
using ChineseAbs.ABSManagementSite.Models;
using FilePattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [DesignAccessAttribute]
    public class EditTemplateTaskController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string templateGuid)
        {
            var template = m_dbAdapter.Template.GetTemplate(templateGuid);
            var templateTask = m_dbAdapter.Template.GetTemplateTasks(template.TemplateId);

            var viewModel = new EditTemplateTaskViewModel();
            viewModel.TemplateGuid = templateGuid;
            viewModel.TemplateName = template.TemplateName;
            viewModel.TemplateTasks = new List<TaskTemplateViewModel>();

            Dictionary<int, int> idMap = new Dictionary<int, int>();
            for (int i = 0; i < templateTask.Count; i++)
            {
                var task = templateTask[i];
                var taskView = Toolkit.ConvertTemplateTask(task);
                taskView.RowSequence = i + 1;
                idMap[task.TemplateTaskId] = taskView.RowSequence;
                viewModel.TemplateTasks.Add(taskView);
            }

            if (viewModel.TemplateTasks != null)
            {
                foreach (var task in viewModel.TemplateTasks)
                {
                    task.PrevTaskNames = Toolkit.ToString(CommUtils.Join(
                        task.PrevTaskIds.ConvertAll(x => idMap[x].ToString())));
                }
            }

            var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(templateGuid);
            viewModel.TemplateTimes = templateTimes.ConvertAll(Toolkit.ConvertTemplateTime);

            return View("../DesignTemplate/EditTemplateTask", viewModel);
        }

        [HttpPost]
        public ActionResult UploadTemplateTaskFile(string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(Request.Files.Count > 0, "请选择文件");

                var file = Request.Files[0];
                CommUtils.Assert(file.ContentLength > 0, "请选择文件");

                CommUtils.Assert(file.FileName.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase),
                    "只支持导入Excel(.xlsx)文件");
                
                var template = m_dbAdapter.Template.GetTemplate(templateGuid);

                file.InputStream.Seek(0, SeekOrigin.Begin);
                var table = ExcelUtils.ParseExcel(file.InputStream, 0, 1, 0, 9);
                CommUtils.Assert(table.Count >= 1, "模板任务为空");

                var templateNames = table.Select(x => x[0].ToString()).ToList();
                var index = templateNames.FindIndex(x => x != template.TemplateName);
                if (index >= 0)
                {
                    CommUtils.Assert(false, "模板工作中包含[{0}]的模板名称，和当前模板[{1}]不一致", templateNames[index], template.TemplateName);
                }

                var excelIds = table.Select(x => x[1].ToString());

                var distinct = excelIds.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                CommUtils.Assert(distinct.Count() == 0, "模板任务中，模板工作ID（" + CommUtils.Join(distinct) + ")不能相同");

                List<TemplateTask> templateTaskList = ParseTemplateTaskTable(table);
                templateTaskList.ForEach(x => x.TemplateId = template.TemplateId);

                TemplateTaskContainer container = new TemplateTaskContainer();
                container.AddRange(templateTaskList);
                container = container.SortByDependency();

                //id in excel, id in db
                var dictId = new Dictionary<int, int>();
                foreach (var record in container)
                {
                    var recordId = record.TemplateTaskId;
                    record.PrevIds = record.PrevIds.ConvertAll(x => dictId[x]);
                    var newRecord = m_dbAdapter.Task.NewTemplateTask(record);
                    var newRecordId = newRecord.TemplateTaskId;
                    dictId[recordId] = newRecordId;
                    LogEditProduct(EditProductType.EditTask, null,
                        "从Excel导入工作模板", "templateId=[" + template.TemplateId + "],新增templateTaskId=[" + newRecordId + "]");
                }

                return ActionUtils.Success(container.Count);
            });
        }

        public ActionResult DownloadTemplateTaskFile(string templateGuid)
        {
            string err = "未知的错误";
            try
            {
                if (string.IsNullOrEmpty(templateGuid))
                {
                    return RedirectToAction("CreateProjet", "MyProjects");
                }

                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var templateTasks = m_dbAdapter.Template.GetTemplateTasks(templateGuid);

                //使用1 2 3~N替代掉TemplateTaskId
                var templateTaskIdDict = new Dictionary<int, int>();
                var templateTaskId = 1;
                templateTasks.ForEach(x =>
                {
                    templateTaskIdDict[x.TemplateTaskId] = templateTaskId;
                    x.TemplateTaskId = templateTaskId;
                    ++templateTaskId;
                });

                //CommUtils.Assert(tasks.Count == 0,"当前模板");
                TemplateTaskPattern patternObj = new TemplateTaskPattern();
                patternObj.TemplateTaskList = templateTasks.ConvertAll(x => new TemplateTaskItem
                {
                    TemplateName = template.TemplateName,
                    TemplateTaskId = x.TemplateTaskId,
                    TemplateTaskName = x.TemplateTaskName,
                    BeginTime = x.BeginDate,
                    EndTime = x.TriggerDate,
                    PrevTemplateTaskIds = string.Join("|",
                        x.PrevIds.Where(prevId => templateTaskIdDict.ContainsKey(prevId)).ToList()
                        .ConvertAll(prevId => templateTaskIdDict[prevId].ToString()).ToArray()),
                    TemplateTaskDetail = x.TemplateTaskDetail,
                    TemplateTaskTarget = x.TemplateTaskTarget,
                    TemplateTaskExtensionName = (x.TemplateTaskExtensionName == null ? "" : x.TemplateTaskExtensionName)
                }).ToList();

                var excelPattern = new ExcelPattern();
                var patternFilePath = DocumentPattern.GetPath(DocPatternType.TemplateTask);
                MemoryStream ms = new MemoryStream();
                if (!excelPattern.Generate(patternFilePath, patternObj, ms))
                {
                    throw new ApplicationException("Generate file failed.");
                }

                string fileFullName = template.TemplateName + ".xlsx";
                string mimeType = "application/xlsx";

                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, mimeType, fileFullName);
            }
            catch (ApplicationException ae)
            {
                err = ae.Message;
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception e)
            {
                err = "服务器错误:" + e.Message;
                return RedirectToAction("NotFound", "Error");
            }
        }

        private List<TemplateTask> ParseTemplateTaskTable(List<List<object>> table)
        {
            var excelIds = table.ConvertAll(x => x[1].ToString());
            var excelTaskNames = table.ConvertAll(x => x[2].ToString());
            var excelPrevids = table.ConvertAll(x => x[5].ToString());
            excelPrevids = excelPrevids.Distinct().ToList();

            for (int i = 0; i < excelTaskNames.Count; i++)
            {
                CommUtils.Assert(excelTaskNames[i].Length <= 30, "模板任务文件解析错误（Row:{0}）:模板工作名称[{1}]的最大长度为30字符数", (i + 1), excelTaskNames[i]);
            }
            //检查模板工作ID只能由数字组成
            for (int i = 0; i < excelIds.Count; i++)
            {
                CommUtils.Assert(DateUtils.CheckIsNumberFormat(excelIds[i]), "模板任务文件解析错误（Row:{0}）:模板工作ID必须由数字组成，不能包含字母和特殊符号", (i + 1));
            }

            //检查模板工作ID中是否存在前置模板工作ID
            for (int i = 0; i < excelPrevids.Count; i++)
            {
                if (!string.IsNullOrEmpty(excelPrevids[i].ToString()))
                {
                    var Previde = excelPrevids[i].ToString().Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    var differentPrevide = Previde.Except(excelIds).ToList();
                    CommUtils.Assert(differentPrevide.Count == 0, "模板任务文件解析错误（Row:{0}）:前置模板工作ID（{1}）在模板工作ID中不存在。"
                        + Environment.NewLine +
                        "注：因为逗号分隔会被Excel识别为金额格式，从而造成前置模板工作ID解析错误，当前系统也支持竖线“|”分隔，并建议使用竖线分割。",
                        (i + 1), CommUtils.Join(differentPrevide));
                }
            }

            List<TemplateTask> templateTaskList = new List<TemplateTask>();
            for (int i = 0; i < table.Count; i++)
            {
                try
                {
                    templateTaskList.Add(ParseTemplateTask(table[i]));
                }
                catch (Exception e)
                {
                    throw new ApplicationException("模板任务文件解析错误（Row:" + (i + 1).ToString() + "）:" + e.Message);
                }
            }

            for (int i = 0; i < templateTaskList.Count; i++)
            {
                try
                {
                    CommUtils.Assert(!string.IsNullOrEmpty(templateTaskList[i].TemplateTaskName), "模板任务文件解析错误（Row:" + (i + 1) + "）:" + "模板工作名称不能为空");
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }

            return templateTaskList;
        }

        private TemplateTask ParseTemplateTask(List<object> objs)
        {
            TemplateTask templateTask = new TemplateTask();
            templateTask.TemplateTaskId = int.Parse(objs[1].ToString());
            templateTask.TemplateTaskGuid = Guid.NewGuid().ToString();
            templateTask.TemplateTaskName = objs[2].ToString();
            templateTask.BeginDate = objs[3].ToString();
            templateTask.TriggerDate = objs[4].ToString();
            templateTask.PrevIds = objs[5].ToString().Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList().ConvertAll(x => int.Parse(x));
            templateTask.TemplateTaskDetail = objs[6].ToString();
            templateTask.TemplateTaskTarget = objs[7].ToString();

            var taskExType = objs[8].ToString();
            if (string.IsNullOrEmpty(taskExType))
            {
                templateTask.TemplateTaskExtensionName = string.Empty;
            }
            else
            {
                templateTask.TemplateTaskExtensionName = CommUtils.ParseEnum<TaskExtensionType>(taskExType).ToString();
            }

            return templateTask;
        }

        [HttpPost]
        public ActionResult DeleteTemplateTask(string templateTaskId)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrEmpty(templateTaskId), "找不到模板工作，请刷新页面后重试");

                int id = int.Parse(templateTaskId);
                var templateTask = m_dbAdapter.Template.GetTemplateTask(id);
                var templateTasks = m_dbAdapter.Template.GetTemplateTasks(templateTask.TemplateId.Value);
                templateTasks = templateTasks.Where(x => x.PrevIds.Contains(id)).ToList();

                if (templateTasks.Count>0)
                {
                    for (int i = 0; i < templateTasks.Count; i++)
                    {
                        templateTasks[i].PrevIds.Remove(id);
                        LogEditProduct(EditProductType.EditTask, null, 
                            "更新模板工作[" + templateTasks[i].TemplateTaskId +
                            "],删除前置任务prev_template_task_ids[" + id + "]", "");
                    }

                    templateTasks.All(x => 1 == m_dbAdapter.Task.UpdateTemplateTask(x));    
                }

                int result = m_dbAdapter.Template.DeleteTemplateTask(templateTask);
                LogEditProduct(EditProductType.CreateProduct, null, "删除模板工作[" + templateTaskId + "][" + templateTask.TemplateTaskName + "]", "");
                CommUtils.Assert(result == 1, "操作异常");

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DeleteTemplateTime(string templateGuid, string templateTimeName)
        {
            return ActionUtils.Json(() =>
            {
                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(template.TemplateId);
                templateTimes = templateTimes.Where(x => x.TemplateTimeName == templateTimeName).ToList();
                CommUtils.Assert(templateTimes.Count != 0,
                    "找不到模板[{0}]中的模板时间[{1}]", template.TemplateName, templateTimeName);
                
                templateTimes.ForEach(x => m_dbAdapter.Template.DeleteTemplateTime(x.TemplateTimeId));
                LogEditProduct(EditProductType.CreateProduct, null, "删除模板时间[" + template.TemplateName + "]["
                    + templateTimeName + "]", "");

                return ActionUtils.Success(templateTimes.Count);
            });
        }

        [HttpPost]
        public ActionResult RemoveAllTemplateTime(string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(templateGuid);
                if (templateTimes.Count != 0)
                {
                    templateTimes.ForEach(x => m_dbAdapter.Template.DeleteTemplateTime(x));
                    var templateTimesIds = CommUtils.Join(templateTimes.ConvertAll(x => x.TemplateTimeId.ToString()));
                    LogEditProduct(EditProductType.EditTask, null,
                        "清空模板时间", "共删除[" + templateTimes.Count + "]条模板时间，templateId=["
                        + templateTimes.FirstOrDefault().TemplateId + "],templateTimeId=[" + templateTimesIds + "]");
                }

                return ActionUtils.Success(templateTimes.Count);
            });
        }

        [HttpPost]
        public ActionResult RemoveAllTemplateTask(string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                var templateTasks = m_dbAdapter.Template.GetTemplateTasks(templateGuid);
                if (templateTasks.Count != 0)
                {
                    templateTasks.ForEach(x => m_dbAdapter.Template.DeleteTemplateTask(x));
                    var templateTasksIds = CommUtils.Join(templateTasks.ConvertAll(x => x.TemplateTaskId.ToString()));
                    LogEditProduct(EditProductType.EditTask, null,
                                "清空模板工作", "共删除[" + templateTasks.Count + "]条模板工作，templateId=[" +
                                templateTasks.FirstOrDefault().TemplateId + "],templateTaskId=[" + templateTasksIds + "]");
                }

                return ActionUtils.Success(templateTasks.Count);
            });
        }

        [HttpPost]
        public ActionResult EstimateTemplateTime(string templateGuid, string templateTimeName)
        {
            return ActionUtils.Json(() =>
            {
                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(template.TemplateId);
                CommUtils.AssertEquals(templateTimes.Count(x => x.TemplateTimeName == templateTimeName), 1,
                    "模板中有多个名称为[{0}]的模板时间", templateTimeName);
                var templateTime = templateTimes.Single(x => x.TemplateTimeName == templateTimeName);
                bool isForward = templateTime.SearchDirection == TemplateTimeSearchDirection.Forward;
                bool ignoreReduplicateDays = templateTime.HandleReduplicate == TemplateTimeHandleReduplicate.Ignore;
                var dateList = DateUtils.GenerateDateList(templateTime.BeginTime, templateTime.EndTime,
                    templateTime.TimeSpan, templateTime.TimeSpanUnit, templateTime.TemplateTimeType, isForward, ignoreReduplicateDays);
                var result = dateList.ConvertAll(x => x.ToString("yyyy-MM-dd"));
                return ActionUtils.Success(result);
            });
        }

        public ActionResult DownloadTemplateTimeFile(string templateGuid)
        {
            string err = "未知的错误";
            try
            {
                if (string.IsNullOrEmpty(templateGuid))
                {
                    return RedirectToAction("CreateProjet", "MyProjects");
                }

                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var timeLists = m_dbAdapter.Template.GetTemplateTimeLists(templateGuid);

                TemplateTimePattern patternObj = new TemplateTimePattern();
                patternObj.TemplateTimeList = timeLists.ConvertAll(x => new TemplateTimeItem
                {
                    TemplateName = template.TemplateName,
                    TemplateTimeName = x.TemplateTimeName,
                    BeginTime = x.BeginTime,
                    EndTime = x.EndTime,
                    TimeSpan = x.TimeSpan,
                    TimeSpanUnit = x.TimeSpanUnit,
                    TemplateTimeType = x.TemplateTimeType,
                    SearchDirection = x.SearchDirection,
                    HandleReduplicate = x.HandleReduplicate
                }).ToList();

                var excelPattern = new ExcelPattern();
                var patternFilePath = DocumentPattern.GetPath(DocPatternType.TemplateTime);

                MemoryStream ms = new MemoryStream();
                if (!excelPattern.Generate(patternFilePath, patternObj, ms))
                {
                    throw new ApplicationException("Generate file failed.");
                }
                string fileFullName = template.TemplateName + "_时间模板" + ".xlsx";
                string mimeType = "application/xlsx";

                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, mimeType, fileFullName);
            }
            catch (ApplicationException ae)
            {
                err = ae.Message;
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception e)
            {
                err = "服务器错误:" + e.Message;
                return RedirectToAction("NotFound", "Error");
            }
        }

        [HttpPost]
        public ActionResult UploadTemplateTimeFile(string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(Request.Files.Count > 0, "请选择文件");

                var file = Request.Files[0];
                CommUtils.Assert(file.ContentLength > 0, "请选择文件");

                CommUtils.Assert(file.FileName.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase),
                    "只支持导入Excel(.xlsx)文件");

                file.InputStream.Seek(0, SeekOrigin.Begin);

                var table = ExcelUtils.ParseExcel(file.InputStream, 0, 1, 0, 9);
                CommUtils.Assert(table.Count > 0, "模板时间为空");

                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var templateNames = table.Select(x => x[0].ToString()).ToList();
                var index = templateNames.FindIndex(x => x != template.TemplateName);
                if (index >= 0)
                {
                    CommUtils.Assert(false, "模板时间中包含[{0}]的模板名称，和当前模板[{1}]不一致", templateNames[index], template.TemplateName);
                }

                //判断excel数据格式并序列化
                List<TemplateTime> templateTimeList = ParseTemplateTimeTable(table);
                templateTimeList.ForEach(x => x.TemplateId = template.TemplateId);

                //数据库里所有的时间名称
                var dbNames = m_dbAdapter.Template.GetTemplateTimeNames(template.TemplateId)
                    .ConvertAll(x => x.TemplateTimeName);

                //excel里的所有时间名称
                var excelNames = table.ConvertAll(x => (string)x[1]);

                CommUtils.AssertEquals(excelNames.Count, excelNames.Distinct().Count(), "模板时间中包含重复的模板时间名称");

                var names = excelNames.Intersect(dbNames).ToList();
                CommUtils.Assert(names.Count == 0, "模板时间名称({0})已存在，请查证后在上传", CommUtils.Join(names));

                var templateTimeIds = templateTimeList.ConvertAll(x => m_dbAdapter.Template.NewTemplateTime(x).TemplateTimeId);
                var newTimeIds = CommUtils.Join(templateTimeIds.ConvertAll(x => x.ToString()));
                LogEditProduct(EditProductType.EditTask, null,
                        "从Excel导入时间模板", "共导入[" + templateTimeList.Count + "]条时间模板，templateId=[" +
                        template.TemplateId + "],新增templateTimeId=[" + newTimeIds + "]");

                CommUtils.AssertEquals(templateTimeIds.Count, templateTimeList.Count, "上传模板失败，模板数据有误");
                return ActionUtils.Success(templateTimeList.Count);
            });
        }

        private List<TemplateTime> ParseTemplateTimeTable(List<List<object>> table)
        {
            List<TemplateTime> templateTimeList = new List<TemplateTime>();
            for (int i = 0; i < table.Count; i++)
            {
                try
                {
                    templateTimeList.Add(ParseTemplateTime(table[i]));
                }
                catch (Exception e)
                {
                    CommUtils.Assert(false, "模板时间文件解析错误（Row:" + (i + 1).ToString() + "）:" + e.Message);
                }
            }
            for (int i = 0; i < templateTimeList.Count; i++)
            {
                try
                {
                    var rowInfo = "（Row:" + (i + 1).ToString() + "）";
                    CommUtils.Assert(DateUtils.CheckIsLetterFormat(templateTimeList[i].TemplateTimeName.ToString()),
                        "模板时间文件解析错误{0}:模板时间名称必须由字母组成，不能包含数字和特殊符号", rowInfo);
                    CommUtils.Assert(DateUtils.CheckIsNumberFormat(templateTimeList[i].TimeSpan.ToString()),
                        "模板时间文件解析错误{0}:时间间隔必须由数字组成，不能包含字母和特殊符号", rowInfo);
                    CommUtils.Assert(templateTimeList[i].BeginTime <= templateTimeList[i].EndTime,
                        "模板时间文件解析错误{0}:结束时间不能小于开始时间", rowInfo);
                }
                catch (Exception e)
                {
                    CommUtils.Assert(false, e.Message);
                }
            }
            return templateTimeList;
        }

        private TemplateTime ParseTemplateTime(List<object> objs)
        {
            TemplateTime templateTime = new TemplateTime();
            templateTime.TemplateTimeName = (string)objs[1];
            templateTime.BeginTime = DateTime.Parse(objs[2].ToString());
            templateTime.EndTime = DateTime.Parse(objs[3].ToString());
            templateTime.TimeSpan = int.Parse(objs[4].ToString());
            templateTime.TimeSpanUnit = CommUtils.ParseEnum<TimeSpanUnit>((string)objs[5]);
            templateTime.TemplateTimeType = CommUtils.ParseEnum<TemplateTimeType>((string)objs[6]);
            templateTime.SearchDirection = CommUtils.ParseEnum<TemplateTimeSearchDirection>((string)objs[7]);
            templateTime.HandleReduplicate = CommUtils.ParseEnum<TemplateTimeHandleReduplicate>((string)objs[8]);
            return templateTime;
        }

        private void LogEditProduct(EditProductType type, int? projectId, string description, string comment)
        {
            m_dbAdapter.Project.NewEditProductLog(type, projectId, description, comment);
        }
    }
}