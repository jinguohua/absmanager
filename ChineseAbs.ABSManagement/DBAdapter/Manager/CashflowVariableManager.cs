
using System;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.Models;
using System.Linq;


namespace ChineseAbs.ABSManagement.Manager
{
    public class CashflowVariableManager
        : BaseModelManager<CashflowVariable, ABSMgrConn.TableCashflowVariable>
    {
        public CashflowVariableManager()
        {
            m_defaultTableName = "dbo.CashflowVariable";
            m_defaultPrimaryKey = "cashflow_variable_id";
            m_defalutFieldPrefix = "cashflow_variable_";
        }

        public List<CashflowVariable> GetByPaymentDay(int projectId, DateTime paymentDay)
        {
            var records = m_db.Fetch<ABSMgrConn.TableCashflowVariable>(
                "SELECT * FROM " + m_defaultTableName
                + " where project_id = @0 and payment_date = @1 and record_status_id = @2", projectId, paymentDay,(int)RecordStatus.Valid);

            if (records.Count == 0)
            {
                return new List<CashflowVariable>();
            }

            return records.ToList().ConvertAll(x => new CashflowVariable(x));
        }
    }
}
