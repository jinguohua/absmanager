using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Utils.TreeUtils;
using ChineseAbs.ABSManagementSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    using TreeNode = Node<PropInfo>;

    //[DesignAccessAttribute]
    public class DesignDocumentController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var viewModel = new DesignDocumentViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetJsonData()
        {
            return ActionUtils.Json(() =>
            {
                DesignDocumentViewModel jsonData = new DesignDocumentViewModel();

                var obj = InitializeIncDisReport();
                jsonData.Tree.Root = ReflectNode(obj, "Root");

                return ActionUtils.Success(jsonData);
            });
        }

        private static readonly List<DocPatternType> m_validDocPatternTypes = new List<DocPatternType>
        {
            DocPatternType.IncomeDistributionReport,
            DocPatternType.SpecialPlanTransferInstruction,
            DocPatternType.CashInterestRateConfirmForm,
            DocPatternType.InterestPaymentPlanApplication,
        };

        private List<DocPatternType> GetDocPatternTypes(ABSManagement.Models.Project project)
        {
            var docPatternTypes = m_validDocPatternTypes.ConvertAll(x => x);
            if (project != null && project.Name.Contains("建元"))
            {
                docPatternTypes.Add(DocPatternType.DemoJianYuanReport);
            }
            return docPatternTypes;
        }

        private bool IsValidDocPatternName(ABSManagement.Models.Project project, string docPatternName)
        {
            return GetDocPatternTypes(project).ConvertAll(x => DocumentPattern.GetFileName(x))
                .Any(x => x.Equals(docPatternName, StringComparison.CurrentCultureIgnoreCase));
        }

        [HttpPost]
        public ActionResult GetProjectsNameAndGuid()
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.ModifyTask), "没有修改产品的权限");

                var authoritiedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
                var projects = m_dbAdapter.Project.GetProjects(authoritiedProjectIds);

                var result = projects.ConvertAll(x => new
                {
                    name = x.Name,
                    guid = x.ProjectGuid

                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetTemplate(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(projectGuid, "projectGuid不能为空");

                var authoritiedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authoritiedProjectIds.Any(x => x == project.ProjectId), "用户[{0}]没有上传文件模板到产品[{1}]的权限", CurrentUserName, project.Name);

                var docPatternTypes = GetDocPatternTypes(project);
                var result = docPatternTypes.ConvertAll(x =>
                {
                    var path = DocumentPattern.GetPath(project, x);
                    var fileName = DocumentPattern.GetFileName(x);
                    var exist = System.IO.File.Exists(path);
                    return new
                    {
                        templateFileName = fileName.Remove(fileName.LastIndexOf(".")),
                        docPatternType = x.ToString(),
                        status = exist ? "Exist" : "NotExist",
                        createTime = exist ? System.IO.File.GetCreationTime(path).ToString("yyyy-MM-dd HH:mm") : "",
                    };
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult UploadTemplate(string projectGuid, string templateFileName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(Request.Files.Count > 0, "请选择文件");

                var file = Request.Files[0];
                CommUtils.Assert(file.ContentLength > 0, "文件内容不能为空");

                CommUtils.AssertHasContent(projectGuid, "projectGuid不能为空");
                CommUtils.AssertHasContent(templateFileName, "templateFileName不能为空");

                //   var authoritiedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
                var authoritiedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authoritiedProjectIds.Any(x => x == project.ProjectId), "用户[{0}]没有修改产品[{1}]的权限", CurrentUserName, project.Name);

                templateFileName += ".docx";
                CommUtils.Assert(file.FileName.EndsWith(".docx", StringComparison.CurrentCultureIgnoreCase),
                    "文件[{0}]格式错误,请选择.docx格式的文件", file.FileName);

                CommUtils.Assert(!CommUtils.IsWPS(file.InputStream), "不支持wps编辑过的.docx格式文件，仅支持office编辑的.docx文件");
                CommUtils.Assert(IsValidDocPatternName(project, templateFileName), "上传参数有误：templateFileName={0}", templateFileName);

                string sourcePath = Path.Combine(DocumentPattern.GetFolder(project), templateFileName);
                string backupPath = Path.Combine(DocumentPattern.GetFolder(project), "backup");
                if (!Directory.Exists(backupPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(backupPath);
                    directoryInfo.Create();
                }

                string backupFilePath = Path.Combine(backupPath, templateFileName);

                if (System.IO.File.Exists(sourcePath))
                {
                    backupFilePath = FileUtils.InsertTimeStamp(backupFilePath);
                    System.IO.File.Copy(sourcePath, backupFilePath, true);
                }

                file.SaveAs(sourcePath);

                return ActionUtils.Success(1);
            });
        }


        public ActionResult DownloadTemplate(string projectGuid, string templateFileName)
        {
            CommUtils.AssertHasContent(projectGuid, "projectGuid不能为空");
            CommUtils.AssertHasContent(templateFileName, "templateFileName不能为空");

            var authoritiedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            CommUtils.Assert(authoritiedProjectIds.Any(x => x == project.ProjectId), "用户[{0}]没有修改产品[{1}]的权限", CurrentUserName, project.Name);

            templateFileName += ".docx";
            string sourcePath = Path.Combine(DocumentPattern.GetFolder(project), templateFileName);

            return File(sourcePath, "application/octet-stream", templateFileName);
        }

        public IncomeDistributionReport InitializeIncDisReport()
        {
            var incDisReport = new IncomeDistributionReport();

            incDisReport.Sequence = 1;
            incDisReport.DenominationDetail = null;
            incDisReport.RepayDetail = "分期偿还情况";
            incDisReport.RepayDetailWithHyphen = "分期偿还情况（使用带-的券名）";
            incDisReport.RepayDetailWithHyphenByJinTai = "分期偿还情况（使用带-的券名)(金泰专用）";
            incDisReport.RepayPrincipalDetail = "分期偿还本金情况（对应白鹭2016-1中的【注：】中内容）";
            incDisReport.EquityRegisterDetail = "权益登记日详情（对应白鹭2016-1）";
            incDisReport.EquityRegisterDetailByJinTai = "权益登记日详情（金泰专用）";
            incDisReport.EquityRegisterDetailByZhongGang = "权益登记日详情（中港专用）";
            incDisReport.DurationDayCount = 2;
            incDisReport.AccrualDateSum = 3;
            incDisReport.PreviousT = new DateTime();
            incDisReport.T = new DateTime();
            incDisReport.T_1 = new DateTime();
            incDisReport.Date = new DateTime();
            incDisReport.BeginAccrualDate = new DateTime();
            incDisReport.EndAccrualDate = new DateTime();
            incDisReport.TaskEndTime = new DateTime();

            Func<ChineseAbs.ABSManagement.Pattern.PaymentDetail> initPaymentDetail = () =>
                new ChineseAbs.ABSManagement.Pattern.PaymentDetail
                {
                    Sequence = 0,
                    NameCN = "NameCN",
                    NameCNHyphen = "NameCNHyphen",
                    NameEN = "NameEN",
                    NameENUnderline = "NameENUnderline",
                    NameENHyphen = "NameENHyphen",
                    Residual = 123.4m,
                    UnitMoney = 123.5m,
                    UnitPrincipal = 123.6m,
                    UnitInterest = 123.7m,
                    Notional = 1.8m,
                    UnitCount = 3,
                    Money = 2.0m,
                    Principal = 2.1m,
                    Interest = 2.2m,
                    Denomination = 2.3m,
                    IsEquity = true,
                    CouponString = "CouponString"
                };

            ChineseAbs.ABSManagement.Pattern.PaymentDetail payDetail = initPaymentDetail();
            incDisReport.PriorSecurityList = new List<ABSManagement.Pattern.PaymentDetail>();
            incDisReport.SubSecurityList = new List<ABSManagement.Pattern.PaymentDetail>
            {
                initPaymentDetail(),
                initPaymentDetail()
            };

            incDisReport.SecurityList = new List<ABSManagement.Pattern.PaymentDetail>
            {
                initPaymentDetail(),
                initPaymentDetail()
            };

            incDisReport.Sum = initPaymentDetail();
            incDisReport.SumPrior = initPaymentDetail();
            incDisReport.SumSub = initPaymentDetail();

            incDisReport.PrincipalTable = new List<CashItem>
            {
                new CashItem {Date = new DateTime(), Percent = new Dictionary<string,decimal> ()},
                new CashItem {Date = new DateTime(), Percent = new Dictionary<string,decimal> ()}
            };

            incDisReport.Security = new Dictionary<string, ChineseAbs.ABSManagement.Pattern.PaymentDetail>();
            incDisReport.Security.Add("T1key", incDisReport.SumPrior);
            incDisReport.Security.Add("T2key", incDisReport.SumPrior);

            incDisReport.SumPrincipalTable = new CashItem
            {
                Date = new DateTime(),
                Percent = new Dictionary<string, decimal>()
            };

            return incDisReport;
        }

        private TreeNode ReflectNode(object value, string name)
        {
            var propInfo = new PropInfo();
            propInfo.SetDataType(value == null ? "Nulltype" : value.GetType().Name);
            propInfo.Name = name;
            propInfo.Value = string.Empty;

            TreeNode childNode = new TreeNode();
            childNode.Info = propInfo;

            if (value == null)
            {
                childNode.Info.Value = " : null";
            }
            else if (childNode.Info.DataType != SimpleDataType.Undefined)
            {
                childNode.Info.Value = " : " + value.ToString();
            }
            else
            {
                childNode.Nodes = ReflectChildNodes(value);
            }

            return childNode;
        }

        private List<TreeNode> ReflectChildNodes(object obj)
        {
            var result = new List<TreeNode>();

            var type = obj.GetType();
            if (!type.IsGenericType)
            {
                foreach (var propInfo in type.GetProperties())
                {
                    result.Add(ReflectNode(propInfo.GetValue(obj, null), propInfo.Name));
                }
            }
            else if (type.GetGenericTypeDefinition() == typeof(List<>))
            {
                int count = Convert.ToInt32(type.GetProperty("Count").GetValue(obj, null));
                var propInfo = type.GetProperty("Item");
                for (int i = 0; i < count; ++i)
                {
                    object value = propInfo.GetValue(obj, new object[] { i });
                    result.Add(ReflectNode(value, i.ToString()));
                }
            }
            else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keys = (type.GetProperty("Keys").GetValue(obj, null) as IEnumerable<object>).ToList();
                var values = type.GetProperty("Values").GetValue(obj, null) as IEnumerable<object>;

                for (int i = 0; i < keys.Count; ++i)
                {
                    result.Add(ReflectNode(values.ElementAt(i), keys[i].ToString()));
                }
            }

            return result;
        }

    }
}
