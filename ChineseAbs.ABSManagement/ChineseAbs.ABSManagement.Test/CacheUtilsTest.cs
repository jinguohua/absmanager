using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class CacheUtilsTest
    {
        [TestMethod]
        public void CacheTest()
        {
            var taskManager = new TaskManager(new Models.UserInfo("cgzhou"));
            var taskCache = CacheUtils.Build<string, Models.Task>(x => taskManager.GetTask(x));
            Assert.AreEqual(taskCache.Count, 0);
            var task1 = taskCache["EYCWB9"];
            Assert.AreEqual(taskCache.Count, 1);
            var task2 = taskCache["Q2Z82X"];
            Assert.AreEqual(taskCache.Count, 2);

            var taskCache2 = CacheUtils.Build<string, Models.Task>(
                x =>
                {
                    var task = taskManager.GetTask(x);
                    return taskManager.GetTasksByProjectId(task.ProjectId);
                },
                y => y.ShortCode
            );
            Assert.AreEqual(taskCache2.Count, 0);

            var taskCount3 = taskManager.GetTasksByProjectId(taskManager.GetTask("EYCWB9").ProjectId).Count;
            var task3 = taskCache2["EYCWB9"];
            Assert.AreEqual(taskCache2.Count, taskCount3);

            taskCount3 += taskManager.GetTasksByProjectId(taskManager.GetTask("Q2Z82X").ProjectId).Count;
            var task4 = taskCache2["Q2Z82X"];
            Assert.AreEqual(taskCache2.Count, taskCount3);
        }
    }
}
