using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.DocumentManagementSystem
{
    public enum DmsFileSeriesTemplateType
    {
        // 无
        None = 0,
        // 收益分配报告
        IncomeDistributionReport = 1,
        // 专项计划划款指令
        SpecialPlanTransferInstruction = 2,
        // 兑付兑息确认表
        CashInterestRateConfirmForm = 3,
        // 付息方案申请
        InterestPaymentPlanApplication = 4,
    }

    public class DMSFileSeriesTemplate : BaseModel<TableDMSFileSeriesTemplate>
    {
        public DMSFileSeriesTemplate()
        {

        }

        public DMSFileSeriesTemplate(TableDMSFileSeriesTemplate obj)
            : base(obj)
        {

        }

        public int FileSeriesId { get; set; }
        public DmsFileSeriesTemplateType TemplateType { get; set; }

        public override TableDMSFileSeriesTemplate GetTableObject()
        {
            var obj = new TableDMSFileSeriesTemplate();
            obj.dms_file_series_template_id = Id;
            obj.dms_file_series_template_guid = Guid;
            obj.dms_file_series_id = FileSeriesId;
            obj.dms_file_series_template_type_id = (int)TemplateType;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableDMSFileSeriesTemplate obj)
        {
            Id = obj.dms_file_series_template_id;
            Guid = obj.dms_file_series_template_guid;
            FileSeriesId = obj.dms_file_series_id;
            TemplateType = (DmsFileSeriesTemplateType)obj.dms_file_series_template_type_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
