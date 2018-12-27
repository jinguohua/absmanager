using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class EditModelCSV : BaseModel<TableEditModelCSV>
    {
        public EditModelCSV()
        {

        }

        public EditModelCSV(TableEditModelCSV obj)
            : base(obj)
        {

        }
        public int EditModelCsvId { get; set; }
        public string EditModelCsvGuid { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string ProjectGuid { get; set; }
        public string Asofdate { get; set; }

        public override TableEditModelCSV GetTableObject()
        {
            var obj = new TableEditModelCSV();
            obj.edit_model_csv_id = Id;
            obj.edit_model_csv_guid = Guid;
             obj.project_guid = ProjectGuid ;
             obj.asofdate = Asofdate;
            obj.type = Type;
            obj.title = Title;
            obj.comment = Comment;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableEditModelCSV obj)
        {
            EditModelCsvId = obj.edit_model_csv_id;
            EditModelCsvGuid = obj.edit_model_csv_guid;
            Id = obj.edit_model_csv_id;
            Guid = obj.edit_model_csv_guid;
            ProjectGuid = obj.project_guid;
            Asofdate = obj.asofdate;
            Type = obj.type;
            Title = obj.title;
            Comment = obj.comment;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
