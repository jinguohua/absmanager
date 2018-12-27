using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class PermissionManagerTest
    {
        [TestMethod]
        public void PermissionNewRecord()
        {
            var mgr = new PermissionManager(new Models.UserInfo("cgzhou"));
            Permission permission = new Permission();
            permission.UserName = "cgzhou";
            permission.ObjectUniqueIdentifier = "this-is-the-guid-or-short-code";
            permission.Type = PermissionType.Read;
            permission.ObjectType = PermissionObjectType.None;
            mgr.NewPermission(permission);

            var allPermission = mgr.GetAllPermission(permission.UserName, permission.ObjectUniqueIdentifier);
            Assert.AreEqual(allPermission.Count(x => x.Type == PermissionType.Read), 1);

            bool hasReadPermission = mgr.HasPermission(permission.UserName, permission.ObjectUniqueIdentifier, PermissionType.Read);
            Assert.IsTrue(hasReadPermission);
            bool hasWritePermission = mgr.HasPermission(permission.UserName, permission.ObjectUniqueIdentifier, PermissionType.Write);
            Assert.IsFalse(hasWritePermission);
        }
    }
}
