using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public enum EditProductType
    {
        CreateProduct = 1,
        GenerateTask = 2,
        EditTask = 3,
        DeleteDataset = 4,
        EditPermission = 5,
        CreateTask = 6,
        DeleteProduct = 7,
        EditDownloadFileAuthority = 8,
        UpdateScriptYml=9
    }

    public class ProjectManager : BaseManager
    {
        public ProjectManager()
        {
            m_defaultTableName = "dbo.Project";
            m_defaultPrimaryKey = "project_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public string NewProject(string projectName, int modelId, int? cnabsDealId)
        {
            ABSMgrConn.TableProject project = new ABSMgrConn.TableProject()
            {
                name = projectName,
                create_user_name = UserInfo.UserName,
                create_time = DateTime.Now,
                project_guid = Guid.NewGuid().ToString(),
                model_id = modelId,
                cnabs_deal_id = cnabsDealId,
                record_status_id = (int)RecordStatus.Valid
            };

            NewProject(project);
            return project.project_guid;
        }

        public void NewEditProductLog(EditProductType type, int? projectId, string description, string comment)
        {
            ABSMgrConn.TableEditProductLogs log = new ABSMgrConn.TableEditProductLogs();
            log.project_id = projectId;
            log.edit_product_log_type = (int)type;
            log.description = description;
            log.comment = comment;
            log.time_stamp = DateTime.Now;
            log.time_stamp_user_name = UserInfo.UserName;
            NewEditProductLog(log);
        }

        public int NewProject(ABSMgrConn.TableProject project)
        {
            return Insert("Project", "project_id", project);
        }

        public void NewEditProductLog(ABSMgrConn.TableEditProductLogs log)
        {
            m_db.Insert("EditProductLogs", "edit_product_log_id", true, log);
        }

        public int Update(Project project)
        {
            return m_db.Update("dbo.Project", "project_id", project.GetTableObject());
        }

        public void Remove(Project project)
        {
            project.RecordStatus = RecordStatus.Deleted;
            Update(project);
        }

        private class ProjectCreateUserInfo
        {
            public string project_guid { get; set; }
            public DateTime? create_time { get; set; }
            public string create_user_name { get; set; }
            public string time_stamp_user_name { get; set; }
            public string time_stamp { get; set; }
        }

        private string GetProjectCreatorInLog(Project project)
        {
            var sql = "SELECT project.project_guid, "
                + "project.create_time, "
                + "project.create_user_name, "
                + "log.time_stamp_user_name, "
                + "log.time_stamp "
                + "FROM dbo.EditProductLogs AS log "
                + "RIGHT JOIN dbo.Project AS project "
                + "ON log.description = '创建Product[' + project.name + '][' + project.project_guid + ']' "
                + "WHERE log.time_stamp_user_name IS NOT NULL "
                + "AND project_guid = @0";

            var records = m_db.Fetch<ProjectCreateUserInfo>(sql, project.ProjectGuid);
            if (records.Count == 1)
            {
                var createUserName = records.First().time_stamp_user_name;
                if (!string.IsNullOrWhiteSpace(createUserName))
                {
                    return createUserName;
                }
            }

            return string.Empty;
        }

        public Project GetProjectByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableProject>("project_guid", guid);
            var project = new Project(record);
            if (project.ModelId > 0)
            {
                project.Model = GetModel(project.ModelId);
            }
            return project;
        }

        public ProjectDealInfo GetProjectDealInfoByProjectId(int projectId)
        {
            var vwProjects = m_db.Query<ABSMgrConn.VwProject>(
                "SELECT * FROM dbo.vw_project WHERE project_id = @0  AND record_status_id <> @1",
                projectId, (int)RecordStatus.Deleted);
            var vwProject = vwProjects.Single();
            var projectDealInfo = new ProjectDealInfo();
            projectDealInfo.FromTableObject(vwProject);
            return projectDealInfo;
        }

        public Project GetProjectById(int projectId)
        {
            var record = SelectSingle<ABSMgrConn.TableProject>(projectId);
            var project = new Project(record);
            if (project.ModelId > 0)
            {
                project.Model = GetModel(project.ModelId);
            }
            return project;
        }

        public Page<Project> GetProjects(long pageNum, long itemsPerPage, bool isLoadDealInfo, List<int> authorizedProjectIds)
        {
            if (authorizedProjectIds.Count == 0)
            {
                return new Page<Project>
                {
                    Items = new List<Project>()
                };
            }

            var sqlCondition = "(" + string.Join(", ", authorizedProjectIds.ConvertAll(x => x.ToString())) + ")";
            var page = m_db.Page<ABSMgrConn.TableProject>(pageNum, itemsPerPage,
                "SELECT * FROM dbo.Project WHERE project_id IN "
                + sqlCondition + " AND record_status_id <> @0" + m_orderBy, (int)RecordStatus.Deleted);

            var projects = new Page<Project>().Parse(page);
            projects.Items = page.Items.ConvertAll(item => new Project(item));

            if (isLoadDealInfo)
            {
                if (projects.Items.Any(x => x.CnabsDealId.HasValue))
                {
                    var vwProjects = m_db.Fetch<ABSMgrConn.VwProject>(
                        "SELECT * FROM dbo.vw_project WHERE project_id IN (@authorizedProjectIds)" + m_orderBy, new { authorizedProjectIds });

                    var cnabsDealIds = projects.Items.Where(x => x.CnabsDealId.HasValue).Select(x => x.CnabsDealId);
                    var vwDeals = m_db.Fetch<ABSMgrConn.VwDeal>(
                        "SELECT * FROM dbo.vw_deal WHERE deal_id IN (@cnabsDealIds)", new { cnabsDealIds });

                    var vwPaymentDates = m_db.Fetch<ABSMgrConn.VwPaymentDates>(
                        "SELECT * FROM " + m_db.ChineseABSDB + "[dbo].[PaymentDates] WHERE deal_id IN (@cnabsDealIds) ORDER BY payment_date", new { cnabsDealIds });

                    var nowDate = DateTime.Today;

                    foreach (var project in projects.Items)
                    {
                        var vwProject = vwProjects.FirstOrDefault(x => x.project_id == project.ProjectId);
                        if (vwProject != null)
                        {
                            if (vwProject.deal_name_chinese != null || vwProject.issuer != null
                                || vwProject.total_offering != null || vwProject.frequency != null)
                            {
                                project.ProjectDealInfo = new ProjectDealInfo();
                                project.ProjectDealInfo.FromTableObject(vwProject);
                            }
                        }

                        //Get deal type
                        if (project.CnabsDealId.HasValue)
                        {
                            var vwDeal = vwDeals.FirstOrDefault(x => x.deal_id == project.CnabsDealId.Value);
                            if (vwDeal != null)
                            {
                                project.DealType = vwDeal.type;
                            }

                            var vwPaymentDate = vwPaymentDates
                                .Where(x => x.deal_id == project.CnabsDealId.Value && x.payment_date.HasValue)
                                .FirstOrDefault(x => x.payment_date.Value >= nowDate);
                            if (vwPaymentDate != null)
                            {
                                project.NextPaymentDate = vwPaymentDate.payment_date.Value;
                            }
                        }
                    }
                }

                if (projects.Items.Any(x => !x.CnabsDealId.HasValue))
                {
                    var rootFolder = WebConfigUtils.RootFolder;
                    foreach (var project in projects.Items)
                    {
                        if (project.CnabsDealId.HasValue || project.ModelId <= 0)
                        {
                            continue;
                        }

                        project.Model = GetModel(project.ModelId);

                        var modelFolder = Path.Combine(rootFolder, project.Model.ModelFolder);
                        var ymlFilePath = modelFolder + @"\Script.yml";
                        if (File.Exists(ymlFilePath))
                        {
                            using (StreamReader sr = new StreamReader(ymlFilePath))
                            {
                                var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                                if (nancyDealData != null)
                                {
                                    project.DealType = nancyDealData.DealTypeString;
                                    project.ProjectDealInfo = new ProjectDealInfo();
                                    project.ProjectDealInfo.Originator = nancyDealData.Originator;

                                    var paymentDates = nancyDealData.ScheduleData.PaymentSchedule.GetPaymentDates();
                                    var nowDate = DateTime.Today;
                                    if (paymentDates.Any(x => x >= nowDate))
                                    {
                                        project.NextPaymentDate = paymentDates.First(x => x >= nowDate);
                                    }
                                    else
                                    {
                                        project.NextPaymentDate = paymentDates.Last();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return projects;
        }

        public List<Project> GetProjects(List<int> authorizedProjectIds)
        {
            if (authorizedProjectIds.Count == 0)
            {
                return new List<Project>();
            }

            var sqlCondition = "(" + string.Join(", ", authorizedProjectIds.ConvertAll(x => x.ToString())) + ")";
            var projects = m_db.Query<ABSMgrConn.TableProject>(
                "SELECT * FROM dbo.Project WHERE project_id IN " + sqlCondition
                + " AND record_status_id <> @0" + m_orderBy, (int)RecordStatus.Deleted);

            var result = projects.Select(x => new Project(x)).ToList();
            result.ForEach(GetProjectCreateUserName);
            return result;
        }

        //如果找不到产品的创建者，去log中查找产品的创建信息，更新到project中
        private void GetProjectCreateUserName(Project project)
        {
            var createUserName = project.CreateUserName;
            if (createUserName == null)
            {
                createUserName = GetProjectCreatorInLog(project);
                project.CreateUserName = createUserName ?? string.Empty;
                Update(project);
                project.CreateUserName = createUserName;
            }
        }

        public List<Project> GetProjectsHasDealModel(List<int> authorizedProjectIds)
        {
            if (authorizedProjectIds.Count == 0)
            {
                return new List<Project>();
            }

            var sqlCondition = "(" + string.Join(", ", authorizedProjectIds.ConvertAll(x => x.ToString())) + ")";
            var projects = m_db.Query<ABSMgrConn.TableProject>(
                "SELECT * FROM dbo.Project WHERE project_id IN " + sqlCondition +
                " AND project_id IN (SELECT DISTINCT(project_id) FROM dbo.Dataset) "
                + " AND record_status_id <> @0 " + m_orderBy, (int)RecordStatus.Deleted);

            return projects.ToList().ConvertAll(item => new Project(item)).ToList();
        }

        public List<Project> GetByProjectSeriesId(int projectSeriesId)
        {
            var projects = Select<ABSMgrConn.TableProject>("dbo.Project", "project_series_id", projectSeriesId);
            return projects.ToList().ConvertAll(x => new Project(x));
        }

        public Model GetModel(int modelId)
        {
            var tableModel = m_db.Single<ABSMgrConn.TableModels>(
                "SELECT * from dbo.Models where model_id = @0", modelId);

            return new Model(tableModel);
        }

        public string GetProjectNameById(int projectId)
        {
            return m_db.ExecuteScalar<string>("select name from dbo.Project where project_id = @0", projectId);
        }
        
        private const string m_orderBy = " ORDER BY create_time desc, project_id desc";
    }
}
