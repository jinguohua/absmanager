using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class Prepayment: BaseDataContainer<TablePrepayment>
    {
        public Prepayment()
        {
        }

        public Prepayment(TablePrepayment prepayment)
            : base(prepayment)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int PrepaymentSetId { get; set; }

        public int AssetId { get; set; }

        public DateTime PrepayDate { get; set; }

        public double Money { get; set; }

        public DateTime OriginDate { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TablePrepayment GetTableObject()
        {
            var obj = new TablePrepayment();
            obj.prepayment_id = Id;
            obj.prepayment_guid = Guid;

            obj.prepayment_set_id = PrepaymentSetId;
            obj.asset_id = AssetId;
            obj.prepay_date = PrepayDate;
            obj.money = Money;
            obj.origin_date = OriginDate;

            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TablePrepayment obj)
        {
            Id = obj.prepayment_id;
            Guid = obj.prepayment_guid;

            PrepaymentSetId = obj.prepayment_set_id;
            AssetId = obj.asset_id;
            PrepayDate = obj.prepay_date;
            Money = obj.money;
            OriginDate = obj.origin_date;

            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
