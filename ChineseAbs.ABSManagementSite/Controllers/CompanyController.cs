using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABS.Core.Models;
using ABS.Core.Services;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Controllers;
using SAFS.Utility.Web;

namespace ABS.ABSManagementSite.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly string _companyType="CompanyTypes";

        // GET: Company
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetCompanyList()
        {
            var data = CompanyService.Companies.ToList();
            data.ForEach(o => o.CategoryType = GetDispalyName(o.Category));
            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = data });
        }
        public string GetDispalyName(string[] categories)
        {
            List<string> lst = new List<string>();
            foreach (var item in categories)
            {
                lst.Add(BaseCodeService.GetDispalyName(_companyType, item));
            }
            return string.Join(",", lst);

        }
        public JsonResult AddCompany(CompanyViewModel vmodel)
        {
            CompanyService.AddCompany(vmodel);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }
        public JsonResult UpdateCompany(CompanyViewModel vmodel)
        {
            CompanyService.UpdateCompany(vmodel);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }
        public JsonResult DeleteCompany(int id)
        {
            CommUtils.Assert(id != 0, "数据错误,不能删除");
            CompanyService.DeleteCompany(id);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }

        public JsonResult GetBaseCategory(string category)
        {
            var list = BaseCodeService.GetItemsByCategory(category);
            var data = list.Select(o => new { id = o.Key, text = o.Value }).ToList();
            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = data });
        }
    }
}