using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public enum ActivityObjectType
    {
        //产品系列
        ProjectSeries = 1,
        //产品
        Project = 2,
        //工作组
        TaskGroup = 3,
        //工作
        Task = 4,
        //项目成员
        TeamMember = 5,
        //日程
        Agenda = 6,
        //参与机构
        Contact = 7,
        //项目管理员
        TeamAdmin = 8,
        
    }

    public class ProjectActivity: BaseDataContainer<TableProjectActivity>
    {
        public ProjectActivity()
        {
        }

        public ProjectActivity(TableProjectActivity projectActivity)
            : base(projectActivity)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public ActivityObjectType ActivityObjectType { get; set; }

        public string ActivityObjectUniqueIdentifier { get; set; }

        public string Comment { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableProjectActivity GetTableObject()
        {
            var obj = new TableProjectActivity();
            obj.project_activity_id = Id;
            obj.project_activity_guid = Guid;
            obj.project_id = ProjectId;
            obj.project_activity_object_type_id = (int)ActivityObjectType;
            obj.project_activity_object_unique_identifier = ActivityObjectUniqueIdentifier;
            obj.project_activity_comment = Comment;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableProjectActivity obj)
        {
            Id = obj.project_activity_id;
            Guid = obj.project_activity_guid;
            ProjectId = obj.project_id;
            ActivityObjectType = (ActivityObjectType)obj.project_activity_object_type_id;
            ActivityObjectUniqueIdentifier = obj.project_activity_object_unique_identifier;
            Comment = obj.project_activity_comment;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
