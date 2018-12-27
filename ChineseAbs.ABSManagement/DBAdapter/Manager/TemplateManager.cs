using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public class TemplateManager : BaseManager
    {
        public TemplateManager()
        {
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public Template GetTemplate(string templateGuid)
        {
            var templates = m_db.Query<ABSMgrConn.TableTemplate>("SELECT * FROM Template WHERE template_guid=@0 ORDER BY template_id DESC", templateGuid);
            return new Template(templates.Single());
        }

        public List<Template> GetTemplates()
        {
            var templates = m_db.Query<ABSMgrConn.TableTemplate>("SELECT * FROM Template ORDER BY template_id DESC");
            return templates.ToList().ConvertAll(x => new Template(x)).ToList();
        }

        public List<string> GetTemplateNames()
        {
            var templates = m_db.Query<ABSMgrConn.TableTemplate>("SELECT * FROM Template ORDER BY template_id DESC");
            return templates.ToList().ConvertAll(x => x.template_name);
        }

        public int UpdateTemplate(Template template)
        {
            var templateTable = template.GetTableObject();
            return m_db.Update("Template", "template_id", templateTable, template.TemplateId);
        }

        public int DeleteTemplate(Template template)
        {
            return m_db.Delete("Template", "template_id", template.GetTableObject());
        }

        public List<TemplateTask> GetTemplateTasks(string templateGuid)
        {
            var template = GetTemplate(templateGuid);
            return GetTemplateTasks(template.TemplateId);
        }

        public List<TemplateTask> GetTemplateTasks(int templateId)
        {
            var templateTasks = m_db.Query<ABSMgrConn.TableTemplateTask>(
                "SELECT * FROM TemplateTask WHERE template_id = @0 ORDER BY template_id", templateId);
            return templateTasks.ToList().ConvertAll(x => new TemplateTask(x)).ToList();
        }

        public TemplateTime NewTemplateTime(TemplateTime templateTime)
        {
            var tableTemplateTime = templateTime.GetTableObject();
            var templateTimeId = m_db.Insert("TemplateTime", "Template_time_id", true, tableTemplateTime);
            templateTime.TemplateTimeId = (int)templateTimeId;
            return templateTime;
        }

        public int GetTemplateTimeId(int templateId, string templateTimeName)
        {
            var ids = m_db.Query<int>(
                "SELECT template_time_id FROM dbo.TemplateTime WHERE template_id=@0 AND template_time_name=@1 ORDER BY template_id", templateId,templateTimeName);

             if (ids.Count() < 1)
            {
                throw new ApplicationException("找不到对应的Template[" + templateId + "]");
            }

            return ids.Single();
        }

        public Template GetTemplateByName(string templateName)
        {
            var templateList = m_db.Query<ABSMgrConn.TableTemplate>(
                "SELECT * FROM dbo.Template WHERE template_name = @0", templateName);

            return new Template(templateList.Single());
        }

        public bool TemplateExists(string templateName)
        {
            string sql = @"select count(*) from dbo.Template where template_name=@0";
            int count = m_db.ExecuteScalar<int>(sql, templateName);
            return count > 0;
        }

        public Template NewTemplate(Template template)
        {
            if (TemplateExists(template.TemplateName))
            {
                throw new ApplicationException("模板名称已存在[" + template.TemplateName + "]");
            }
            
            var templateId = (int)m_db.Insert("Template", "template_id", true, template.GetTableObject());
            template.TemplateId = templateId;
            return template;
        }

        public List<TemplateTime> GetTemplateTimeList(int templateTimeId)
        {
            var templateTimeList = m_db.Query<ABSMgrConn.TableTemplateTime>(
                "SELECT * FROM dbo.TemplateTime WHERE template_time_id = @0 ORDER BY template_time_id",
                templateTimeId);

            return templateTimeList.ToList().ConvertAll(x => new TemplateTime(x)).ToList();
        }

        public List<TemplateTime> GetTemplateTimeNames(int templateId)
        {
            var templateTimeNames = m_db.Query<ABSMgrConn.TableTemplateTime>(
                "SELECT * FROM dbo.TemplateTime WHERE template_id=@0 ORDER BY template_time_name", templateId);
            return templateTimeNames.ToList().ConvertAll(x => new TemplateTime(x)).ToList();
        }

        public List<TemplateTime> GetTemplateTimeLists(int templateId)
        {
            var templateTimeLists = m_db.Query<ABSMgrConn.TableTemplateTime>(
                "SELECT * FROM dbo.TemplateTime WHERE template_id = @0 ORDER BY template_id",
                templateId);
            return templateTimeLists.ToList().ConvertAll(x => new TemplateTime(x)).ToList();
        }

        public List<TemplateTime> GetTemplateTimeLists(string templateGuid)
        {
            var template = GetTemplate(templateGuid);
            return GetTemplateTimeLists(template.TemplateId);
        }

        public int UpdateTemplateTimeList(TemplateTime templateTime)
        {
            var templateTimeTable = templateTime.GetTableObject();
            return m_db.Update("TemplateTime","template_time_id",templateTimeTable,templateTime.TemplateTimeId);
        }

        public int DeleteTemplateTask(TemplateTask templateTask)
        {
            return m_db.Delete("TemplateTask", "template_task_id", templateTask.GetTableObject());
        }

        public TemplateTask GetTemplateTask(int templateTaskId)
        {
            var templateTask = m_db.Query<ABSMgrConn.TableTemplateTask>(
                "SELECT * FROM dbo.TemplateTask WHERE template_task_id = @0 ORDER BY template_id", templateTaskId);
            return new TemplateTask(templateTask.Single());
        }

        public TemplateTime GetTemplateTime(int templateTimeId)
        {
            var templateTime = m_db.Query<ABSMgrConn.TableTemplateTime>(
                "SELECT * FROM dbo.TemplateTime WHERE template_time_id = @0 ORDER BY template_id", templateTimeId);
            return new TemplateTime(templateTime.Single());
        }

        public int DeleteTemplateTime(int templateTimeId)
        {
            var templateTime = GetTemplateTime(templateTimeId);
            return DeleteTemplateTime(templateTime);
        }

        public int DeleteTemplateTime(TemplateTime templateTime)
        {
            return m_db.Delete("TemplateTime", "template_time_id", templateTime.GetTableObject());
        }
    }
}
