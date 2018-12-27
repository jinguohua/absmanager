using System;
using ChineseAbs.ABSManagement.Models;
using PrepaymentModel = ChineseAbs.ABSManagement.Models.Prepayment;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.Prepayment
{
    public class PrepaymentManager : BaseManager
    {
        public PrepaymentManager()
        {
            m_defaultTableName = "dbo.Prepayment";
            m_defaultPrimaryKey = "prepayment_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public PrepaymentModel New(int prepaymentSetId, int assetId, DateTime prepayDate, DateTime originDate, double money)
        {
            var now = DateTime.Now;

            var prepayment = new PrepaymentModel();
            prepayment.Guid = Guid.NewGuid().ToString();

            prepayment.PrepaymentSetId = prepaymentSetId;
            prepayment.AssetId = assetId;
            prepayment.PrepayDate = prepayDate;
            prepayment.Money = money;
            prepayment.OriginDate = originDate;

            prepayment.CreateTime = now;
            prepayment.CreateUserName = UserInfo.UserName;
            prepayment.LastModifyTime = now;
            prepayment.LastModifyUserName = UserInfo.UserName;
            prepayment.RecordStatus = RecordStatus.Valid;
            prepayment.Id = Insert(prepayment.GetTableObject());
            return prepayment;
        }

        public PrepaymentModel GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TablePrepayment>("prepayment_guid", guid);
            return new PrepaymentModel(record);
        }

        public List<PrepaymentModel> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TablePrepayment, string>("prepayment_guid", guids);
            return records.ToList().ConvertAll(x => new PrepaymentModel(x));
        }

        public List<PrepaymentModel> GetByPrepaymentSetId(int prepaymentSetId)
        {
            var records = Select<ABSMgrConn.TablePrepayment>("prepayment_set_id", prepaymentSetId);
            return records.ToList().ConvertAll(x => new PrepaymentModel(x));
        }

        public int Remove(PrepaymentModel prepayment)
        {
            prepayment.RecordStatus = RecordStatus.Deleted;
            prepayment.LastModifyTime = DateTime.Now;
            prepayment.LastModifyUserName = UserInfo.UserName;
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, prepayment.GetTableObject());
        }

        public PrepaymentModel GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TablePrepayment>(id);
            return new PrepaymentModel(record);
        }
    }
}
