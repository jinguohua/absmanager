using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public enum ExampleFile
    {
        工作流模板,
        参与机构模板,
    }
    public class DownloadController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index(string guid)
        {
            var userName = string.IsNullOrWhiteSpace(CurrentUserName) ? "anonymous" : CurrentUserName;

            var resource = ResourcePool.Release(userName, new Guid(guid));
            if (resource != null)
            {
                if (resource.Type == ResourceType.MemoryStream)
                {
                    var ms = resource.Instance as MemoryStream;
                    var mimeType = FileUtils.GetMIME(FileUtils.GetExtension(resource.Name));
                    return File(ms, mimeType, resource.Name);
                }
                if (resource.Type == ResourceType.FilePath)
                {
                    var filePath = resource.Instance as string;
                    var mimeType = FileUtils.GetMIME(FileUtils.GetExtension(resource.Name));
                    return File(filePath, mimeType, resource.Name);
                }
            }

            return RedirectToAction("Index", "Error");
        }

        [HttpPost]
        public ActionResult ExampleFile(string exampleFileName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(exampleFileName), "参考文件名称不能为空");

                ExampleFile exampleFile;
                if (!Enum.TryParse<ExampleFile>(exampleFileName, out exampleFile))
                {
                    CommUtils.Assert(false, "参考文件名称[{0}]错误", exampleFileName);
                }
                CommUtils.Assert(m_dicExampleFilePath.Keys.ToList().Contains(exampleFile.ToString()),"找不到文件或系统没有配置文件，请联系管理员");

                var filePath = Path.Combine(CurrentAppliCationPath, m_dicExampleFilePath[exampleFile.ToString()]);

                if (!System.IO.File.Exists(filePath))
                {
                    CommUtils.Assert(false, "找不到文件或系统没有配置文件，请联系管理员");
                }
                var resource = ResourcePool.RegisterFilePath(CurrentUserName, Path.GetFileName(filePath), filePath);
                return ActionUtils.Success(resource.Guid);
            });
        }

        private Dictionary<string, string> m_dicExampleFilePath = new Dictionary<string, string>()
        {
            {"工作流模板",@"ExampleFile\工作流模板.xlsx"},
            {"参与机构模板",@"ExampleFile\参与机构模板.xlsx"},
        };

        public static readonly string CurrentAppliCationPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

    }
}