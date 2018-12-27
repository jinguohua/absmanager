using ChineseAbs.ABSManagement.Models.DealModel;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.DealModel
{
    public class AssetCashflowVariableManager
        : BaseModelManager<AssetCashflowVariable, ABSMgrConn.TableAssetCashflowVariable>
    {
        public AssetCashflowVariableManager()
        {
            m_defaultTableName = "dbo.AssetCashflowVariable";
            m_defaultPrimaryKey = "asset_cashflow_variable_id";
            m_defalutFieldPrefix = "asset_cashflow_variable_";
        }

        public AssetCashflowVariable GetByProjectIdPaymentDay(int projectId, DateTime paymentDay)
        {
            var records = m_db.Fetch<ABSMgrConn.TableAssetCashflowVariable>(
                "SELECT * FROM " + m_defaultTableName
                + " where project_id = @0 and payment_date = @1", projectId, paymentDay);

            if (records.Count == 0)
            {
                return null;
            }
            else if (records.Count > 1)
            {
                throw new ApplicationException("Get date failed,data of number greater than two,please contact the admin.");
            }

            return new AssetCashflowVariable(records.First()); 
        }

        public bool IsExist(int projectId, DateTime paymentDay)
        {
            var assetCashflowVariable = GetByProjectIdPaymentDay(projectId, paymentDay);
            return assetCashflowVariable != null;
        }
    }
}
