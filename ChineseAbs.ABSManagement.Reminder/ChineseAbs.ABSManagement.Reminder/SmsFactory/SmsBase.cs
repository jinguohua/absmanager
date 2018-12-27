using ChineseAbs.ABSManagement.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Reminder.SmsFactory
{
    public class SmsBase
    {
        public SmsBase()
        {

        }

        public static void SendSmsUserThread(string smsTo, params string[] smsContent)
        {
            SmsSender.SendAsync(smsTo, "ReminderCode", smsContent);
        }

    }
}
