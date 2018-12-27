using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class Template : BaseDataContainer<TableTemplate>
    {
        public Template()
        {
        }

        public Template(TableTemplate template)
            : base(template)
        {
        }

        public int AccountBalanceId { get; set; }

        public int AccountId { get; set; }

        public DateTime AsOfDate { get; set; }

        public decimal? EndBalance { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string TimtStampUserName { get; set; }

        public int TemplateId { set; get; }

        public string TemplateGuid { set; get; }

        public string TemplateName { set; get; }

        public string CreateUser { set; get; }

        public DateTime? CreateTime { set; get; }

        public override TableTemplate GetTableObject()
        {
            var template = new TableTemplate();
            template.template_id = TemplateId;
            template.template_guid = TemplateGuid;
            template.template_name = TemplateName;
            template.create_user = CreateUser;
            template.create_time = CreateTime;
            return template;
        }

        public override void FromTableObject(TableTemplate template)
        {
            TemplateId = template.template_id;
            TemplateGuid = template.template_guid;
            TemplateName = template.template_name;
            CreateUser = template.create_user;
            CreateTime = template.create_time;
        }
    }
}
