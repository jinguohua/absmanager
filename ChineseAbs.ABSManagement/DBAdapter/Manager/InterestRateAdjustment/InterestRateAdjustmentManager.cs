using System;
using ChineseAbs.ABSManagement.Models;
using InterestRateAdjustmentModel = ChineseAbs.ABSManagement.Models.InterestRateAdjustment;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Manager.InterestRateAdjustment
{
    public class InterestRateAdjustmentManager : BaseManager
    {
        public InterestRateAdjustmentManager()
        {
            m_defaultTableName = "dbo.InterestRateAdjustment";
            m_defaultPrimaryKey = "interest_rate_adjustment_id";
            m_defaultHasRecordStatusField = true;
            m_defaultOrderBy = " ORDER BY interest_rate_type_id, adjustment_date ";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public InterestRateAdjustmentModel New(int interestRateAdjustmentSetId,
            PrimeInterestRate interestRateType, DateTime adjustmentDate, double interestRate)
        {
            var now = DateTime.Now;

            var interestRateAdjustment = new InterestRateAdjustmentModel();
            interestRateAdjustment.Guid = Guid.NewGuid().ToString();

            interestRateAdjustment.InterestRateAdjustmentSetId = interestRateAdjustmentSetId;
            interestRateAdjustment.InterestRateType = interestRateType;
            interestRateAdjustment.AdjustmentDate = adjustmentDate;
            interestRateAdjustment.InterestRate = interestRate;
            interestRateAdjustment.InterestRateName = CommUtils.ToCnString(interestRateType);

            interestRateAdjustment.CreateTime = now;
            interestRateAdjustment.CreateUserName = UserInfo.UserName;
            interestRateAdjustment.LastModifyTime = now;
            interestRateAdjustment.LastModifyUserName = UserInfo.UserName;
            interestRateAdjustment.RecordStatus = RecordStatus.Valid;
            interestRateAdjustment.Id = Insert(interestRateAdjustment.GetTableObject());
            return interestRateAdjustment;
        }

        public InterestRateAdjustmentModel GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableInterestRateAdjustment>("interest_rate_adjustment_guid", guid);
            return new InterestRateAdjustmentModel(record);
        }

        public List<InterestRateAdjustmentModel> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TableInterestRateAdjustment, string>("interest_rateadjustment_guid", guids);
            return records.ToList().ConvertAll(x => new InterestRateAdjustmentModel(x));
        }

        public List<InterestRateAdjustmentModel> GetByInterestRateAdjustmentSetId(int interestRateAdjustmentSetId)
        {
            var records = Select<ABSMgrConn.TableInterestRateAdjustment>("interest_rate_adjustment_set_id", interestRateAdjustmentSetId);
            return records.ToList().ConvertAll(x => new InterestRateAdjustmentModel(x));
        }

        public int Remove(InterestRateAdjustmentModel interestRateAdjustment)
        {
            interestRateAdjustment.RecordStatus = RecordStatus.Deleted;
            interestRateAdjustment.LastModifyTime = DateTime.Now;
            interestRateAdjustment.LastModifyUserName = UserInfo.UserName;
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, interestRateAdjustment.GetTableObject());
        }

        public InterestRateAdjustmentModel GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableInterestRateAdjustment>(id);
            return new InterestRateAdjustmentModel(record);
        }

        public InterestRateAdjustmentModel Update(InterestRateAdjustmentModel adjustment)
        {
            adjustment.LastModifyTime = DateTime.Now;
            adjustment.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, adjustment.GetTableObject());
            return adjustment;
        }
    }
}
