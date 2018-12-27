using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Db
{
    public class DBAdapter
    {
        public DBAdapter()
        {
            m_db = new Database("ABSMgrConn");
        }

        public Project SelectOneProject(string projectName)
        {
            var projects = m_db.Page<Project>(1, 10, "SELECT * FROM dbo.Project WHERE name = @0 ORDER BY id desc", projectName);
            if (projects.Items.Count == 0)
            {
                throw new ApplicationException("工程【" + projectName + "】不存在");
            }
            else if (projects.Items.Count > 1)
            {
                throw new ApplicationException("检测到含有重复工程名称【" + projectName + "】的工程");
            }

            return projects.Items[0];
        }

        public bool TaskExists(string taskName)
        {
            string sql = @"select count(*) from dbo.Task
                            where name=@0";
            int count = m_db.ExecuteScalar<int>(sql, taskName);
            return count > 0;
        }

        public bool ProjectExists(string projectName)
        {
            string sql = @"select count(*) from dbo.Project
                            where name=@0";
            int count = m_db.ExecuteScalar<int>(sql, projectName);
            return count > 0;
        }

        public void DeleteProject(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ApplicationException("工程名称不能为空");
            }

            var project = SelectOneProject(projectName);

            var deleteRows = m_db.Delete(project);
            if (deleteRows == 0)
            {
                throw new ApplicationException("删除工程【" + projectName + "】失败");
            }
            else if (deleteRows >= 2)
            {
                throw new ApplicationException("删除了" + deleteRows + "个工程【" + projectName + "】");
            }
        }

        public void NewTask(string projectName, Task task)
        {
            if (TaskExists(task.task_name))
            {
                throw new ApplicationException("任务【" + task.task_name + "】已经存在，不能重复创建");
            }

            m_db.Insert(task);

            var taskInProject = new TaskInProject
            {
                ProjectName = projectName,
                TaskName = task.task_name
            };

            m_db.Insert(taskInProject);
        }


        public void NewProject(Project project)
        {
            if (ProjectExists(project.name))
            {
                throw new ApplicationException("工程【" + project.name + "】已经存在，不能重复创建");
            }

            ABSMgrConnDB.GetInstance().Insert(project);
        }

        public Page<Project> LoadProjects(long page, long itemsPerPage)
        {
            return m_db.Page<Project>(page, itemsPerPage, "SELECT * FROM dbo.Project ORDER BY id desc");
        }

        public IEnumerable<Project> LoadProjects()
        {
            return m_db.Query<Project>("SELECT * FROM dbo.Project ORDER BY id desc");
        }

        public IEnumerable<string> LoadProjectNames()
        {
            return m_db.Query<string>("SELECT name FROM dbo.Project ORDER BY id desc");
        }

        public Page<Task> LoadTasks(string projectName, long page, long itemsPerPage)
        {
            return m_db.Page<Task>(page, itemsPerPage,
                " SELECT * FROM dbo.Task WHERE dbo.Task.name IN "
                + "(SELECT taskName FROM dbo.TaskInProject WHERE dbo.TaskInProject.projectName = @0)"
                + "ORDER BY dbo.Task.id", projectName);
        }

        public Page<Task> LoadTasks(long page, long itemsPerPage)
        {
            return m_db.Page<Task>(page, itemsPerPage,
                " SELECT * FROM dbo.Task ORDER BY dbo.Task.id");
        }

        public IEnumerable<Task> LoadTasks()
        {
            return m_db.Query<Task>(" SELECT * FROM dbo.Task ORDER BY dbo.Task.id");
        }


        public IEnumerable<string> LoadTemplateNames()
        {
            return m_db.Query<string>("SELECT DISTINCT(template_name) FROM TaskTemplate ORDER BY template_name");
        }

        public IEnumerable<TaskTemplate> LoadTaskTemplate(string templateName)
        {
            return m_db.Query<TaskTemplate>("SELECT * FROM TaskTemplate WHERE template_name = @0 ORDER BY id", templateName);
        }

        private Database m_db;
    }
}