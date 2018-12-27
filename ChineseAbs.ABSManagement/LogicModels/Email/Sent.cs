using ChineseAbs.ABSManagement.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.Email
{
    //已发送
    public class Sent : EmailBaseLogic
    {
        public Sent(string userName) : base(userName)
        {
            GetAllMsg(() => m_dbAdapter.EmailFromTo.GetByFrom(EmailAddress));
        }




    }
}
