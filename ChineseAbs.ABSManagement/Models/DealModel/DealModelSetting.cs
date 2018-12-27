using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class DealModelSetting : BaseModel<TableDealModelSetting>
    {
        public DealModelSetting()
        {

        }

        public DealModelSetting(TableDealModelSetting obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public bool EnablePredictMode { get; set; }

        public override TableDealModelSetting GetTableObject()
        {
            var obj = new TableDealModelSetting();
            obj.deal_model_setting_id = Id;
            obj.deal_model_setting_guid = Guid;
            obj.project_id = ProjectId;
            obj.enable_predict_mode = EnablePredictMode ? 1 : 0;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableDealModelSetting obj)
        {
            Id = obj.deal_model_setting_id;
            Guid = obj.deal_model_setting_guid;
            ProjectId = obj.project_id;
            EnablePredictMode = obj.enable_predict_mode == 1;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
