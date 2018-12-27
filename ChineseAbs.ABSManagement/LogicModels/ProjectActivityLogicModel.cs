using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class ProjectActivityLogicModel : BaseLogicModel
    {
        public ProjectActivityLogicModel(ProjectLogicModel project)
            : base(project)
        {
        }

        public void Add(int projectId, ActivityObjectType activityObjectType, string activityObjectUniqueIdentifier, string comment)
        {
            CommUtils.AssertHasContent(comment, "Project activity's comment must has content.");
            CommUtils.AssertHasContent(activityObjectUniqueIdentifier, "Project activity's activityObjectUniqueIdentifier must has content.");

            m_dbAdapter.ProjectActivity.NewProjectActivity(projectId, activityObjectType, activityObjectUniqueIdentifier, comment);

        }

        public List<ProjectActivity> Get(int topActivityCount)
        {
            var projectId = ProjectLogicModel.Instance.ProjectId;
            var maxTopActivityCount = 100;
            CommUtils.Assert(maxTopActivityCount <= 100, "Project activity's topActivityCount > {0}.", maxTopActivityCount);
            //按时间倒序，取出topActivityCount条 项目动态

            var result = m_dbAdapter.ProjectActivity.GetProjectActivities(projectId, topActivityCount);

            return result;
            //return new List<ProjectActivity>();
        }
    }
}
