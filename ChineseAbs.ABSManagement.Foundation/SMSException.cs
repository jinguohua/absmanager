using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Foundation
{
    public class SMSException : Exception
    {
        public SMSException() { }

        public SMSException(string message)
            : base(message)
        {

        }

        public SMSException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
