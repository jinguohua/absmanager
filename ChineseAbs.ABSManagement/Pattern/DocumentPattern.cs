using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.IO;

namespace ChineseAbs.ABSManagement.Pattern
{
    public enum DocPatternType
    {
        /// <summary>
        /// 模板工作
        /// </summary>
        TemplateTask,
        /// <summary>
        /// 模板时间
        /// </summary>
        TemplateTime,
        /// <summary>
        /// 工作列表
        /// </summary>
        TaskList,
        /// <summary>
        /// 收益分配报告
        /// </summary>
        IncomeDistributionReport,
        /// <summary>
        /// 专项计划划款指令
        /// </summary>
        SpecialPlanTransferInstruction,
        /// <summary>
        /// 兑付兑息确认表
        /// </summary>
        CashInterestRateConfirmForm,
        /// <summary>
        /// 付息方案申请
        /// </summary>
        InterestPaymentPlanApplication,
        /// <summary>
        /// 建元Demo用报告
        /// </summary>
        DemoJianYuanReport
    }

    public class DocumentPattern
    {
        public static string GetFolder(Project project)
        {
            //旧版的模板文件路径格式： PatternFileFolder \ ProjectName \ PatternFileName
            //存在创建同名功能访问当其它文档模板路径的风险

            //新版模板文件路径修改为： PatternFileFolder \ ProjectName_ProjectGuid \ PatternFileName
            var pathWithGuid = Path.Combine(WebConfigUtils.PatternFileFolder, project.Name + "_" + project.ProjectGuid);

            if (Directory.Exists(pathWithGuid))
            {
                return pathWithGuid;
            }
            else
            {
                //识别到旧版模板文件路径后，迁移到新版文档路径
                var path = Path.Combine(WebConfigUtils.PatternFileFolder, project.Name);
                if (Directory.Exists(path))
                {
                    Directory.Move(path, pathWithGuid);
                    return pathWithGuid;
                }
                else
                {
                    Directory.CreateDirectory(pathWithGuid);
                    return pathWithGuid;
                }
            }
        }

        public static string GetPath(DocPatternType patternType)
        {
            CommUtils.Assert(patternType == DocPatternType.TemplateTask
                || patternType == DocPatternType.TemplateTime
                || patternType == DocPatternType.TaskList,
                "PatternType isn't TemplateTask/TemplateTime/TaskList");

            return Path.Combine(WebConfigUtils.PatternFileFolder, GetFileName(patternType));
        }

        public static string GetPath(Project project, DocPatternType patternType)
        {
            CommUtils.Assert(patternType != DocPatternType.TemplateTask
                && patternType != DocPatternType.TemplateTime
                && patternType != DocPatternType.TaskList,
                "PatternType is TemplateTask/TemplateTime/TaskList");

            return Path.Combine(GetFolder(project), GetFileName(patternType));
        }

        public static string GetFileName(DocPatternType patternType)
        {
            switch (patternType)
            {
                case DocPatternType.TemplateTask:
                    return "模板工作.xlsx";
                case DocPatternType.TemplateTime:
                    return "模板时间列表.xlsx";
                case DocPatternType.TaskList:
                    return "工作列表.xlsx";
                case DocPatternType.IncomeDistributionReport:
                    return "收益分配报告模板.docx";
                case DocPatternType.SpecialPlanTransferInstruction:
                    return "专项计划划款指令模板.docx";
                case DocPatternType.CashInterestRateConfirmForm:
                    return "兑付兑息确认表模板.docx";
                case DocPatternType.InterestPaymentPlanApplication:
                    return "付息方案申请模板.docx";
                case DocPatternType.DemoJianYuanReport:
                    return "Demo建元模板.docx";
                default:
                    CommUtils.Assert(false, "未知的模板类型：{0}", patternType.ToString());
                    return string.Empty;
            }
        }
    }
}
