using ChineseAbs.ABSManagement.DocumentFactory;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.IO;
using System.Web;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension.Demo
{
    public class TaskExDemoJianYuanReport : TaskExBase
    {
        public TaskExDemoJianYuanReport(string userName, string shortCode)
            : base(userName, shortCode)
        {
            this.OnFinishing += TaskExDemoJianYuanReport_OnFinishing;
        }

        HandleResult TaskExDemoJianYuanReport_OnFinishing()
        {
            if (Task.TaskExtension.TaskExtensionStatus != TaskExtensionStatus.Finished)
            {
                return new HandleResult(EventResult.Cancel, "请先确认核对。");
            }

            return new HandleResult();
        }

        public override object GetEntity()
        {
            return new object();
        }

        public Tuple<MemoryStream, DocFileInfo> UploadDemoJianYuanReport(HttpPostedFileBase file, string shortCode)
        {
            var ms = new MemoryStream();
            var docFileInfo = new DocFileInfo { DisplayName = "信托受托机构报告.docx" };
            
            var docFactiory = new DocumentFactory(m_userName);
            docFactiory.Generate(DocPatternType.DemoJianYuanReport, ms,
                shortCode, file.InputStream, file.FileName);

            ms.Seek(0, SeekOrigin.Begin);
            return Tuple.Create(ms, docFileInfo);
        }
    }
}