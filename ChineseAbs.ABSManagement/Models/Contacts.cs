using ABSMgrConn;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models
{
    public enum EDutyType
    {
        原始权益人 = 1,
        底层资产 = 2,
        托管行 = 3,
        登记托管机构 = 4,
        交易场所 = 5,
        计划管理人 = 6,
        投资人 = 7,
        评级机构 = 8,
        担保人 = 9,
        律师事务所 = 10,
        会计师事务所 = 11,

        财务顾问 = 12,
        差额支付承诺人 = 13,
        承销商 = 14,
        发起机构 = 15,
        发行人 = 16,
        监管银行 = 17,
        评估机构 = 18,
        受托机构 = 19,
        税务顾问 = 20,
        委托机构 = 21,
        项目安排人 = 22,
        证券化服务商 = 23,
        资产服务机构 = 24,
        资金保管机构 = 25,
        其它 = 26,
    }

    public class  Contacts : List<Contact>
    {

        public Contacts() { }

        public Contacts(IEnumerable<TableContacts> queryTable)
        {
            foreach (var item in queryTable)
            {
                this.Add(new Contact(item));
            }
        }

        public List<TableContacts> ToTableList()
        {
            var tableList = new List<TableContacts>();
            foreach (var item in this)
            {
                tableList.Add(item.GetTableObject());
            }
            return tableList;
        }

    }

    public class Contact : BaseDataContainer<TableContacts>
    {
        #region Constructors
        public Contact()
        {
        }
        public Contact(TableContacts contact)
            : base(contact)
        {
        }
        #endregion

        #region Properties

        public int ContactId { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public string OrganizationName { get; set; }

        public int DutyId 
        {
            get {
                return (int)DutyType;
            }
            set {
                this.DutyType = (EDutyType)Enum.Parse(typeof(EDutyType), value.ToString());
            }
        }

        public EDutyType DutyType { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CellPhone { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string Note { get; set; }

        #endregion

        public override TableContacts GetTableObject()
        {
            var rt = new TableContacts();
            rt.contact_id = this.ContactId;
            rt.contact_guid = this.Guid;
            rt.project_id = this.ProjectId;
            rt.organization_name = this.OrganizationName;
            rt.duty_id = this.DutyId;
            rt.name = this.Name;
            rt.email = this.Email;
            rt.cellPhone = this.CellPhone;
            rt.time_stamp = this.TimeStamp;
            rt.note = this.Note;
            return rt;
        }

        public override void FromTableObject(TableContacts obj)
        {
            this.ContactId = obj.contact_id;
            this.Guid = obj.contact_guid;
            this.ProjectId = obj.project_id;
            this.OrganizationName = obj.organization_name;
            this.DutyId = obj.duty_id;
            this.Name = obj.name;
            this.Email = obj.email;
            this.CellPhone = obj.cellPhone;
            this.TimeStamp = obj.time_stamp;
            this.Note = obj.note;
        }
    }
}
