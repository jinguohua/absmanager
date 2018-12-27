using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class UserActionHabits : BaseModel<TableUserActionHabits>
    {
        public UserActionHabits()
        {

        }

        public UserActionHabits(TableUserActionHabits obj)
            : base(obj)
        {

        }
        public string UserName { get; set; }
        public string ActionCategoryName { get; set; }
        public string ActionName { get; set; }
        public string ActionSetting { get; set; }

        public override TableUserActionHabits GetTableObject()
        {
            var obj = new TableUserActionHabits();
            obj.user_action_habits_id = Id;
            obj.user_action_habits_guid = Guid;
            obj.user_name = UserName;
            obj.action_category_name = ActionCategoryName;
            obj.action_name = ActionName;
            obj.action_setting = ActionSetting;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableUserActionHabits obj)
        {
            Id = obj.user_action_habits_id;
            Guid = obj.user_action_habits_guid;
            UserName = obj.user_name;
            ActionCategoryName = obj.action_category_name;
            ActionName = obj.action_name;
            ActionSetting = obj.action_setting;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
