using ChineseAbs.ABSManagement.Models;
using System;

namespace ChineseAbs.ABSManagement.Utils
{
    static public class TranslateUtils
    {
        public static string ToCnString(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Undefined:
                    return "未定义";
                case TaskStatus.Waitting:
                    return "等待";
                case TaskStatus.Running:
                    return "进行中";
                case TaskStatus.Finished:
                    return "完成";
                case TaskStatus.Skipped:
                    return "跳过";
                case TaskStatus.Overdue:
                    return "逾期";
                case TaskStatus.Error:
                    return "错误";
                default:
                    throw new ApplicationException("未定义的TaskStatus");
            }
        }


    }
}
