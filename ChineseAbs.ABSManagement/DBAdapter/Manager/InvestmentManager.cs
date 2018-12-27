using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    public class InvestmentManager : BaseManager
    {
        public InvestmentManager()
        {
            m_defaultTableName = "dbo.Investment";
            m_defaultPrimaryKey = "investment_id";
            m_defaultOrderBy = "";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public List<Investment> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TableInvestment, string>("investment_guid", guids);
            return records.Select(x => new Investment(x)).ToList();
        }

        public Investment NewInvestment(Investment invesetment)
        {
            invesetment.Guid = Guid.NewGuid().ToString();
            invesetment.Id = Insert(invesetment.GetTableObject());
            return invesetment;
        }

        public List<Investment> GetInvestmentsByProjectId(int projectId)
        {
            var records = m_db.Fetch<ABSMgrConn.TableInvestment>(
                "SELECT * FROM dbo.Investment WHERE project_id = @0 AND record_status_id <> @1 ORDER BY investment_id",
                projectId, (int)RecordStatus.Deleted);

            return records.ConvertAll(x => new Investment(x));
        }

        public Page<Investment> GetInvestments(long pageNum, long itemsPerPage, List<int> investmentIds)
        {
            if (investmentIds.Count == 0)
            {
                return new Page<Investment>
                {
                    Items = new List<Investment>()
                };
            }

            var sqlCondition = "(" + string.Join(", ", investmentIds.ConvertAll(x => x.ToString())) + ")";
            var page = m_db.Page<ABSMgrConn.TableInvestment>(pageNum, itemsPerPage,
                    "SELECT * FROM dbo.Investment WHERE investment_id IN " + sqlCondition + " AND record_status_id <> @1 ORDER BY investment_id");

            var investments = new Page<Investment>().Parse(page);
            investments.Items = page.Items.ConvertAll(item => new Investment(item));

            return investments;
        }

        public Investment GetInvestment(string investmentGuid)
        {
            var records = m_db.Fetch<ABSMgrConn.TableInvestment>(
                "SELECT * FROM dbo.Investment WHERE investment_guid = @0 AND record_status_id <> @1 ORDER BY investment_id",
                investmentGuid, (int)RecordStatus.Deleted);

            CommUtils.AssertEquals(records.Count, 1,
                "Load multiply records from db, investmentGuid={0}", investmentGuid);

            return new Investment(records.Single());
        }

        public Investment GetInvestment(int investmentId) 
        {
            var records = m_db.Fetch<ABSMgrConn.TableInvestment>(
                "SELECT * FROM dbo.Investment WHERE investment_id = @0 AND record_status_id <> @1 ORDER BY investment_id",
                investmentId, (int)RecordStatus.Deleted);

            CommUtils.AssertEquals(records.Count, 1,
                "Load multiply records from db, investmentId={0}", investmentId);

            return new Investment(records.Single());
        }

        public int UpdateInvestment(Investment investment)
        {
            var investmentTable = investment.GetTableObject();
            return m_db.Update("Investment", "investment_id", investmentTable, investment.Id);
        }

        public int RemoveInvestment(Investment investment)
        {
            investment.RecordStatus = RecordStatus.Deleted;
            var count = UpdateInvestment(investment);
            CommUtils.AssertEquals(count, 1, "Remove investment failed : id={0};recordsCount={1}", investment.Id, count);
            return count;
        }
    }
}
