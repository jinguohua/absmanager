using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public enum ProjectSeriesType
    {
        Undefined = 0,
        融资租赁 = 1,
        企业贷款 = 2,
        住房抵押贷款 = 3,
        汽车抵押贷款 = 4,
        个人消费贷款 = 5,
        REITs = 6,

        信托受益权 = 7,
        收费收益权 = 8,
        应收账款 = 9,
        住房公积金 = 10,
        专项信贷 = 11,
        金融租赁 = 12,
        小额贷款 = 13,
        不良资产重组 = 14,
        委托贷款 = 15,
        保理融资 = 16,
        其它 = 99,
    }

    public enum RecordStatus
    {
        Deleted = -1,
        Undefined = 0,
        Valid = 1,
    }

    public enum ProjectSeriesStage
    {
        Undefined = 0,
        发行 = 1,
        存续期 = 2,
        终止 = 3,
        清算 = 4,
    }

    public class ProjectSeries: BaseDataContainer<TableProjectSeries>
    {
        public ProjectSeries()
        {
            this.Guid = System.Guid.NewGuid().ToString();
        }

        public ProjectSeries(TableProjectSeries project)
            : base(project)
        {

        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        public ProjectSeriesType Type { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? EstimatedFinishTime { get; set; }

        public string PersonInCharge { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public ProjectSeriesStage Stage { get; set; }

        public string Email { get; set; }

        public override TableProjectSeries GetTableObject()
        {
            var obj = new TableProjectSeries();
            obj.project_series_id = Id;
            obj.project_series_guid = Guid;
            obj.name = Name;
            obj.type_id = (int)Type;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.estimated_finish_time = EstimatedFinishTime;
            obj.person_in_charge = PersonInCharge;
            obj.record_status_id = (int)RecordStatus;
            obj.project_series_stage_id = (int)Stage;
            obj.email = Email;
            return obj;
        }

        public override void FromTableObject(TableProjectSeries obj)
        {
            Id = obj.project_series_id;
            Guid = obj.project_series_guid;
            Name = obj.name;
            Type = (ProjectSeriesType)(obj.type_id);
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            EstimatedFinishTime = obj.estimated_finish_time;
            PersonInCharge = obj.person_in_charge;
            RecordStatus = (RecordStatus)obj.record_status_id;
            Stage = (ProjectSeriesStage)obj.project_series_stage_id;
            Email = obj.email;
        }
    }
}
