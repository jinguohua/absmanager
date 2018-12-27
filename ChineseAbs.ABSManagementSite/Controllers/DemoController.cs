using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Utils.Demo;
using ChineseAbs.ABSManagementSite.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [DemoDeployed]
    public class DemoController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Project()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfigProject()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ImportData()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ExportReport()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult GenerateByUploadedExcel(DateTime paymentDate, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var path = DemoJianYuanUtils.GetExcelReportPath();
                CommUtils.Assert(System.IO.File.Exists(path), "请先上传服务商报告文件（path={0}）", path);

                Resource resource = null;
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    var ms = new MemoryStream();
                    var docFileInfo = new DocFileInfo { DisplayName = "信托受托机构报告.docx" };

                    var utils = new DemoJianYuanUtils();
                    utils.Generate(ms, fs, "服务商报告.xls", paymentDate, asOfDate);

                    ms.Seek(0, SeekOrigin.Begin);
                    var result = Tuple.Create(ms, docFileInfo);

                    //生成报告
                    var userName = string.IsNullOrWhiteSpace(CurrentUserName) ? "anonymous" : CurrentUserName;
                    resource = ResourcePool.RegisterMemoryStream(userName, result.Item2.DisplayName, result.Item1);
                }

                return ActionUtils.Success(resource.Guid.ToString());
            });
        }
        

        [AllowAnonymous]
        public ActionResult Generate(HttpPostedFileBase file, DateTime paymentDate, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var fileName = file.FileName;
                if (!fileName.EndsWith("xls", StringComparison.CurrentCultureIgnoreCase)
                    && !fileName.EndsWith("xlsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    CommUtils.Assert(false, "根据[{0}]生成文档失败，请选择Excel文件上传", fileName);
                }

                //上传服务商报告
                var result = UploadDemoJianYuanReport(file, paymentDate, asOfDate);

                //生成报告
                var userName = string.IsNullOrWhiteSpace(CurrentUserName) ? "anonymous" : CurrentUserName;
                var resource = ResourcePool.RegisterMemoryStream(userName, result.Item2.DisplayName, result.Item1);

                return ActionUtils.Success(resource.Guid.ToString());
            });
        }

        private Tuple<MemoryStream, DocFileInfo> UploadDemoJianYuanReport(HttpPostedFileBase file, DateTime paymentDate, string asOfDate)
        {
            var ms = new MemoryStream();
            var docFileInfo = new DocFileInfo { DisplayName = "信托受托机构报告.docx" };

            var utils = new DemoJianYuanUtils();
            utils.Generate(ms, file.InputStream, file.FileName, paymentDate, asOfDate);

            ms.Seek(0, SeekOrigin.Begin);
            return Tuple.Create(ms, docFileInfo);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadTemplateFile(HttpPostedFileBase file)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertNotNull(file, "文件不能为空,请选择文件");
                CommUtils.Assert(file.ContentLength > 0, "文件内容不能为空");

                CommUtils.Assert(file.FileName.EndsWith(".docx", StringComparison.CurrentCultureIgnoreCase),
                    "文件[{0}]格式错误,请选择.docx格式的文件", file.FileName);

                CommUtils.Assert(!CommUtils.IsWPS(file.InputStream), "不支持wps编辑过的.docx格式文件，仅支持office编辑的.docx文件");

                var path = DemoJianYuanUtils.GetTemplateFilePath();

                file.SaveAs(path);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadExcelReport(HttpPostedFileBase file)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertNotNull(file, "文件不能为空,请选择文件");
                CommUtils.Assert(file.ContentLength > 0, "文件内容不能为空");

                CommUtils.Assert(file.FileName.EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase),
                    "文件[{0}]格式错误,请选择.xls格式的文件", file.FileName);

                var path = DemoJianYuanUtils.GetExcelReportPath();

                file.SaveAs(path);

                return ActionUtils.Success(1);
            });
        }

        [AllowAnonymous]
        public ActionResult DownloadExcelReport()
        {
            return ActionUtils.Json(() =>
            {
                var path = DemoJianYuanUtils.GetExcelReportPath();
                CommUtils.Assert(System.IO.File.Exists(path), "请先上传服务商报告文件（path={0}）", path);

                var guid = string.Empty;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);
                    var bytes = br.ReadBytes((int)br.BaseStream.Length);
                    var ms = new MemoryStream(bytes);

                    var fileName = Path.GetFileName(path);

                    var userName = string.IsNullOrWhiteSpace(CurrentUserName) ? "anonymous" : CurrentUserName;
                    var resource = ResourcePool.RegisterMemoryStream(userName, fileName, ms);
                    guid = resource.Guid.ToString();
                }

                return ActionUtils.Success(guid);
            });
        }


        [AllowAnonymous]
        public ActionResult DownloadTemplateFile()
        {
            return ActionUtils.Json(() =>
            {
                var path = DemoJianYuanUtils.GetTemplateFilePath();
                CommUtils.Assert(System.IO.File.Exists(path), "请先上传模板文件（path={0}）", path);

                var guid = string.Empty;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);
                    var bytes = br.ReadBytes((int)br.BaseStream.Length);
                    var ms = new MemoryStream(bytes);

                    var fileName = Path.GetFileName(path);

                    var userName = string.IsNullOrWhiteSpace(CurrentUserName) ? "anonymous" : CurrentUserName;
                    var resource = ResourcePool.RegisterMemoryStream(userName, fileName, ms);
                    guid = resource.Guid.ToString();
                }

                return ActionUtils.Success(guid);
            });
        }

        [AllowAnonymous]
        public ActionResult DownloadModel(string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(asOfDate, "无效的参数：asOfDate");

                var folder = DemoJianYuanUtils.GetModelFolder();

                var ymlFilePath = Path.Combine(folder, "script.yml");

                CommUtils.Assert(System.IO.File.Exists(ymlFilePath), "请先上传模型（找不到模型文件：{0}）", ymlFilePath);

                var asOfDateFolder = Path.Combine(folder, asOfDate);
                CommUtils.Assert(System.IO.Directory.Exists(asOfDateFolder), "请先上传模型（找不到路径：{0}）", asOfDateFolder);

                var fileNames = new List<string> { 
                    "script.yml",
                    asOfDate + @"\AmortizationSchedule.csv",
                    asOfDate + @"\Reinvestment.csv",
                    asOfDate + @"\AnalyzerResults.csv",
                    asOfDate + @"\AssetCashflowTable.csv",
                    asOfDate + @"\CashflowTable.csv",
                    asOfDate + @"\collateral.csv",
                    asOfDate + @"\CurrentVariables.csv",
                    asOfDate + @"\FutureVariables.csv",
                    asOfDate + @"\PastVariables.csv"
                };

                var ms = new MemoryStream();
                ZipUtils.CompressFiles(folder, fileNames, ms);

                var fileFullName = "DataModel(" + asOfDate + ").zip";

                var userName = string.IsNullOrWhiteSpace(CurrentUserName) ? "anonymous" : CurrentUserName;
                var resource = ResourcePool.RegisterMemoryStream(userName, fileFullName, ms);
                var guid = resource.Guid.ToString();

                return ActionUtils.Success(guid);
            });
        }

        protected class ModelFile
        {
            public ModelFile(string pathName, bool mustExist)
            {
                PathName = pathName;
                MustExist = mustExist;
            }

            public string PathName;
            public string ZipPathName;
            public bool MustExist;
        }

        protected class ModelFiles
        {
            public ModelFiles(string AsOfDate)
            {
                YmlFile = new ModelFile(@"script.yml", false);

                CurrentVariables = new ModelFile(AsOfDate + @"/CurrentVariables.csv", true);
                FutureVariables = new ModelFile(AsOfDate + @"/FutureVariables.csv", true);
                PastVariables = new ModelFile(AsOfDate + @"/PastVariables.csv", true);

                AssetCashflowTable = new ModelFile(AsOfDate + @"/AssetCashflowTable.csv", false);
                CashflowTable = new ModelFile(AsOfDate + @"/CashflowTable.csv", false);
                Amortization = new ModelFile(AsOfDate + @"/AmortizationSchedule.csv", false);
                Reinvestment = new ModelFile(AsOfDate + @"/Reinvestment.csv", false);
                AnalyzerResults = new ModelFile(AsOfDate + @"/AnalyzerResults.csv", false);

                collateral = new ModelFile(AsOfDate + @"/collateral.csv", false);
            }

            public ModelFile YmlFile;
            public ModelFile AnalyzerResults;
            public ModelFile AssetCashflowTable;
            public ModelFile CashflowTable;
            public ModelFile Amortization;
            public ModelFile Reinvestment;
            public ModelFile collateral;
            public ModelFile CurrentVariables;
            public ModelFile FutureVariables;
            public ModelFile PastVariables;

            public Dictionary<string, string> GetFileDictionary()
            {
                var dict = new Dictionary<string, string>();
                Action<ModelFile> addDict = (modelFile) =>
                {
                    if (!string.IsNullOrEmpty(modelFile.PathName) && !string.IsNullOrEmpty(modelFile.ZipPathName))
                    {
                        dict[modelFile.ZipPathName] = modelFile.PathName;
                    }
                };

                addDict(YmlFile);

                addDict(Amortization);
                addDict(Reinvestment);

                addDict(AssetCashflowTable);
                addDict(CashflowTable);

                addDict(AnalyzerResults);
                addDict(CurrentVariables);
                addDict(FutureVariables);
                addDict(PastVariables);

                addDict(collateral);
                return dict;
            }
        }

        private ModelFiles CheckModelFile(string asOfDate, List<string> fileNames)
        {
            var knownCsvFiles = new List<string> { 
                asOfDate + @"/AnalyzerResults.csv",
                asOfDate + @"/CurrentVariables.csv",
                asOfDate + @"/FutureVariables.csv",
                asOfDate + @"/PastVariables.csv",

                asOfDate + @"/AssetCashflowTable.csv",
                asOfDate + @"/CashflowTable.csv",

                asOfDate + @"/AmortizationSchedule.csv",
                asOfDate + @"/Reinvestment.csv",
            };

            //文件名称/路径完全匹配
            Func<string, string, bool> cmpFileName = (left, right) => left.Equals(right, StringComparison.CurrentCultureIgnoreCase);

            //检查文件扩展名
            Func<string, string, bool> checkExtension = (fileName, ExtensionName) =>
            {
                return fileName.Length - ExtensionName.Length - 1 ==
                    fileName.LastIndexOf("." + ExtensionName, StringComparison.CurrentCultureIgnoreCase);
            };

            //检查collateral.csv文件
            Func<string, string, bool> checkCollateral = (fileName, ExtensionName) =>
            {
                return checkExtension(fileName, ExtensionName) && !knownCsvFiles.Contains(fileName);
            };

            //查找文件
            Func<ModelFile, Func<string, string, bool>, string, string> findFile = (modelFile, cmpFunc, param) =>
            {
                var fileCount = fileNames.Count(x => cmpFunc(x, param));
                CommUtils.Assert(fileCount < 2, "Search file [" + modelFile.PathName + "] failed!");
                if (modelFile.MustExist)
                {
                    CommUtils.Assert(fileCount > 0, "Can't find file [" + modelFile.PathName + "] !");
                }

                return fileNames.SingleOrDefault(x => cmpFunc(x, param));
            };


            var modelFiles = new ModelFiles(asOfDate);
            modelFiles.YmlFile.ZipPathName = findFile(modelFiles.YmlFile, checkExtension, "yml");

            modelFiles.Amortization.ZipPathName = findFile(modelFiles.Amortization, cmpFileName, modelFiles.Amortization.PathName);
            modelFiles.Reinvestment.ZipPathName = findFile(modelFiles.Reinvestment, cmpFileName, modelFiles.Reinvestment.PathName);

            modelFiles.CashflowTable.ZipPathName = findFile(modelFiles.CashflowTable, cmpFileName, modelFiles.CashflowTable.PathName);
            modelFiles.CurrentVariables.ZipPathName = findFile(modelFiles.CurrentVariables, cmpFileName, modelFiles.CurrentVariables.PathName);
            modelFiles.FutureVariables.ZipPathName = findFile(modelFiles.FutureVariables, cmpFileName, modelFiles.FutureVariables.PathName);
            modelFiles.PastVariables.ZipPathName = findFile(modelFiles.PastVariables, cmpFileName, modelFiles.PastVariables.PathName);

            modelFiles.AnalyzerResults.ZipPathName = findFile(modelFiles.AnalyzerResults, cmpFileName, modelFiles.AnalyzerResults.PathName);
            modelFiles.AssetCashflowTable.ZipPathName = findFile(modelFiles.AssetCashflowTable, cmpFileName, modelFiles.AssetCashflowTable.PathName);

            //查找collateral.csv
            modelFiles.collateral.ZipPathName = findFile(modelFiles.collateral, cmpFileName, modelFiles.collateral.PathName);
            if (string.IsNullOrEmpty(modelFiles.collateral.ZipPathName))
            {
                //查找其它csv来替代collateral.csv
                modelFiles.collateral.ZipPathName = findFile(modelFiles.collateral, checkCollateral, "csv");
            }

            return modelFiles;
        }

        [AllowAnonymous]
        public ActionResult UploadModel(HttpPostedFileBase file, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(DateUtils.IsDigitDate(asOfDate), "asOfDate格式错误（应为八位数字）:{0}", asOfDate);

                CommUtils.AssertNotNull(file, "请选择文件");
                CommUtils.Assert(file.FileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase),
                    "请选择zip文件({0})", file.FileName);

                var fileNames = ZipUtils.GetZipFileNames(file.InputStream);
                file.InputStream.Seek(0, SeekOrigin.Begin);

                //检查上传模型文件结构
                var modelFiles = CheckModelFile(asOfDate, fileNames);

                var modelFolder = DemoJianYuanUtils.GetModelFolder();

                var fileDicts = modelFiles.GetFileDictionary();
                ZipUtils.ExtractFiles(file.InputStream, fileDicts, modelFolder);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetKeyValue()
        {
            return ActionUtils.Json(() =>
            {
                var path = DemoJianYuanUtils.GetKeyValuePath();

                string content = string.Empty;
                if (System.IO.File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                var dict = ParseKeyValue(content);
                return ActionUtils.Success(dict);
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetKeyValue(string keyValue)
        {
            return ActionUtils.Json(() =>
            {
                var dict = ParseKeyValue(keyValue);
                
                var path = DemoJianYuanUtils.GetKeyValuePath();

                //CommUtils.Assert(System.IO.File.Exists(path), "请先上传服务商报告文件（path={0}）", path);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(keyValue);
                }

                return ActionUtils.Success(dict);
            });
        }

        private Dictionary<string, string> ParseKeyValue(string triggerOption)
        {
            var dict = new Dictionary<string, string>();
            var triggerOptionItems = CommUtils.Split(triggerOption);
            foreach (var trigger in triggerOptionItems)
            {
                var keyValue = CommUtils.Split(trigger, new[] { "^" });
                CommUtils.AssertEquals(keyValue.Length, 2, "解析keyValue[{0}]失败", triggerOption);
                dict[keyValue[0]] = keyValue[1];
            }
            return dict;
        }
    }
}

