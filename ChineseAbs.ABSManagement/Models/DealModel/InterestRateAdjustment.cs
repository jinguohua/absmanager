using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public enum PrimeInterestRate
    {
        //三个月存款利率
        CNS003M = 1001,
        //六个月存款利率
        CNS006M = 1002,
        //一年期存款利率
        CNS012M = 1003,

        //六个月内贷款利率
        CNL006M = 2001,
        //六个月至一年贷款利率
        CNL012M = 2002,
        //一年至三年贷款利率
        CNL003Y = 2003,
        //三年至五年贷款利率
        CNL005Y = 2004,
        //五年以上贷款利率
        CNL010Y = 2005,

        //五年期以下（含五年）个人住房公积金贷款利率
        CNHL005Y = 3001,
        //五年期以上个人住房公积金贷款利率
        CNHL010Y = 3002,

        //一个月上海银行间同业拆放利率
        SHIBOR1M = 4001,
        //三个月上海银行间同业拆放利率
        SHIBOR3M = 4002,
        //六个月上海银行间同业拆放利率
        SHIBOR6M = 4003,
        //九个月上海银行间同业拆放利率
        SHIBOR9M = 4004,
        //一年上海银行间同业拆放利率
        SHIBOR1Y = 4005,
    }

    public class InterestRateAdjustment: BaseDataContainer<TableInterestRateAdjustment>
    {
        public InterestRateAdjustment()
        {
        }

        public InterestRateAdjustment(TableInterestRateAdjustment interestRateAdjustment)
            : base(interestRateAdjustment)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }


        public int InterestRateAdjustmentSetId { get; set; }

        public PrimeInterestRate InterestRateType { get; set; }

        public DateTime AdjustmentDate { get; set; }

        public double InterestRate { get; set; }

        public string InterestRateName { get; set; }
        

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableInterestRateAdjustment GetTableObject()
        {
            var obj = new TableInterestRateAdjustment();
            obj.interest_rate_adjustment_id = Id;
            obj.interest_rate_adjustment_guid = Guid;

            obj.interest_rate_adjustment_set_id = InterestRateAdjustmentSetId;
            obj.interest_rate_type_id = (int)InterestRateType;
            obj.adjustment_date = AdjustmentDate;
            obj.interest_rate = InterestRate;
            obj.interest_rate_name = InterestRateName;

            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableInterestRateAdjustment obj)
        {
            Id = obj.interest_rate_adjustment_id;
            Guid = obj.interest_rate_adjustment_guid;

            InterestRateAdjustmentSetId = obj.interest_rate_adjustment_set_id;
            InterestRateType = (PrimeInterestRate)obj.interest_rate_type_id;
            AdjustmentDate = obj.adjustment_date;
            InterestRate = obj.interest_rate;
            InterestRateName = obj.interest_rate_name;

            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
