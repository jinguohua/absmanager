using ABSMgrConn;
using ChineseAbs.ABSManagement.Models.Email;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.Email
{
    public class EmailFromToManager
        : BaseModelManager<EmailFromTo, ABSMgrConn.TableEmailFromTo>
    {
        public EmailFromToManager()
        {
            m_defaultTableName = "dbo.EmailFromTo";
            m_defaultPrimaryKey = "email_fromto_id";
            m_defalutFieldPrefix = "email_fromto_";
        }

        public List<EmailFromTo> GetByTo(string emailAddress)
        {
            var records = Select<TableEmailFromTo>("to", emailAddress);
            return records.Select(x => new EmailFromTo(x)).ToList();
        }

        public List<EmailFromTo> GetByFrom(string emailAddress)
        {
            var records = Select<TableEmailFromTo>("from", emailAddress);
            return records.Select(x => new EmailFromTo(x)).ToList();
        }

        public void DeleteByGuids(List<string> guids)
        {
            GetByGuids(guids).ForEach(x =>
           {
               x.RecordStatus = Models.RecordStatus.Deleted;
               Update(x);
           });
        }

    }
}
