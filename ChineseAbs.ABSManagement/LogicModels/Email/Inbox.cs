using ChineseAbs.ABSManagement.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.Email
{
    //收件箱
    public class Inbox : EmailBaseLogic
    {
        public Inbox(string userName) : base(userName)
        {
            GetAllMsg(() => m_dbAdapter.EmailFromTo.GetByTo(EmailAddress));
            m_unReadCount = m_emailFromToList.Count(x => x.EmailRead == 0);
        }

        private int m_unReadCount;
        public int UnReadCount { get { return m_unReadCount; } }

        //标记为已读
        public void SignAsRead(List<string> Guids)
        {
            var emailList = m_dbAdapter.EmailFromTo.GetByGuids(Guids);
            emailList.ForEach(x =>
            {
                x.EmailRead = 1;
                m_dbAdapter.EmailFromTo.Update(x);
            });

        }


    }
}
