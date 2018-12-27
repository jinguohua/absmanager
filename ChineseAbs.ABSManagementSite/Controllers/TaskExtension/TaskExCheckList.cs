using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Models;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public class TaskExCheckList : TaskExBase
    {
        public TaskExCheckList(string userName, string shortCode)
            : base(userName, shortCode)
        {
            this.OnFinishing += TaskExCheckList_OnFinishing;
        }

        HandleResult TaskExCheckList_OnFinishing()
        {
            if (!string.IsNullOrEmpty(Task.TaskExtension.TaskExtensionInfo))
            {
                var taskExCheckLists = CommUtils.FromJson<TaskExCheckListInfo>(Task.TaskExtension.TaskExtensionInfo);
                var isAllChecked = taskExCheckLists.CheckGroups.All(x => x.CheckItems.All(item => item.CheckStatus == TaskExCheckType.Checked.ToString()));
                if (!isAllChecked)
                {
                    return new HandleResult(EventResult.Cancel, "当前工作有检查项未核对，无法完成工作");
                }
            }

            return new HandleResult();
        }

        public override object GetEntity()
        {
            if (string.IsNullOrEmpty(Task.TaskExtension.TaskExtensionInfo))
            {
                return null;
            }

            var viewModel = new TaskExDocumentViewModel();
            viewModel.TaskExCheckLists = CommUtils.FromJson<TaskExCheckListInfo>(Task.TaskExtension.TaskExtensionInfo);

            return viewModel;
        }
    }
}