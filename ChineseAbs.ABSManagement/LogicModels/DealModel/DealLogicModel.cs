using ChineseAbs.ABSManagement.Framework;
using ChineseAbs.ABSManagement.LogicModels.OverrideSingleAsset;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.CalcService.Data.NancyData;
using System;
using System.Data;
using System.IO;

namespace ChineseAbs.ABSManagement.LogicModels.DealModel
{
    public class DealLogicModel : BaseLogicModel
    {
        public DealLogicModel(DatasetLogicModel dataset, string ymlPath, string datasetPath)
            : base(dataset.ProjectLogicModel)
        {
            m_datasetLogicModel = dataset;

            if (!ymlPath.EndsWith("script.yml", StringComparison.CurrentCultureIgnoreCase))
            {
                ymlPath = Path.Combine(ymlPath, "script.yml");
            }

            m_ymlPath = ymlPath;
            m_datasetPath = datasetPath;
            m_asOfDate = datasetPath.EndsWith("\\") || datasetPath.EndsWith("/")
                ? Path.GetFileName(Path.GetDirectoryName(datasetPath)) : Path.GetFileName(datasetPath);

            CommUtils.Assert(DateUtils.IsDigitDate(m_asOfDate), "解析AsOfDate失败：datasetPath={0}", datasetPath);
        }

        private DealModelFolerInfo CopyTemporaryModel()
        {
            var srcDsFolder = m_datasetPath;
            var temporaryFolder = CommUtils.CreateTemporaryFolder(Platform.UserName);
            var temporaryDsFolder = CommUtils.CreateFolder(temporaryFolder, m_asOfDate);

            ParallelUtils.StartUntilFinish(
                () => FileUtils.Copy(m_ymlPath, Path.Combine(temporaryFolder, "script.yml")),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "collateral.csv"), Path.Combine(temporaryDsFolder, "collateral.csv")),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "AmortizationSchedule.csv"), Path.Combine(temporaryDsFolder, "AmortizationSchedule.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "PromisedCashflow.csv"), Path.Combine(temporaryDsFolder, "PromisedCashflow.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "Reinvestment.csv"), Path.Combine(temporaryDsFolder, "Reinvestment.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "PastVariables.csv"), Path.Combine(temporaryDsFolder, "PastVariables.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "CurrentVariables.csv"), Path.Combine(temporaryDsFolder, "CurrentVariables.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "FutureVariables.csv"), Path.Combine(temporaryDsFolder, "FutureVariables.csv"), false),
                () => FileUtils.Copy(Path.Combine(srcDsFolder, "CombinedVariables.csv"), Path.Combine(temporaryDsFolder, "CombinedVariables.csv"))
            );

            return new DealModelFolerInfo(temporaryFolder, temporaryDsFolder);
        }

        private void Run(double cdr, double cpr, AssetOverrideSetting assetOverrideSetting = null)
        {
            var folderInfo = CopyTemporaryModel();
            
            m_staticAnalysisResult = NancyUtils.RunStaticResultByPath(cdr.ToString(), cpr.ToString(),
                folderInfo.YmlFolder, folderInfo.DsFolder, assetOverrideSetting);

            ParallelUtils.Start(() => Directory.Delete(folderInfo.YmlFolder, true));
        }

        public string YmlFolder
        {
            get
            {
                var ymlFileName = "script.yml";
                return m_ymlPath.EndsWith(ymlFileName, StringComparison.CurrentCultureIgnoreCase)
                    ? m_ymlPath.Substring(0, m_ymlPath.Length - ymlFileName.Length) : m_ymlPath;
            }
        }

        public string DsFolder { get { return m_datasetPath; } }

        public DataTable AssetCashflowDt
        {
            get
            {
                if (m_staticAnalysisResult == null)
                {
                    Run(0, 0, this.m_datasetLogicModel.AssetOverrideSetting);
                }

                return m_staticAnalysisResult.AssetCashflowDt;
            }
        }

        public DataTable CashflowDt
        {
            get
            {
                if (m_staticAnalysisResult == null)
                {
                    Run(0, 0);
                }

                return m_staticAnalysisResult.CashflowDt;
            }
        }

        public OverrideSingleAssetLogicModel OverrideSingleAsset
        {
            get
            {
                if (m_overrideSingleAsset == null)
                {
                    m_overrideSingleAsset = new OverrideSingleAssetLogicModel(ProjectLogicModel,
                        m_datasetLogicModel.DatasetSchedule.PaymentDate);
                }
                return m_overrideSingleAsset;
            }
        }
        private OverrideSingleAssetLogicModel m_overrideSingleAsset;


        public NancyStaticAnalysisResult NancyStaticAnalysisResult { get { return m_staticAnalysisResult; } }

        private NancyStaticAnalysisResult m_staticAnalysisResult;

        private string m_ymlPath;

        private string m_datasetPath;

        private string m_asOfDate;

        private DatasetLogicModel m_datasetLogicModel;
    }
}
