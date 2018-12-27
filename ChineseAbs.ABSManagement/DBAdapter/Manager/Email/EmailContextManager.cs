using ChineseAbs.ABSManagement.Models.Email;

namespace ChineseAbs.ABSManagement.Manager.Email
{
    public class EmailContextManager
        : BaseModelManager<EmailContext, ABSMgrConn.TableEmailContext>
    {
        public EmailContextManager()
        {
            m_defaultTableName = "dbo.EmailContext";
            m_defaultPrimaryKey = "email_context_id";
            m_defalutFieldPrefix = "email_context_";
        }
       
      //  public new(string subject,string )
    }
}
