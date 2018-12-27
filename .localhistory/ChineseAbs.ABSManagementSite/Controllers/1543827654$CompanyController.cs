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

        // GET: Company
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetCompanyList()
        {
            var data = CompanyService.Companies.ToList();
            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = data });
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
            CommUtils.Assert(id!=0, "数据错误,不能删除");
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