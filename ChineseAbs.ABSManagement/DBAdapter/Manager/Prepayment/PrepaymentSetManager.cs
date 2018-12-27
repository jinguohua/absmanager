using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Manager.Prepayment
{
    public class PrepaymentSetManager : BaseManager
    {
        public PrepaymentSetManager()
        {
            m_defaultTableName = "dbo.PrepaymentSet";
            m_defaultPrimaryKey = "prepayment_set_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public PrepaymentSet New(int projectId, DateTime paymentDate, string name)
        {
            var now = DateTime.Now;

            var prepaymentSet = new PrepaymentSet();
            prepaymentSet.Guid = Guid.NewGuid().ToString();

            prepaymentSet.ProjectId = projectId;
            prepaymentSet.PaymentDate = paymentDate;
            prepaymentSet.Name = name;

            prepaymentSet.CreateTime = now;
            prepaymentSet.CreateUserName = UserInfo.UserName;
            prepaymentSet.LastModifyTime = now;
            prepaymentSet.LastModifyUserName = UserInfo.UserName;
            prepaymentSet.RecordStatus = RecordStatus.Valid;
            prepaymentSet.Id = Insert(prepaymentSet.GetTableObject());
            return prepaymentSet;
        }

        public PrepaymentSet GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TablePrepaymentSet>("prepayment_set_guid", guid);
            return new PrepaymentSet(record);
        }

        public PrepaymentSet GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TablePrepaymentSet>(id);
            return new PrepaymentSet(record);
        }
    }
}
