using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Manager.InterestRateAdjustment
{
    public class InterestRateAdjustmentSetManager : BaseManager
    {
        public InterestRateAdjustmentSetManager()
        {
            m_defaultTableName = "dbo.InterestRateAdjustmentSet";
            m_defaultPrimaryKey = "interest_rate_adjustment_set_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public InterestRateAdjustmentSet New(int projectId, DateTime paymentDate, string name)
        {
            var now = DateTime.Now;

            var interestRateAdjustmentSet = new InterestRateAdjustmentSet();
            interestRateAdjustmentSet.Guid = Guid.NewGuid().ToString();

            interestRateAdjustmentSet.ProjectId = projectId;
            interestRateAdjustmentSet.PaymentDate = paymentDate;
            interestRateAdjustmentSet.Name = name;

            interestRateAdjustmentSet.CreateTime = now;
            interestRateAdjustmentSet.CreateUserName = UserInfo.UserName;
            interestRateAdjustmentSet.LastModifyTime = now;
            interestRateAdjustmentSet.LastModifyUserName = UserInfo.UserName;
            interestRateAdjustmentSet.RecordStatus = RecordStatus.Valid;
            interestRateAdjustmentSet.Id = Insert(interestRateAdjustmentSet.GetTableObject());
            return interestRateAdjustmentSet;
        }

        public InterestRateAdjustmentSet GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableInterestRateAdjustmentSet>("interest_rate_adjustment_set_guid", guid);
            return new InterestRateAdjustmentSet(record);
        }

        public InterestRateAdjustmentSet GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableInterestRateAdjustmentSet>(id);
            return new InterestRateAdjustmentSet(record);
        }
    }
}
