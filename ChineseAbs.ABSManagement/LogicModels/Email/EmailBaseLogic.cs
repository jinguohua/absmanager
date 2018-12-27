using ChineseAbs.ABSManagement.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.Email
{
    public abstract class EmailBaseLogic
    {
        public EmailBaseLogic(string userName)
        {
            m_userName = userName;
            EmailAddress = m_dbAdapter.Authority.GetAccountInfoByUserName(userName).email;
        }

        private string m_userName;
        public string UserName { get { return m_userName; } }

        public string EmailAddress { get; }

        protected DBAdapter m_dbAdapter
        {
            get { return new DBAdapter(m_userName); }
        }

        //获取所有信息
        public Tuple<Dictionary<int, EmailContext>, List<EmailFromTo>> GetAllMsg(Func<List<EmailFromTo>> func)
        {
            m_emailFromToList = func();
            m_emailContextList = m_dbAdapter.EmailContext.GetByIds(m_emailFromToList.Select(x => x.EmailcontextId));
            m_emailContextDict = m_emailContextList.ToDictionary(x => x.Id);
            return new Tuple<Dictionary<int, EmailContext>, List<EmailFromTo>>(m_emailContextDict, m_emailFromToList);
        }

        public void DeleteEmail(List<string> Guids)
        {
            m_dbAdapter.EmailFromTo.DeleteByGuids(Guids);
        }

        public List<EmailFromTo> m_emailFromToList;
        public List<EmailContext> m_emailContextList;
        public Dictionary<int, EmailContext> m_emailContextDict;

        //总邮件数
        // public int m_count;
        public int Count { get { return m_emailFromToList.Count; } }


    }
}
