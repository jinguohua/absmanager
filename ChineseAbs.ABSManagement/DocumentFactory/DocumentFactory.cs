using ChineseAbs.ABSManagement.DocumentFactory.Maker;
using ChineseAbs.ABSManagement.DocumentFactory.Maker.Demo;
using ChineseAbs.ABSManagement.Pattern;
using System.IO;

namespace ChineseAbs.ABSManagement.DocumentFactory
{
    public class DocumentFactory
    {
        public DocumentFactory(string userName)
        {
            m_userName = userName;
        }

        public void Generate(DocPatternType documentType, MemoryStream ms, params object[] param)
        {
            var maker = GetDocumentMaker(documentType);
            maker.Generate(ms, param);
        }

        private DocumentMakerBase GetDocumentMaker(DocPatternType documentType)
        {
            DocumentMakerBase maker = null;
            switch (documentType)
            {
                case DocPatternType.IncomeDistributionReport:
                    maker = new IncomeDistributionReportMaker(m_userName);
                    break;

                case DocPatternType.SpecialPlanTransferInstruction:
                    maker = new SpecialPlanTransferInstructionMaker(m_userName);
                    break;
                case DocPatternType.CashInterestRateConfirmForm:
                    maker = new CashInterestRateConfirmFormMaker(m_userName);
                    break;
                case DocPatternType.InterestPaymentPlanApplication:
                    maker = new InterestPaymentPlanApplicationMaker(m_userName);
                    break;
                case DocPatternType.DemoJianYuanReport:
                    maker = new DemoJianYuanReportMaker(m_userName);
                    break;
            }

            return maker;
        }

        private string m_userName;
    }
}
