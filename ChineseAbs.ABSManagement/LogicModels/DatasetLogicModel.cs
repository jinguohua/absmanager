using ChineseAbs.ABSManagement.LogicModels.DealModel;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Utils;
using SFL.CDOAnalyser.MasterData;
using SFL.Enumerations;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class DatasetLogicModel : BaseLogicModel
    {
        public DatasetLogicModel(DatasetScheduleLogicModel datasetSchedule)
            : base(datasetSchedule.UserName, datasetSchedule.ProjectLogicModel)
        {
            DatasetSchedule = datasetSchedule;
            Initialize();
        }

        private void Initialize()
        {
            m_instance = m_project.Datasets.SingleOrDefault(x => x.PaymentDate.HasValue && x.PaymentDate.Value == DatasetSchedule.PaymentDate);
        }

        private Dataset m_instance;

        public Dataset Instance { get { return m_instance; } }

        public bool HasDealModel { get { return m_instance != null; } }

        public DatasetScheduleLogicModel DatasetSchedule { get; set; }

        private DatasetLogicModel m_previous;

        public DatasetLogicModel Previous
        {
            get
            {
                if (DatasetSchedule.Previous == null)
                {
                    return null;
                }
                
                if (m_previous == null)
                {
                    m_previous = new DatasetLogicModel(DatasetSchedule.Previous);
                    if (m_previous.Instance == null)
                    {
                        m_previous = null;
                    }
                }

                return m_previous;
            }
        }

        private DatasetLogicModel m_next;

        public DatasetLogicModel Next
        {
            get
            {
                if (DatasetSchedule.Next == null)
                {
                    return null;
                }

                if (m_next == null)
                {
                    m_next = new DatasetLogicModel(DatasetSchedule.Next);
                    if (m_next.Instance == null)
                    {
                        m_next = null;
                    }
                }

                return m_next;
            }
        }

        public string Folder
        {
            get
            {
                return Path.Combine(m_project.ModelFolder, m_instance.AsOfDate);
            }
        }

        public DealLogicModel DealModel
        {
            get
            {
                CommUtils.Assert(HasDealModel, "第{0}期模型未生成", DatasetSchedule.PaymentDate.ToShortDateString());

                if (m_dealModel == null)
                {
                    m_dealModel = new DealLogicModel(this, m_project.ModelFolder, Folder);
                }

                return m_dealModel;
            }
        }

        private DealLogicModel m_dealModel;


        public void LoadAssetCashflowInfo()
        {
            var basicAnalyticsData = NancyUtils.GetBasicAnalyticsData(m_project.Instance.ProjectId, null, m_instance.AsOfDate);
            var assetItems = basicAnalyticsData.BasicAssetCashflow.BasicAssetCashflowItems.Where(x => x.PaymentDate == DatasetSchedule.PaymentDate);

            foreach (var assetItem in assetItems)
            {
                var findAssets = Assets.Where(x => x.SecurityData.AssetId == assetItem.AssetId).ToList();
                if (findAssets.Count > 1)
                {
                    CommUtils.Assert(false, "找到了重复的AssetId[{0}]", assetItem.AssetId);
                }

                var findAsset = findAssets.SingleOrDefault();
                findAsset.BasicAsset = assetItem;
            }
        }

        public AssetLogicModel GetAssetById(int assetId)
        {
            CommUtils.AssertEquals(Assets.Count(x => x.AssetId == assetId), 1,
                "查找assetId={0}失败，产品[{1}]，偿付期[{2}]",
                ProjectLogicModel.Instance.Name, Instance.PaymentDate.Value);
            return Assets.Single(x => x.AssetId == assetId);
        }

        private List<AssetLogicModel> m_assets;

        public List<AssetLogicModel> Assets
        {
            get
            {
                if (m_assets == null)
                {
                    CSecurityData[] securities = new CSecurityData[0];
                    if (File.Exists(CollateralCsv.FilePath))
                    {
                        securities = CCollateralLoader.LoadSecurities(CollateralCsv.FilePath);
                    }

                    m_assets = new List<AssetLogicModel>();

                    foreach (var security in securities)
                    {
                        var assetLogicModel = new AssetLogicModel(this, security);
                        assetLogicModel.AssetId = security.AssetId;
                        m_assets.Add(assetLogicModel);
                    }

                    m_assets.ForEach(x => x.AmortizationType = GetAmortizationType(x.AssetId));
                }

                return m_assets;
            } 
        }

        private AmortizationType GetAmortizationType(int assetId)
        {
            var datasetCursor = this;
            while (datasetCursor != null)
            {
                var asset = datasetCursor.Assets.FirstOrDefault(x => x.SecurityData != null && x.SecurityData.AssetId == assetId);
                if (asset != null)
                {
                    var paymentMethod = asset.SecurityData.PaymentMethod;
                    if (paymentMethod == ZEnums.EPaymentMethod.EqualPmt)
                    {
                        return AmortizationType.EqualPmt;
                    }
                    else if (paymentMethod == ZEnums.EPaymentMethod.EqualPrin)
                    {
                        return AmortizationType.EqualPrin;
                    }
                    else if (paymentMethod == ZEnums.EPaymentMethod.UNDEFINEDENUM)
                    {
                        if (!datasetCursor.AmortizationSchedule.Any(x => x.AssetId == asset.AssetId))
                        {
                            return AmortizationType.SingleAmortization;
                        }
                    }
                }
                datasetCursor = datasetCursor.Next;
            }

            return AmortizationType.UserDefined;
        }

        private string GetCsvPath(string csvFileName)
        {
            var datasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(m_project.Instance, m_instance.AsOfDate);
            if (!Directory.Exists(datasetFolder))
            {
                return null;
            }

            var csvPathName = Path.Combine(datasetFolder, csvFileName);
            if (!File.Exists(csvPathName))
            {
                return null;
            }

            return csvPathName;
        }

        //private void GetVariablesCsvInstance<T>(string filePath, ref T csvInstance) where T : VariablesCsv, new()
        //{
        //    if (csvInstance == null)
        //    {
        //        csvInstance = new T();

        //        var path = GetCsvPath(filePath);
        //        if (!string.IsNullOrEmpty(path))
        //        {
        //            csvInstance.Load(path);
        //        }
        //    }
        //}

        //private FutureVariablesCsv m_futureVariablesCsv;

        //public FutureVariablesCsv FutureVariablesCsv
        //{
        //    get
        //    {
        //        GetVariablesCsvInstance("FutureVariables.csv", ref m_futureVariablesCsv);
        //        return m_futureVariablesCsv;
        //    }
        //}

        //private VariablesCsv m_currentVariablesCsv;

        //public VariablesCsv CurrentVariablesCsv
        //{
        //    get
        //    {
        //        GetVariablesCsvInstance("CurrentVariables.csv", ref m_currentVariablesCsv);
        //        return m_currentVariablesCsv;
        //    }
        //}

        //private VariablesCsv m_pastVariablesCsv;

        //public VariablesCsv PastVariablesCsv
        //{
        //    get
        //    {
        //        GetVariablesCsvInstance("PastVariables.csv", ref m_pastVariablesCsv);
        //        return m_pastVariablesCsv;
        //    }
        //}

        VariablesHelper m_Variables = null;
        public VariablesHelper Variables
        {
            get
            {
                if(m_Variables == null)
                {
                    m_Variables = new VariablesHelper(Folder);
                    m_Variables.Load();
                }
                return m_Variables;
            }
        }

        private CollateralCsv m_collateralCsv;

        public CollateralCsv CollateralCsv
        {
            get
            {
                if (m_collateralCsv == null)
                {
                    m_collateralCsv = new CollateralCsv();
                    var path = GetCsvPath("collateral.csv");
                    if (!string.IsNullOrEmpty(path))
                    {
                        m_collateralCsv.Load(path);
                    }
                }

                return m_collateralCsv;
            }
        }

        private AmortizationSchedule m_amortizationSchedule;

        public AmortizationSchedule AmortizationSchedule
        {
            get
            {
                if (m_amortizationSchedule == null)
                {
                    m_amortizationSchedule = new AmortizationSchedule();
                    var path = GetCsvPath("AmortizationSchedule.csv");
                    if (!string.IsNullOrEmpty(path))
                    {
                        m_amortizationSchedule.Load(path);
                    }
                }

                return m_amortizationSchedule;
            }
        }

        private ReinvestmentCsv m_reinvestmentCsv;

        public ReinvestmentCsv ReinvestmentCsv
        {
            get
            {
                if (m_reinvestmentCsv == null)
                {
                    var path = GetCsvPath("Reinvestment.csv");

                    if (!string.IsNullOrEmpty(path))
                    {
                        m_reinvestmentCsv = new ReinvestmentCsv();
                        m_reinvestmentCsv.Load(path);
                    }
                }

                return m_reinvestmentCsv;
            }
        }

        public AssetOverrideSetting AssetOverrideSetting
        {
            get
            {
                if (m_assetOverrideSetting == null)
                {
                    var assetCashflowVariable = m_dbAdapter.AssetCashflowVariable.GetByProjectIdPaymentDay(
                        m_project.Instance.ProjectId, DatasetSchedule.PaymentDate);

                    m_assetOverrideSetting = new AssetOverrideSetting(assetCashflowVariable);
                }

                return m_assetOverrideSetting;
            }
        }

        private AssetOverrideSetting m_assetOverrideSetting;
    }
}
