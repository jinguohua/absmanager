using ChineseAbs.ABSManagement.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.Email
{
    //已发送
    public class Draft : Sent
    {
        public Draft(string userName) : base(userName)
        {
            m_emailContextList.Select(x => x.IsDraft == 1);
        }

    }
}   
