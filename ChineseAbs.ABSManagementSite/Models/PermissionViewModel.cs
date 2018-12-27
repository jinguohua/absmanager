using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class PermissionTree
    {
        public PermissionTree()
        {
            children = new List<PermissionTree>();
        }

        public string key { set; get; }

        public string title { set; get; }

        public string type { set; get; }

        public List<PermissionTree> children { set; get; }
    }

    public class UserPermissionInfo
    {
        public string UserName { set; get; }

        public string RealName { set; get; }

        public string Permission { set; get; }

        public string UniqueIdentifier { set; get; }
    }
}