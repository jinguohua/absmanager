using System.Reflection;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public static class TaskExFactory
    {
        public static TaskExBase CreateInstance(string taskExType, string shortCode, string userName)
        {
            if (!string.IsNullOrEmpty(taskExType))
            {
                var typeName = "ChineseAbs.ABSManagementSite.Controllers.TaskExtension.TaskEx" + taskExType;
                var param = new object[] { userName, shortCode };
                var instance = Assembly.GetExecutingAssembly().CreateInstance(typeName, false, BindingFlags.Default, null, param, null, null);
                return (TaskExBase)instance;
            }

            return null;
        }
    }
}