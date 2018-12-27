using ChineseAbs.ABSManagement.LogicModels.Authority;
using ChineseAbs.ABSManagement.LogicModels.Team;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class ProjectLogicModel : BaseLogicModel
    {
        public ProjectLogicModel(string userName, int projectId)
            : base(userName, null)
        {
            m_instance = m_dbAdapter.Project.GetProjectById(projectId);
        }

        public ProjectLogicModel(string userName, string projectGuid)
            : base(userName, null)
        {
            m_instance = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
        }

        public ProjectLogicModel(string userName, Project project)
            : base(userName, null)
        {
            m_instance = project;
        }

        private Project m_instance;

        public Project Instance { get { return m_instance; } }

        public bool HasDealModel
        {
            get
            {
                var projectId = m_instance.ProjectId;
                if (m_instance.Model == null && m_instance.ModelId > 0)
                {
                    m_instance.Model = m_dbAdapter.Project.GetModel(m_instance.ModelId);
                }

                var allDataset = m_dbAdapter.Dataset.GetDatasetByProjectId(projectId);
                var validDatasets = allDataset.Where(x => x.PaymentDate.HasValue)
                    .OrderBy(x => x.PaymentDate.Value);
                return validDatasets.Count() > 0;
            }
        }

        /// <summary>
        /// 是否开启预测模式
        /// 预测模式：asOfDateBegin 对应 paymentDay，除第一期外，返回现金流首列是上期的paymentDay
        /// 非预测模式：asOfDateEnd 对应 paymentDay，除第一期外，返回现金流首列是当期的paymentDay
        /// 默认为预测模式
        /// *设置是否为预测模式时，需要调用ABSDealUtils.UpdateDatasetByPredictModel 以更新collateral相关信息
        /// </summary>
        public bool EnablePredictMode
        {
            get
            {
                if (m_enablePredictMode == null)
                {
                    m_enablePredictMode = false;

                    var dealModelSetting = m_dbAdapter.DealModelSetting.GetByProjectId(Instance.ProjectId);
                    m_enablePredictMode = dealModelSetting.EnablePredictMode;
                }

                return m_enablePredictMode.Value;
            }

            set
            {
                var dealModelSetting = m_dbAdapter.DealModelSetting.GetByProjectId(Instance.ProjectId);
                if (dealModelSetting.EnablePredictMode != value)
                {
                    dealModelSetting.EnablePredictMode = value;
                    m_dbAdapter.DealModelSetting.Update(dealModelSetting);
                }
            }
        }

        private bool? m_enablePredictMode;

        public DealScheduleLogicModel DealSchedule
        {
            get
            {
                if (m_dealSchedule == null)
                {
                    m_dealSchedule = new DealScheduleLogicModel(this);
                }

                return m_dealSchedule;
            }
        }

        private DealScheduleLogicModel m_dealSchedule;

        //本地版：从模型中取出的所有兑付日集合
        //非本地版（可以从CNABS站点中获取数据）：从CNABS站点中获取的所有产品对应兑付日的集合
        public List<DateTime> GetAllPaymentDates(IEnumerable<DateTime> allPaymentDates)
        {
            if (m_allPaymentDates == null)
            {
                m_allPaymentDates = allPaymentDates.ToList();
                // 不再使用CNABS站点中的paymentDates信息
                //if (!CommUtils.IsLocalDeployed() && m_instance.CnabsDealId.HasValue)
                //{
                //    m_allPaymentDates = m_dbAdapter.Model.GetPaymentDates(m_instance.CnabsDealId.Value);
                //}
            }
            return m_allPaymentDates;
        }

        private List<DateTime> m_allPaymentDates;

        public TaskGroupLogicModel NewTaskGroup(string projectSeriesGuid, string name, string description)
        {
            ValidateUtils.Name(name, "名称");
            ValidateUtils.Description(description, "描述");

            var taskGroups = m_dbAdapter.TaskGroup.GetByProjectId(m_instance.ProjectId);
            CommUtils.Assert(!taskGroups.Any(x => x.Name == name), "[{0}]中，[{1}]分组已存在", m_instance.Name, name);

            var taskGroup = m_dbAdapter.TaskGroup.NewTaskGroup(m_instance.ProjectId, name, description);
            var logicModel = new TaskGroupLogicModel(this, taskGroup);

            var project = m_dbAdapter.Project.GetProjectById(taskGroup.ProjectId);
            var permissionLogicModel = new PermissionLogicModel(this);
            var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
            permissionLogicModel.SetPermission(projectSeries, project, taskGroup.Guid, PermissionObjectType.TaskGroup);

            Activity.Add(taskGroup.ProjectId, ActivityObjectType.TaskGroup, taskGroup.Guid, "新建工作组：" + name);

            m_taskGroups = null;

            return logicModel;
        }

        public void RemoveTaskGroup(string taskGroupGuid)
        {
            var hasTaskGroupGuid = TaskGroups.Any(x => x.Instance.Guid == taskGroupGuid);
            CommUtils.Assert(hasTaskGroupGuid, "此工作组不存在");

            var taskGroupLogicModel = TaskGroups.Single(x => x.Instance.Guid == taskGroupGuid);
            var taskGroup = taskGroupLogicModel.Instance;
            var tasks = m_dbAdapter.Task.GetByTaskGroupId(taskGroup.Id);

            var taskShortCodes = tasks.Select(x => x.ShortCode);
            var taskPermisssions = m_dbAdapter.Permission.GetByObjectUid(taskShortCodes);
            taskPermisssions.ForEach(x => m_dbAdapter.Permission.DeletePermission(x));

            tasks.ForEach(x => m_dbAdapter.Task.DeleteTask(x));

            //删除项目问题中对应的受阻工作
            var connectTasks = m_dbAdapter.IssueConnectionTasks.GetConnectionTasksByShortCodes(taskShortCodes.ToList());
            m_dbAdapter.IssueConnectionTasks.DeleteConnectionTasks(connectTasks);

            var taskGroupPermissions = m_dbAdapter.Permission.GetByObjectUid(taskGroupGuid);
            taskGroupPermissions.ForEach(x => m_dbAdapter.Permission.DeletePermission(x));

            taskGroup = m_dbAdapter.TaskGroup.RemoveTaskGroup(taskGroup);

            Activity.Add(taskGroup.ProjectId, ActivityObjectType.TaskGroup, taskGroup.Guid, "删除工作组：" + taskGroup.Name);

            m_taskGroups = null;
        }

        public void ModifyTaskGroup(string taskGroupGuid, string name, string description)
        {
            ValidateUtils.Name(name, "名称");
            ValidateUtils.Description(description, "描述");

            var hasTaskGroupGuid = TaskGroups.Any(x => x.Instance.Guid == taskGroupGuid);
            CommUtils.Assert(hasTaskGroupGuid, "此工作组不存在");

            var taskGroupLogicModel = TaskGroups.Single(x => x.Instance.Guid == taskGroupGuid);
            var taskGroup = taskGroupLogicModel.Instance;
            taskGroup.Name = name;
            taskGroup.Description = description;
            m_dbAdapter.TaskGroup.UpdateTaskGroup(taskGroup);

            m_taskGroups = null;
        }

        public List<Note> Notes
        {
            get
            {
                if (m_notes == null)
                {
                    //优先使用CNABS DealID获取数据
                    if (m_instance.CnabsDealId.HasValue)
                    {
                        m_notes = m_dbAdapter.Model.GetNoteByDealId(m_instance.CnabsDealId.Value);
                    }
                    else
                    {
                        var rootFolder = WebConfigUtils.RootFolder;
                        var modelFolder = Path.Combine(rootFolder, m_instance.Model.ModelFolder);
                        var ymlFilePath = modelFolder + @"\Script.yml";

                        CommUtils.Assert(File.Exists(ymlFilePath), "找不到模型文件：[{0}]", ymlFilePath);
                        using (StreamReader sr = new StreamReader(ymlFilePath))
                        {
                            var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                            m_notes = nancyDealData.Notes.ConvertAll(x => new Note()
                            {
                                ShortName = x.Name,
                                NoteName = x.Description,
                                Notional = (decimal)x.Notional,
                                IsEquity = x.IsEquity,
                                SecurityCode = x.Code,
                                CouponString = x.CouponString,
                                ExpectedMaturityDate = x.ExpectedMaturity
                            });
                        }
                    }
                }

                return m_notes;
            }
        }

        protected List<Note> m_notes;

        public List<Dataset> Datasets
        {
            get
            {
                if (m_datasets == null)
                {
                    m_datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(m_instance.ProjectId);
                }

                return m_datasets;
            }
        }

        private List<Dataset> m_datasets = null;

        public List<TaskGroupLogicModel> TaskGroups
        {
            get
            {
                if (m_taskGroups == null)
                {
                    m_taskGroups = m_dbAdapter.TaskGroup.GetByProjectId(m_instance.ProjectId)
                        .ConvertAll(x => new TaskGroupLogicModel(this, x));
                }

                //如果某个工作组没有Sequence，重新设置Sequence
                if (m_taskGroups.Any(x => !x.Instance.Sequence.HasValue))
                {
                    InitSequence();
                }

                m_taskGroups = m_taskGroups.OrderBy(x => x.Instance.Sequence).ToList();

                return m_taskGroups;
            }
        }

        protected List<TaskGroupLogicModel> m_taskGroups;

        private void InitSequence()
        {
            int max = 0;
            if (m_taskGroups.Count() != 0)
            {
                max = m_taskGroups.Max(x => x.Instance.Sequence.HasValue ? x.Instance.Sequence.Value : 0);
            }

            foreach (var taskGroup in m_taskGroups)
            {
                if (!taskGroup.Instance.Sequence.HasValue)
                {
                    taskGroup.Instance.Sequence = ++max;
                    m_dbAdapter.TaskGroup.UpdateTaskGroup(taskGroup.Instance);
                }
            }
        }

        public string ModelFolder
        {
            get
            {
                return Path.Combine(WebConfigUtils.RootFolder, m_instance.Model.ModelFolder);
            }
        }

        public DMSLogicModel DMS
        {
            get
            {
                if (m_dms == null)
                {
                    m_dms = new DMSLogicModel(m_userName, this);
                }

                return m_dms;
            }
        }

        private DMSLogicModel m_dms;

        public ProjectActivityLogicModel Activity
        {
            get
            {
                if (m_projectActivity == null)
                {
                    m_projectActivity = new ProjectActivityLogicModel(this);
                }

                return m_projectActivity;
            }
        }

        private ProjectActivityLogicModel m_projectActivity;

        public AuthorityLogicModel Authority
        {
            get
            {
                if (m_authority == null)
                {
                    m_authority = new AuthorityLogicModel(this);
                }

                return m_authority;
            }
        }

        private AuthorityLogicModel m_authority;

        public TeamLogicModel Team
        {
            get
            {
                if (m_team == null)
                {
                    m_team = new TeamLogicModel(this);
                }

                return m_team;
            }
        }

        private TeamLogicModel m_team;
    }
}
