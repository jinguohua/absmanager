using ABS.Core.Models;
using ABS.Infrastructure;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Services
{
    public class CompanyService : ServiceBase
    {
        public IRepository<Company, int> CompanyRepository { get; set; }

        readonly string companyCacheKey = "CompanyCachekey";

        static object _sync = new object();

        public List<CompanyViewModel> Companies
        {
            get
            {
                return CacheHelper.Get<List<CompanyViewModel>>(companyCacheKey, GetCompanyViewModels);
            }
        }

        public CompanyService(IUnitOfWork unitofwork)
                : base(unitofwork)
        { }

        private List<CompanyViewModel> GetCompanyViewModels()
        {
            var models = CompanyRepository.NoTrackingEntities.ToList();
            var viewModels = AutoMapper.Mapper.Map<List<Company>, List<CompanyViewModel>>(models);
            return viewModels;
        }

        public CompanyViewModel GetCompanyViewModel(int id)
        {
            return Companies.FirstOrDefault(o => o.Id == id);
        }

        public CompanyViewModel GetCompanyByCode(string shortName)
        {
            return Companies.FirstOrDefault(o => o.ShortName.Equals(shortName, StringComparison.CurrentCultureIgnoreCase));
        }

        public string GetCompanyShortNames(params int[] companyIDs)
        {
            if (companyIDs == null || companyIDs.Count() == 0)
                return "";
            return String.Join("/", Companies.Where(o => companyIDs.Contains(o.Id)).Select(o => o.ShortName));
        }

        public string GetCompanyNames(params int[] companyIDs)
        {
            if (companyIDs == null || companyIDs.Count() == 0)
                return "";
            return String.Join("/", Companies.Where(o => companyIDs.Contains(o.Id)).Select(o => o.Name));
        }

        public void ClearCache()
        {
            lock (_sync)
            {
                CacheHelper.Remove(companyCacheKey);
            }
        }


        public CompanyViewModel DefaultCompany
        {
            get
            {
                string code = ConfigurationManager.AppSettings["SystemCompany"];
                return GetCompanyByCode(code);
            }
        }

        public void AddCompany(CompanyViewModel vmodel)
        {
            var model = AutoMapper.Mapper.Map<CompanyViewModel, Company>(vmodel);
            vmodel.IsActived = true;
            CompanyRepository.Insert(model);
            ClearCache();
        }

        public void UpdateCompany(CompanyViewModel vmodel)
        {
            var company = CompanyRepository.Entities.Where(o => o.Id == vmodel.Id).SingleOrDefault();

            var model  = AutoMapper.Mapper.Map<CompanyViewModel, Company>(vmodel);
            
            model.Creator = company.Creator;
            model.CreatedTime = company.CreatedTime;
            
            CompanyRepository.Update(model);

            ClearCache();
        }

        public void DeleteCompany(int id)
        {
            var company = CompanyRepository.Entities.Where(o => o.Id == id).SingleOrDefault();

            CompanyRepository.Delete(company);
            ClearCache();
        }
    }
}
