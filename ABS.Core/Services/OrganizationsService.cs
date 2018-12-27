using ABS.Core.Models;
using ABS.Infrastructure;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Services
{
    public class OrganizationsService : ServiceBase
    {
        public IRepository<Organization, int> OrganizationRespository { get; set; }

        readonly string OrganizationCacheKey = "OrganizationCachekey";

        static object _sync = new object();

        public List<OrganizationViewModel> Organizations
        {
            get
            {
                return CacheHelper.Get<List<OrganizationViewModel>>(OrganizationCacheKey, LoadOrganizations);
            }
        }


        public OrganizationsService(IUnitOfWork unitofwork, IRepository<Organization, int> organizationRespository) :
            base(unitofwork)
        {
            OrganizationRespository = organizationRespository;
        }

        public void Add(OrganizationViewModel org)
        {
            Organization o = AutoMapper.Mapper.Map<Organization>(org);
            if (String.IsNullOrEmpty(o.Name))
            {
                throw new Exception("Name is empty");
            }

            if (o.ParentID.HasValue && Organizations.FirstOrDefault(m => m.Id == o.ParentID.Value) == null)
            {
                throw new Exception("can't find the parent node by id: " + o.ParentID.Value);
            }

            OrganizationRespository.Insert(o);

            CleanCache();

        }

        public void CleanCache()
        {
            CacheHelper.Remove(OrganizationCacheKey);
        }

        public void Delete(int id)
        {
            UnitOfWork.TransactionEnabled = true;
            // clear OrganizationUser table
            var model = OrganizationRespository.Entities.First(o => o.Id == id);
            model.Members.Clear();
            OrganizationRespository.Update(model);

            int records = OrganizationRespository.Delete(r => r.ParentID.Value.Equals(id));
            records = OrganizationRespository.Delete(id);
            UnitOfWork.SaveChanges();

            CleanCache();
        }

        public void Edit(OrganizationViewModel viewModel)
        {
            if (Organizations.FirstOrDefault(o => o.Id == viewModel.Id) == null)
            {
                throw new Exception("can't load by id: " + viewModel.Id.ToString());
            }

            var model = OrganizationRespository.Entities.First(o => o.Id == viewModel.Id);

            //viewModel中Parent为null,所以model的ParentID不能被赋值
            //if (viewModel.ParentID != model.ParentID)
            //{
            //    AutoMapper.Mapper.Map(viewModel, model);
            //    model.Parent = null;
            //}
            //else
            //{
            //    AutoMapper.Mapper.Map(viewModel, model);
            //}

            model.Name = viewModel.Name;
            var isUpdate = OrganizationRespository.Update(model) > 0;
            if (isUpdate)
            {
                CleanCache();
                LoadOrganizations();
            }
        }

        public List<OrganizationViewModel> LoadOrganizations()
        {
            List<Organization> data = OrganizationRespository.GetInclude(o => o.Parent).ToList();
            List<OrganizationViewModel> vm = AutoMapper.Mapper.Map<List<OrganizationViewModel>>(data);

            return vm;
        }

        public OrganizationViewModel GetById(int id)
        {
            return Organizations.FirstOrDefault(o => o.Id == id);
        }

        public OrganizationViewModel GetByIdentityKey(string key)
        {
            key = key ?? "";
            return Organizations.FirstOrDefault(o => key.Equals(o.IdentityKey, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<OrganizationViewModel> GetChildrenByParentId(int id)
        {
            return Organizations.Where(o => o.ParentID == id).ToList();
        }

        public List<OrganizationViewModel> GetChildrenByKey(string key)
        {
            OrganizationViewModel record = GetByIdentityKey(key);
            if (record != null)
            {
                return Organizations.Where(o => o.ParentID == record.Id).ToList();
            }
            else
                return new List<OrganizationViewModel>();
        }

        public List<AppUser> GetMemberByOrganiationId(int organizationId)
        {
            Organization org = OrganizationRespository.Entities
                .Where(o => o.Id == organizationId).SingleOrDefault();
            if (org == null)
            {
                throw new Exception("当前组织信息错误");
            }

            return org.Members.ToList();
        }

        public void AddMemberForOrganiations(int organizationId, List<AppUser> users)
        {
            Organization org = OrganizationRespository.Entities
                .Where(o => o.Id == organizationId).SingleOrDefault();
            if (org == null)
            {
                throw new Exception("当前组织信息错误");
            }
            users.AddRange(org.Members.ToList());
            org.Members = users;

            OrganizationRespository.Update(org);
        }

        public void RemoeMemberForOrganiations(int organizationId, AppUser user)
        {
            Organization org = OrganizationRespository.Entities
                .Where(o => o.Id == organizationId).SingleOrDefault();
            if (org == null)
            {
                throw new Exception("当前组织信息错误");
            }
            org.Members.Remove(user);

            OrganizationRespository.Update(org);
        }
    }
}
