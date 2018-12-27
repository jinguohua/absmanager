using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels.OverrideSingleAsset
{
    public class OverrideSingleAssetLogicModel : BaseLogicModel
    {
        public OverrideSingleAssetLogicModel(ProjectLogicModel project, DateTime paymentDay)
            : base(project)
        {
            m_paymentDay = paymentDay;
        }

        public OverrideSingleAssetLogicModel(ProjectLogicModel project, DateTime paymentDay,
            List<OverrideSingleAssetPrincipal> principalRecords,
            List<OverrideSingleAssetInterest> interestRecords,
            List<OverrideSingleAssetPrincipalBalance> principalBalanceRecords)
            : base(project)
        {
            m_paymentDay = paymentDay;
            m_principalRecords = principalRecords.Where(x => x.ProjectId == project.Instance.ProjectId && x.PaymentDate == paymentDay).ToList();
            m_interestRecords = interestRecords.Where(x => x.ProjectId == project.Instance.ProjectId && x.PaymentDate == paymentDay).ToList();
            m_principalBalanceRecords = principalBalanceRecords.Where(x => x.ProjectId == project.Instance.ProjectId && x.PaymentDate == paymentDay).ToList();
        }

        private List<OverrideSingleAssetPrincipal> m_principalRecords;

        public OverrideSingleAssetPrincipal GetPrincipal(int assetId)
        {
            if (m_principalRecords == null)
            {
                m_principalRecords = m_dbAdapter.OverrideSingleAssetPrincipal.GetByPaymentDay(ProjectLogicModel.Instance.ProjectId, m_paymentDay);
            }

            return m_principalRecords.Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.CreateTime).FirstOrDefault();
        }

        private List<OverrideSingleAssetInterest> m_interestRecords;

        public OverrideSingleAssetInterest GetInterest(int assetId)
        {
            if (m_interestRecords == null)
            {
                m_interestRecords = m_dbAdapter.OverrideSingleAssetInterest.GetByPaymentDay(ProjectLogicModel.Instance.ProjectId, m_paymentDay);
            }

            return m_interestRecords.Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.CreateTime).FirstOrDefault();
        }

        private List<OverrideSingleAssetPrincipalBalance> m_principalBalanceRecords;

        public OverrideSingleAssetPrincipalBalance GetPrincipalBalance(int assetId)
        {
            if (m_principalBalanceRecords == null)
            {
                m_principalBalanceRecords = m_dbAdapter.OverrideSingleAssetPrincipalBalance.GetByPaymentDay(ProjectLogicModel.Instance.ProjectId, m_paymentDay);
            }

            return m_principalBalanceRecords.Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.CreateTime).FirstOrDefault();
        }

        public void OverridePrincipal(int assetId, double principal, string comment)
        {
            var project = ProjectLogicModel;
            var osaPrincipal = new OverrideSingleAssetPrincipal();
            osaPrincipal.ProjectId = project.Instance.ProjectId;
            osaPrincipal.PaymentDate = m_paymentDay;
            osaPrincipal.AssetId = assetId;
            osaPrincipal.Principal = principal;
            osaPrincipal.Comment = comment;
            m_dbAdapter.OverrideSingleAssetPrincipal.New(osaPrincipal);
        }

        public void OverridePrincipalBalance(int assetId, double principalBalance, string comment)
        {
            var project = ProjectLogicModel;
            var osaPrincipalBalance = new OverrideSingleAssetPrincipalBalance();
            osaPrincipalBalance.ProjectId = project.Instance.ProjectId;
            osaPrincipalBalance.PaymentDate = m_paymentDay;
            osaPrincipalBalance.AssetId = assetId;
            osaPrincipalBalance.PrincipalBalance = principalBalance;
            osaPrincipalBalance.Comment = comment;
            m_dbAdapter.OverrideSingleAssetPrincipalBalance.New(osaPrincipalBalance);
        }

        public void OverrideInterest(int assetId, double interest, string comment)
        {
            var project = ProjectLogicModel;
            var osaInterest = new OverrideSingleAssetInterest();
            osaInterest.ProjectId = project.Instance.ProjectId;
            osaInterest.PaymentDate = m_paymentDay;
            osaInterest.AssetId = assetId;
            osaInterest.Interest = interest;
            osaInterest.Comment = comment;
            m_dbAdapter.OverrideSingleAssetInterest.New(osaInterest);
        }

        public bool HasOverrideRecords(int assetId)
        {
            return GetPrincipal(assetId) != null || GetPrincipalBalance(assetId) != null || GetInterest(assetId) != null;
        }

        public void ClearAllOverrideHistory(int projectId, DateTime paymentDate)
        {
            m_dbAdapter.OverrideSingleAssetInterest.RemoveAll(projectId, paymentDate);
            m_dbAdapter.OverrideSingleAssetPrincipal.RemoveAll(projectId, paymentDate);
            m_dbAdapter.OverrideSingleAssetPrincipalBalance.RemoveAll(projectId, paymentDate);
        }

        public DateTime PaymentDay { get { return m_paymentDay; } }

        private DateTime m_paymentDay;
    }
}
