using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ABS.Core.Models;
using ABS.Core.Services;
using ChineseAbs.ABSManagement.Utils;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class CompanyController : BaseApiController
    {
        private readonly string _companyType = "CompanyTypes";

        public BaseCodeService BaseCodeService { get; set; }
        [HttpGet]
        public List<CompanyViewModel> List()
        {
            var data = CompanyService.Companies.ToList();
            data.ForEach(o => o.CategoryType = GetDispalyName(o.Category));

            return data;
        }
        private string GetDispalyName(string[] categories)
        {
            List<string> lst = new List<string>();
            foreach (var item in categories)
            {
                lst.Add(BaseCodeService.GetDispalyName(_companyType, item));
            }
            return string.Join(",", lst);

        }

        [HttpPost]
        public void Add(CompanyViewModel vmodel)
        {
            CompanyService.AddCompany(vmodel);
        }

        [HttpPost]
        public void Update(CompanyViewModel vmodel)
        {
            CompanyService.UpdateCompany(vmodel);
        }

        [HttpPost]
        public void Delete(int id)
        {
            CommUtils.Assert(id != 0, "数据错误,不能删除");
            CompanyService.DeleteCompany(id);
        }

    }
}
