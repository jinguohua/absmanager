using System;

namespace ChineseAbs.ABSManagement.Models
{
    class TaskComments
    {
        public int AccountBalanceId { get; set; }

        public int AccountId { get; set; }

        public DateTime AsOfDate { get; set; }

        public decimal? EndBalance { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string TimtStampUserName { get; set; }
    }
}
