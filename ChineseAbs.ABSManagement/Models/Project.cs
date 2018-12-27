using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class Project: BaseDataContainer<TableProject>
    {
        public Project()
        {
            this.ProjectGuid = Guid.NewGuid().ToString();
        }

        public Project(TableProject project): base(project)
        {

        }

        public int ProjectId { get; set; }

        public string ProjectGuid { get; set; }

        public int? ProjectSeriesId { get; set; }

        public string Name { get; set; }

        public int? TypeId { get; set; }

        public int ModelId { get; set; }

        public int? CnabsDealId { get; set; }

        public string DealType { get; set; }

        public Model Model { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public ProjectDealInfo ProjectDealInfo { get; set; }

        public DateTime? NextCalcDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        public bool IsSpecified { get; set; }

        public override TableProject GetTableObject()
        {
            var project = new TableProject();
            project.project_id = ProjectId;
            project.project_guid = ProjectGuid;
            project.project_series_id = ProjectSeriesId;
            project.name = Name;
            project.type_id = TypeId;
            project.cnabs_deal_id = CnabsDealId;
            project.model_id = ModelId;
            project.create_user_name = CreateUserName;
            project.create_time = CreateTime;
            project.time_stamp = TimeStamp;
            project.time_stamp_user_name = TimeStampUserName;
            project.record_status_id = (int)RecordStatus;
            return project;
        }

        public override void FromTableObject(TableProject project)
        {
            ProjectId = project.project_id;
            ProjectGuid = project.project_guid;
            ProjectSeriesId = project.project_series_id;
            Name = project.name;
            TypeId = project.type_id;
            CnabsDealId = project.cnabs_deal_id;
            ModelId = project.model_id;
            CreateUserName = project.create_user_name;
            CreateTime = project.create_time;
            TimeStamp = project.time_stamp;
            TimeStampUserName = project.time_stamp_user_name;
            RecordStatus = (RecordStatus)project.record_status_id;
        }
    }

    public class ProjectDealInfo
    {
        public string DealNameChinese { get; set; }

        public string Originator { get; set; }

        public string Issuer { get; set; }

        public string Trustee { get; set; }

        public string Servicer { get; set; }

        public string TotalOffering { get; set; }

        public DateTime? LegalMaturity { get; set; }

        public string Frequency { get; set; }

        public DateTime? ClosingDate { get; set; }

        public DateTime? FirstPaymentDate { get; set; }

        public DateTime? CurrentPaymentDate { get; set; }

        public int? AssetCount { get; set; }

        public int? IssuerCount { get; set; }

        public double? Wal { get; set; }

        public double? Wac { get; set; }

        public string LawFirm { get; set; }

        public string AccountingFirm { get; set; }

        public void FromTableObject(ABSMgrConn.VwProject obj)
        {
            DealNameChinese = obj.deal_name_chinese;
            Originator = obj.originator;
            Issuer = obj.issuer;
            Trustee = obj.trustee;
            Servicer = obj.servicer;
            TotalOffering = obj.total_offering;
            LegalMaturity = obj.legal_maturity;
            Frequency = obj.frequency;
            ClosingDate = obj.closing_date;
            FirstPaymentDate = obj.first_payment_date;
            CurrentPaymentDate = obj.current_payment_date;
            AssetCount = obj.asset_count;
            IssuerCount = obj.issuer_count;
            Wal = obj.wal;
            Wac = obj.wac;
            LawFirm = obj.law_firm;
            AccountingFirm = obj.accounting_firm;
        }
    }
}
