using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Helpers
{
    public class SerializeHelper
    {

        public static List<SelectListItem> GetSelectList(List<Project> list)
        {
            List<SelectListItem> slist = new List<SelectListItem>();
            if (list != null && list.Count != 0)
            {
                slist.Add(new SelectListItem() { Value = "", Text = "-请选择-" });
                string value = string.Empty;
                string text = string.Empty;
                foreach (Project p in list)
                {
                    text = p.Name;
                    value = p.ProjectId.ToString();
                    slist.Add(new SelectListItem() { Value = value, Text = text});
                }
            }
            return slist;
        }

        public static List<SelectListItem> GetAuthedAccountSelectList(List<AuthedAccount> list)
        {
            List<SelectListItem> slist = new List<SelectListItem>();
            if (list != null && list.Count != 0)
            {
                slist.Add(new SelectListItem() { Value = "", Text = "-请选择-" });
                string value = string.Empty;
                string text = string.Empty;
                foreach (AuthedAccount aa in list)
                {
                    value = aa.UserName;
                    text = aa.RealName;
                    slist.Add(new SelectListItem() { Value = value, Text = text });
                }
            }
            return slist;
        }
    }
}