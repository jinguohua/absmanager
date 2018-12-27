using ChineseAbs.CalcService.Data.NancyData.Cashflows;
using SFL.CDOAnalyser.MasterData;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class AssetLogicModel : BaseLogicModel
    {
        public AssetLogicModel(DatasetLogicModel datasetLogicModel, CSecurityData securityData)
            : base(datasetLogicModel.ProjectLogicModel)
        {
            m_datasetLogicModel = datasetLogicModel;
            SecurityData = securityData;
        }

        public NancyBasicAssetCashflowItem BasicAsset
        {
            get
            {
                if (m_basicAsset == null)
                {
                    m_datasetLogicModel.LoadAssetCashflowInfo();
                }

                return m_basicAsset;
            }

            set
            {
                m_basicAsset = value;
            }
        }

        private NancyBasicAssetCashflowItem m_basicAsset;

        public CSecurityData SecurityData { get; set; }

        public int AssetId { get; set; }

        public AmortizationType AmortizationType { get; set; }

        public DatasetLogicModel Dataset { get { return m_datasetLogicModel; } }

        private DatasetLogicModel m_datasetLogicModel;

        public bool IsEqualPmtOrEqualPrin
        {
            get
            {
                return AmortizationType == AmortizationType.EqualPmt
                    || AmortizationType == AmortizationType.EqualPrin;
            }
        }

        public AmortizationLogicModel Amortization
        {
            get
            {
                if (m_amortization == null)
                {
                    m_amortization = new AmortizationLogicModel(this);
                }

                return m_amortization;
            }
        }

        private AmortizationLogicModel m_amortization;
    }

    //资产偿付类型
    public enum AmortizationType
    {
        //未定义
        Undefined,
        //固定期限摊还
        UserDefined,
        //等额本息
        EqualPmt,
        //等额本金
        EqualPrin,
        //到期一次偿清
        SingleAmortization
    }
}
