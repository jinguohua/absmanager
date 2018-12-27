using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class ProjectSeriesLogicModel : BaseLogicModel
    {
        public ProjectSeriesLogicModel(string userName, int projectSeriesId)
            : base(userName, null)
        {
            m_projectSeries = m_dbAdapter.ProjectSeries.GetById(projectSeriesId);
        }

        public ProjectSeriesLogicModel(string userName, string projectSeriesGuid)
            : base(userName, null)
        {
            m_projectSeries = m_dbAdapter.ProjectSeries.GetByGuid(projectSeriesGuid);
        }

        public ProjectSeriesLogicModel(string userName)
            : base(userName, null)
        {
            m_projectSeries = null;
        }

        public ProjectSeriesLogicModel(string userName, ProjectSeries projectSeries)
            : base(userName, null)
        {
            m_projectSeries = projectSeries;
        }

        // 创建一个项目
        // 创建后给创建者增加读、写、操作的权限
        public ProjectSeriesLogicModel NewProjectSeries(string name, string projectSeriesType, string personInCharge, string createTime, string estimatedFinishTime, string email)
        {
            ValidateUtils.Name(name, "项目名称").FileName();

            CommUtils.Assert(email.Length <= 38, "邮箱不能超过38个字符数！");
            CommUtils.AssertHasContent(personInCharge, "[项目负责人]不能为空");
            CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(personInCharge), "[用户]不存在");

            CommUtils.AssertHasContent(createTime, "[立项日期]不能为空");
            CommUtils.AssertHasContent(estimatedFinishTime, "[计划完成日期]不能为空");

            var valStartTime = DateTime.Parse(createTime);
            var valEstimatedFinishTime = DateTime.Parse(estimatedFinishTime);
            CommUtils.Assert(valEstimatedFinishTime >= valStartTime, "计划完成日期[{0}]必须大于等于立项日期[{1}]", valEstimatedFinishTime, valStartTime);

            var type = CommUtils.ParseEnum<ProjectSeriesType>(projectSeriesType);
            var projectSeries = m_dbAdapter.ProjectSeries.NewProjectSeries(name, type, personInCharge, valStartTime, valEstimatedFinishTime, email);

            m_projectSeries = projectSeries;

            var now = DateTime.Now;
            ABSMgrConn.TableProject project = new ABSMgrConn.TableProject()
            {
                project_guid = Guid.NewGuid().ToString(),
                project_series_id = projectSeries.Id,
                name = projectSeries.Name,
                type_id = (int)ProjectSeriesStage.发行,
                model_id = -1,
                cnabs_deal_id = null,
                create_time = DateTime.Parse(now.ToString("yyyy-MM-dd")),
                create_user_name = m_userName,
                time_stamp = now,
                time_stamp_user_name = m_userName,
                record_status_id = (int)RecordStatus.Valid
            };
            project.project_id = m_dbAdapter.Project.NewProject(project);
            m_currentProject = new ProjectLogicModel(m_userName, new Project(project));

            var adminUsernames = new List<string> { projectSeries.CreateUserName, projectSeries.PersonInCharge };
            adminUsernames = adminUsernames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            
            //给[创建者、负责人]添加ProjectSeries与Project的所有权限
            foreach (var adminUsername in adminUsernames)
            {
                m_dbAdapter.Permission.NewPermission(adminUsername, project.project_guid,
                PermissionObjectType.Project, PermissionType.Read);

                m_dbAdapter.Permission.NewPermission(adminUsername, project.project_guid,
                    PermissionObjectType.Project, PermissionType.Write);

                m_dbAdapter.Permission.NewPermission(adminUsername, project.project_guid,
                    PermissionObjectType.Project, PermissionType.Execute);

                m_dbAdapter.Permission.NewPermission(adminUsername, projectSeries.Guid,
                PermissionObjectType.ProjectSeries, PermissionType.Read);

                m_dbAdapter.Permission.NewPermission(adminUsername, projectSeries.Guid,
                    PermissionObjectType.ProjectSeries, PermissionType.Write);

                m_dbAdapter.Permission.NewPermission(adminUsername, projectSeries.Guid,
                    PermissionObjectType.ProjectSeries, PermissionType.Execute);
            }

            return this;
        }

        public ProjectLogicModel CurrentProject
        {
            get
            {
                if (m_currentProject == null)
                {
                    var projects = m_dbAdapter.Project.GetByProjectSeriesId(m_projectSeries.Id);
                    projects = projects.Where(x => x.TypeId.HasValue && x.TypeId == (int)m_projectSeries.Stage).ToList();
                    CommUtils.Assert(projects.Count < 2, "[{0}]中，处于[{1}]的项目数不唯一", m_projectSeries.Name, m_projectSeries.Stage);
                    //CommUtils.Assert(projects.Count > 0, "[{0}]中不包含处于[{1}]阶段的项目", m_projectSeries.Name, m_projectSeries.Stage);
                    if (projects.Count == 1)
                    {
                        var project = projects.Single();
                        m_currentProject = new ProjectLogicModel(m_userName, project);
                    }
                }

                return m_currentProject;
            }
        }

        public ProjectSeries Instance { get { return m_projectSeries; } }

        private ProjectLogicModel m_currentProject;

        private ProjectSeries m_projectSeries;
    }
}
