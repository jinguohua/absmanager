using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class MessageRemindingController : BaseController
    {
        [HttpPost]
        public ActionResult CreateMessageReminding(string uid, string userid, string remark, DateTime remindTime, string type)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(userid, "提醒人员不能为空");
                CommUtils.AssertHasContent(remindTime.ToString(), "提醒时间不能为空");
                CommUtils.Assert(remindTime > DateTime.Now, "提醒时间要大于现在时间");
                var msgType = CommUtils.ParseEnum<MessageUidType>(type);
                CheckRemindingPermission(uid, msgType);
                userid.Split(',').ToList().ForEach(x =>
                {
                    if (x != "")
                    {
                        m_dbAdapter.MessageReminding.New(uid, x, remark, remindTime, msgType);
                    }
                });
                return ActionUtils.Success(1);
            });
        }

        private void CheckRemindingPermission(string uid,MessageUidType msgType) {
            if (msgType == MessageUidType.Task)
            {
                CommUtils.Assert(m_dbAdapter.Permission.GetByObjectUid(uid).Where(x=>x.UserName==CurrentUserName).ToList().Exists(x => x.Type == PermissionType.Write || x.Type == PermissionType.Execute), "没有{0}的‘写’、‘执行’权限，无法创建或修改提醒", msgType.ToString());
            }
        }

        [HttpPost]
        public ActionResult ModifyMessageReminding(string uid, string userid, string remark, DateTime remindTime, string type)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(userid, "提醒人员不能为空");
                CommUtils.AssertHasContent(remindTime.ToString(), "提醒时间不能为空");
                CommUtils.Assert(remindTime > DateTime.Now, "提醒时间要大于现在时间");
                var msgType = CommUtils.ParseEnum<MessageUidType>(type);
                CheckRemindingPermission(uid, msgType);

                var messageRemindList = m_dbAdapter.MessageReminding.GetByUid(uid);
                string[] useridArr = userid.Split(',');

                var deleteList = messageRemindList.Where(x => !useridArr.Contains(x.UserName));
                deleteList.ToList().ForEach(x =>
                {
                    m_dbAdapter.MessageReminding.Remove(x);
                });

                var addList = useridArr.ToList().Where(x => x != "" && !messageRemindList.Select(t => t.UserName).Contains(x));
                addList.ToList().ForEach(x =>
                {
                    m_dbAdapter.MessageReminding.New(uid, x, remark, remindTime, msgType);
                });

                var modifyList = messageRemindList.Except(deleteList);
                modifyList.ToList().ForEach(x =>
                {
                    var messageRemind = m_dbAdapter.MessageReminding.GetByGuid(x.Guid);
                    messageRemind.UserName = x.UserName;
                    messageRemind.Remark = remark;
                    messageRemind.RemindTime = remindTime;
                    messageRemind.MessageStatus = MessageStatusEnum.UnSend;
                    m_dbAdapter.MessageReminding.Update(messageRemind);
                });

                return ActionUtils.Success(1);
            });
        }


        [HttpPost]
        public ActionResult DeleteMessageReminding(string uid)
        {
            return ActionUtils.Json(() =>
            {
                m_dbAdapter.MessageReminding.RemovebyUid(uid);
                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult GetMessageReminding(string uid)
        {
            return ActionUtils.Json(() =>
            {
                return ActionUtils.Success(m_dbAdapter.MessageReminding.GetResultByUid(uid));
            });
        }

    }
}

