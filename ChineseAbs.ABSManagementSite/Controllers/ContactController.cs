using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Utils.Excel;
using ChineseAbs.ABSManagement.Utils.Excel.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ContactController : BaseController
    {
        [HttpPost]
        public ActionResult GetContacts(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CheckHandleContactPermission(project);

                var contacts = m_dbAdapter.Contact.GetContactsByProjectId(project.ProjectId);
                var result = contacts.ConvertAll(x => new
                {
                    guid = x.Guid,
                    organizationName = x.OrganizationName,
                    dutyType = x.DutyType.ToString(),
                    name = x.Name,
                    cellPhone = x.CellPhone,
                    email = x.Email,
                    note = x.Note
                });

                return ActionUtils.Success(result);
            });
        }

        //发行协作平台：使用项目成员相关检查用户是否有权限
        //存续期管理平台：用户可以访问该产品权限即可
        private void CheckHandleContactPermission(Project project)
        {
            if (project.ProjectSeriesId.HasValue)
            {
                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);
            }
            else
            {
                var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                CommUtils.Assert(authorizedProjectIds.Contains(project.ProjectId), "用户没有权限");
            }
        }

        [HttpPost]
        public ActionResult GetContact(string contactGuid)
        {
            return ActionUtils.Json(() =>
            {
                var contact = m_dbAdapter.Contact.GetByGuid(contactGuid);
                var project = m_dbAdapter.Project.GetProjectById(contact.ProjectId);
                CheckHandleContactPermission(project);

                var result = new
                {
                    guid = contact.Guid,
                    organizationName = contact.OrganizationName,
                    dutyType = contact.DutyType.ToString(),
                    name = contact.Name,
                    cellPhone = contact.CellPhone,
                    email = contact.Email,
                    note = contact.Note
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreateContact(string projectGuid, string organizationName,
            string dutyType, string name, string cellPhone, string email, string note)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CheckHandleContactPermission(project);

                ValidateUtils.Name(organizationName, "相关方", 50);
                CommUtils.Assert(name.Length <= 30, "联系人不能超过30个字符数！");
                CommUtils.Assert(email.Length <= 38, "邮箱不能超过38个字符数！");
                CommUtils.Assert(cellPhone.Length <= 30, "电话不能超过30个字符数！");

                var contact = new Contact();
                contact.ProjectId = project.ProjectId;
                contact.OrganizationName = organizationName;
                contact.DutyType = CommUtils.ParseEnum<EDutyType>(dutyType);
                contact.Name = name;
                contact.Email = email;
                contact.CellPhone = cellPhone;
                contact.Note = note;
                m_dbAdapter.Contact.AddContact(contact);

                var logicModel = Platform.GetProject(project.ProjectGuid);
                logicModel.Activity.Add(project.ProjectId, ActivityObjectType.Contact, contact.Guid, "增加机构：" + contact.OrganizationName);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult RemoveContact(string contactGuid)
        {
            return ActionUtils.Json(() =>
            {
                var contact = m_dbAdapter.Contact.GetByGuid(contactGuid);
                var project = m_dbAdapter.Project.GetProjectById(contact.ProjectId);
                CheckHandleContactPermission(project);

                m_dbAdapter.Contact.RemoveContact(contact.ContactId, contact.ProjectId);

                var logicModel = Platform.GetProject(project.ProjectGuid);
                logicModel.Activity.Add(project.ProjectId, ActivityObjectType.Contact, contactGuid, "删除机构：" + contact.OrganizationName);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult EditContact(string contactGuid, string organizationName,
            string dutyType, string name, string cellPhone, string email, string note)
        {
            return ActionUtils.Json(() =>
            {
                ValidateUtils.Name(organizationName, "相关方", 50);
                CommUtils.Assert(name.Length <= 30, "联系人不能超过30个字符数！");
                CommUtils.Assert(email.Length <= 38, "邮箱不能超过38个字符数！");
                var contact = m_dbAdapter.Contact.GetByGuid(contactGuid);
                var project = m_dbAdapter.Project.GetProjectById(contact.ProjectId);

                CheckHandleContactPermission(project);

                contact.OrganizationName = organizationName;
                contact.DutyType = CommUtils.ParseEnum<EDutyType>(dutyType);
                contact.Name = name;
                contact.Email = email;
                contact.CellPhone = cellPhone;
                contact.Note = note;
                m_dbAdapter.Contact.UpdateContact(contact);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult ExportTable(string tableBodyJson)
        {
            return ActionUtils.Json(() =>
            {
                var table = tableBodyJson.ToDataTable();

                var sheet = new Sheet("ContactTable", table);
                var workbook = new Workbook("ContactTable", sheet);
                var resource = workbook.ToExcel(CurrentUserName);

                return ActionUtils.Success(resource.Guid);
            });
        }

        [HttpPost]
        public ActionResult ImportTable(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CheckHandleContactPermission(project);

                var file = Request.Files[0];
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, bytes.Length);
                file.InputStream.Seek(0, SeekOrigin.Begin);

                Stream newStream = new MemoryStream(bytes);
                var tableHeaderRowsCount = ExcelUtils.GetTableHeaderRowsCount(newStream, 0, 0, tableHeader);
                var table = ExcelUtils.ParseExcel(file.InputStream, 0, tableHeaderRowsCount, 0, 6);

                var validation = new ExcelValidation();
                validation.Add(CellRange.Column(0), new CellTextValidation(1, 50));

                Func<string, string> checkEDutyType = (cellText) =>
                {
                    try
                    {
                        CommUtils.ParseEnum<EDutyType>(cellText);
                    }
                    catch
                    {
                        return "无法将[" + cellText + "]解析为[职责]";
                    }
                    return string.Empty;
                };
                validation.Add(CellRange.Column(1), new CellCustomValidation(checkEDutyType, false));
                validation.Add(CellRange.Column(2), new CellTextValidation(0, 30));
                validation.Add(CellRange.Column(3), new CellTextValidation(0, 38));
                validation.Add(CellRange.Column(3), new CellEmailValidation());
                validation.Add(CellRange.Column(4), new CellTelValidation());
                validation.Check(table, tableHeaderRowsCount);

                Func<List<object>, int, int, Contact> ParseRow = (objs, index, projectId) =>
                {
                    return this.ParseRow(objs, index, projectId);
                };
                List<Contact> contacts = ExcelUtils.ParseTable<Contact>(table, project.ProjectId, ParseRow).ToList();

                contacts.ForEach(x => m_dbAdapter.Contact.AddContact(x));
                var logicModel = Platform.GetProject(project.ProjectGuid);
                contacts.ForEach(x =>
                    logicModel.Activity.Add(project.ProjectId, ActivityObjectType.Contact, x.Guid, "增加机构：" + x.OrganizationName));

                return ActionUtils.Success(contacts.Count);
            });
        }

        private Contact ParseRow(List<object> objs, int index, int projectId)
        {
            var organizationName = objs[0].ToString();
            var dutyType = objs[1].ToString();
            var name = objs[2].ToString();
            var email = objs[3].ToString();
            var cellPhone = objs[4].ToString();
            var note = objs[5].ToString();

            var contact = new Contact();
            contact.ProjectId = projectId;
            contact.OrganizationName = organizationName;
            contact.DutyType = CommUtils.ParseEnum<EDutyType>(dutyType);
            contact.Name = name;
            contact.Email = email;
            contact.CellPhone = cellPhone;
            contact.Note = note;

            return contact;
        }

        private readonly List<List<string>> tableHeader = new List<List<string>>() {new List<string>() {"相关方", "职责", "联系人","邮箱","电话","备注"}};
    }
}