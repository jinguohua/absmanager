using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class MessageReminding : BaseDataContainer<TableMessageReminding>
    {
        public MessageReminding()
        {
        }

        public MessageReminding(TableMessageReminding messageReminding)
            : base(messageReminding)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public string Uid { get; set; }

        public string UserName { get; set; }

        public DateTime RemindTime { get; set; }

        public string Remark { get; set; }

        public MessageUidType Type { get; set; }

        public MessageStatusEnum MessageStatus { get; set; }

        public DateTime? MessageTime { get; set; }

        public string MessageContent { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableMessageReminding GetTableObject()
        {
            var obj = new TableMessageReminding();
            obj.message_reminding_id = Id;
            obj.message_reminding_guid = Guid;
            obj.uid = Uid;
            obj.userid = UserName;
            obj.remind_time = RemindTime;
            obj.remark = Remark;
            obj.type = (int)Type;
            obj.message_status = (int)MessageStatus;
            obj.message_time = MessageTime;
            obj.message_content = MessageContent;

            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableMessageReminding obj)
        {
            Id = obj.message_reminding_id;
            Guid = obj.message_reminding_guid;

            Uid = obj.uid;
            UserName = obj.userid;
            RemindTime = obj.remind_time;
            Remark = obj.remark;
            Type =(MessageUidType)obj.type;
            MessageStatus = (MessageStatusEnum)obj.message_status;
            MessageTime = obj.message_time;
            MessageContent = obj.message_content;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }

    public enum MessageUidType
    {
        Undefind = 0,
        Task = 1,
        Investment = 2
    }

    public enum MessageStatusEnum
    {
        UnSend = 0,
        SendOk=1,
        SendFail = 2
    }


}
