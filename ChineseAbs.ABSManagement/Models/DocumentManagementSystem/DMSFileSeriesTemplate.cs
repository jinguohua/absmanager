using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.DocumentManagementSystem
{
    public enum DmsFileSeriesTemplateType
    {
        // ��
        None = 0,
        // ������䱨��
        IncomeDistributionReport = 1,
        // ר��ƻ�����ָ��
        SpecialPlanTransferInstruction = 2,
        // �Ҹ���Ϣȷ�ϱ�
        CashInterestRateConfirmForm = 3,
        // ��Ϣ��������
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
