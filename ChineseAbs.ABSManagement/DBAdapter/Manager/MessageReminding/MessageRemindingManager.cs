using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MessageRemindingModel = ChineseAbs.ABSManagement.Models.MessageReminding;

namespace ChineseAbs.ABSManagement.Manager.MessageReminding
{
    public class MessageRemindingManager : BaseManager
    {

        public MessageRemindingManager()
        {
            m_defaultTableName = "dbo.MessageReminding";
            m_defaultPrimaryKey = "message_reminding_id";
            m_defaultHasRecordStatusField = true;
        }



        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public MessageRemindingModel New(string uid, string userid,
              string remark, DateTime remindTime, MessageUidType msgType)
        {
            var now = DateTime.Now;

            var messageReminding = new MessageRemindingModel();
            messageReminding.Guid = Guid.NewGuid().ToString();

            messageReminding.Uid = uid;
            messageReminding.UserName = userid;
            messageReminding.Type = msgType;
            messageReminding.Remark = remark;
            messageReminding.RemindTime = remindTime;
            messageReminding.MessageStatus = MessageStatusEnum.UnSend;
            messageReminding.CreateTime = now;
            messageReminding.CreateUserName = UserInfo.UserName;
            messageReminding.LastModifyTime = now;
            messageReminding.LastModifyUserName = UserInfo.UserName;
            messageReminding.RecordStatus = RecordStatus.Valid;
            messageReminding.Id = Insert(messageReminding.GetTableObject());

            return messageReminding;
        }

        public MessageRemindingModel Update(MessageRemindingModel messageReminding)
        {
            messageReminding.LastModifyTime = DateTime.Now;
            messageReminding.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, messageReminding.GetTableObject());
            return messageReminding;
        }



        public MessageRemindingModel GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableMessageReminding>("message_reminding_guid", guid);
            return new MessageRemindingModel(record);
        }

        public void RemovebyUid(string uid)
        {
            GetByUid(uid).ForEach(x =>
            {
                var messageRemind = GetByGuid(x.Guid);
                Remove(messageRemind);
            });

        }

        public List<MessageRemindingModel> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TableMessageReminding, string>("message_reminding_guid", guids);
            return records.ToList().ConvertAll(x => new MessageRemindingModel(x));
        }

        public List<MessageRemindingModel> GetByUid(string uid)
        {
            var records = Select<ABSMgrConn.TableMessageReminding>("uid", uid);
            return records.ToList().ConvertAll(x => new MessageRemindingModel(x));
        }

        public int Remove(MessageRemindingModel messageReminding)
        {
            messageReminding.RecordStatus = RecordStatus.Deleted;
            messageReminding.LastModifyTime = DateTime.Now;
            messageReminding.LastModifyUserName = UserInfo.UserName;
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, messageReminding.GetTableObject());
        }

        public MessageRemindingModel GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableMessageReminding>(id);
            return new MessageRemindingModel(record);
        }


        public object GetResultByUid(string uid)
        {
            var messageRemindList = GetByUid(uid);
            object result;
            if (messageRemindList.Count() > 0)
            {
                var usernames = messageRemindList.Select(x => x.UserName).Distinct().ToArray();
                var users = UserService.GetUsers(usernames);
                List<MessageUser> userList = new List<MessageUser>(); ;
                usernames.ToList().ForEach(x =>
                {
                    userList.Add(new MessageUser
                    {
                        RealName = users.First(o=> o.UserName.Equals(x, StringComparison.CurrentCultureIgnoreCase)).Name,
                        Userid = x
                    });
                });

                var messageRemind = messageRemindList.FirstOrDefault();
                result = new
                {
                    Uid = messageRemind.Uid,
                    UserList = userList,
                    Type = messageRemind.Type.ToString(),
                    Remindtime = messageRemind.RemindTime.ToString("yyyy-MM-dd HH:mm"),
                    Remark = messageRemind.Remark,
                    Messagestatus = Enum.GetName(typeof(MessageStatusEnum), messageRemind.MessageStatus),
                    Exist = true
                };
            }
            else
            {
                result = new
                {
                    Exist = false
                };
            }
            return result;
        }

    }
    public class MessageUser
    {
        public string RealName { get; set; }
        public string Userid { get; set; }

    }
}
