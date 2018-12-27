using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Foundation
{
    public enum NancyResponseStatus
    {
        Success,
        Failed
    }

    public class NancyResult
    {
        public NancyResponseStatus Status { get; set; }

        public string Message { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }

        public string StackTrace { get; set; }
    }
}
