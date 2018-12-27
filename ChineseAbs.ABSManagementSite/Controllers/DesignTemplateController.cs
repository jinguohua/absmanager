using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Filters;
using ChineseAbs.ABSManagementSite.Models;
using System;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [DesignAccessAttribute]
    public class DesignTemplateController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var viewModel = new DesignTemplateViewModel();
            var templates = m_dbAdapter.Template.GetTemplates();
            viewModel.Templates = templates.ConvertAll(x => new TemplateViewModel
            {
                TemplateGuid = x.TemplateGuid,
                TemplateName = x.TemplateName,
                CreateUser = x.CreateUser,
                CreateTime = x.CreateTime
            });

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateTemplate(string newTemplateName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrEmpty(newTemplateName), "创建模板名称不能为空");
                CommUtils.Assert(newTemplateName.Length <= 20, "模板名称不能超过20个字符数");

                var template = new Template()
                {
                    TemplateName = newTemplateName,
                    TemplateGuid = Guid.NewGuid().ToString(),
                    CreateUser = CurrentUserName,
                    CreateTime = DateTime.Now
                };

                template = m_dbAdapter.Template.NewTemplate(template);
                LogEditProduct(EditProductType.CreateProduct, null, "创建Template[" + template.TemplateId + "][" + template.TemplateName + "]", "");

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DeleteTemplate(string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var result = m_dbAdapter.Template.DeleteTemplate(template);

                if (result != 1)
                {
                    return ActionUtils.Failure("删除模板 [" + template.TemplateName + "] 失败");
                }

                LogEditProduct(EditProductType.CreateProduct, null, "删除Template[" + template.TemplateId + "][" + template.TemplateName + "]", "");
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult RenameTemplate(string templateGuid, string newTemplateName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(newTemplateName.Length <= 20, "模板名称不能超过20个字符数");

                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                CommUtils.Assert(template.TemplateName != newTemplateName, "新模板名称 [" + newTemplateName + "] 和原名称相同");

                var templateNames = m_dbAdapter.Template.GetTemplateNames();
                CommUtils.Assert(!templateNames.Contains(newTemplateName), "模板名称 [" + newTemplateName + "] 已经存在");

                var oldTemplateName = template.TemplateName;
                template.TemplateName = newTemplateName;
                m_dbAdapter.Template.UpdateTemplate(template);

                LogEditProduct(EditProductType.CreateProduct, null, "重命名Template[" + template.TemplateId + "][" + oldTemplateName + "] → [" + template.TemplateName + "]", "");
                return ActionUtils.Success("");
            });
        }

        private void LogEditProduct(EditProductType type, int? projectId, string description, string comment)
        {
            m_dbAdapter.Project.NewEditProductLog(type, projectId, description, comment);
        }
    }
}