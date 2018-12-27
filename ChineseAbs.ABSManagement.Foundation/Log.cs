using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Foundation
{
    public static class Log
    {
        private static log4net.ILog m_log = log4net.LogManager.GetLogger("ChineseAbs.ABSManagement");

        public static void Info(object message)
        {
            m_log.Info(message);
        }

        public static void Error(object message)
        {
            m_log.Error(message);
        }

        public static void Error(object message, Exception e)
        {
            m_log.Error(message, e);
        }
    }
}
