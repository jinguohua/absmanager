using ABS.Core.DTO;
using ABS.Core.Models;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ABS.Core.Services
{
    public class ProjectAccountService : ServiceBase
    {
        public IRepository<ProjectAccount, int> ProjectAccountRepository { get; set; }
        public IRepository<Account, int> AccountRepository { get; set; }

        static object _sync = new object();

        public ProjectAccountService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }

        #region Cache
        private void ClearCache()
        {
            lock (_sync)
            {
                CacheHelper.Remove(CacheCenter.ProjectCacheKey);
                CacheHelper.Remove(CacheCenter.ProjectAccountCachekey);
                CacheHelper.Remove(CacheCenter.AccountCachekey);
            }
        }

        private List<AccountViewModel> AccountCache
        {
            get
            {
                return CacheHelper.Get(CacheCenter.AccountCachekey, GetAccountViewModels); ;
            }
        }

        private List<AccountViewModel> GetAccountViewModels()
        {
            var models = AccountRepository.Entities.ToList();
            var viewModels = AutoMapper.Mapper.Map<List<Account>, List<AccountViewModel>>(models);
            return viewModels;
        }

        private List<ProjectAccountViewModel> ProjectAccountCache
        {
            get
            {
                return CacheHelper.Get<List<ProjectAccountViewModel>>(CacheCenter.ProjectAccountCachekey, GetProjectAccountViewModels);
            }
        }

        private List<ProjectAccountViewModel> GetProjectAccountViewModels()
        {
            var models = ProjectAccountRepository.Entities.ToList();
            var viewModels = AutoMapper.Mapper.Map<List<ProjectAccount>, List<ProjectAccountViewModel>>(models);
            return viewModels;
        }
        #endregion

        public List<ProjectAccountViewModel> GetProjectAccountsByProjectId(int projectId)
        {
            var viewModels = ProjectAccountCache.Where(o => o.ProjectID == projectId).ToList();
            viewModels.ForEach(o =>
            {
                if (o.Account == null)
                {
                    o.Account = AccountCache.FirstOrDefault(a => a.Id == o.AccountID);
                }
            });
            return viewModels;
        }

        public bool Save(ProjectAccountViewModel viewModel)
        {
            var isSucceed = false;
            if (viewModel.Account == null)
                throw new Exception("Account对象为空，无法保存");
            var accountModel = viewModel.Account;
            if (viewModel.Id > 0) // update
            {
                var productAccount = ProjectAccountRepository.Entities.FirstOrDefault(o => o.Id == viewModel.Id);
                if (productAccount == null)
                    throw new Exception($"不存在Id为：{viewModel.Id}的ProjectAccount");
                var oldEntity = AccountRepository.Entities.FirstOrDefault(o => o.Id == productAccount.AccountID);
                var entity = AutoMapper.Mapper.Map<AccountViewModel, Account>(accountModel);
                entity.CreatedTime = oldEntity.CreatedTime;
                entity.CouponID = oldEntity.CouponID;
                entity.Id = oldEntity.Id;
                isSucceed = AccountRepository.Update(entity) > 0;
            }
            else // add
            {
                var entity = AutoMapper.Mapper.Map<AccountViewModel, Account>(accountModel);
                entity.CreatedTime = DateTime.Now;
                if (entity.CouponID <= 0)
                    entity.CouponID = null;
                try
                {
                    AccountRepository.UnitOfWork.TransactionEnabled = true;

                    AccountRepository.Insert(entity);
                    var projectAccount = new ProjectAccount
                    {
                        ProjectID = viewModel.ProjectID,
                        AccountID = entity.Id,
                    };
                    ProjectAccountRepository.Insert(projectAccount);

                    AccountRepository.UnitOfWork.SaveChanges();
                    AccountRepository.UnitOfWork.TransactionEnabled = false;

                    isSucceed = true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            ClearCache();

            return isSucceed;
        }

        public bool Delete(int id)
        {
            var isSucceed = false;
            try
            {
                ProjectAccountRepository.UnitOfWork.TransactionEnabled = true;
                var projectAccount = ProjectAccountRepository.Entities.FirstOrDefault(o => o.Id == id);
                if (projectAccount != null)
                {
                    var accountId = projectAccount.AccountID;
                    ProjectAccountRepository.Delete(projectAccount);
                    AccountRepository.Delete(accountId);
                }
                ProjectAccountRepository.UnitOfWork.SaveChanges();
                ProjectAccountRepository.UnitOfWork.TransactionEnabled = false;
            }
            catch (Exception e)
            {
                throw e;
            }
            isSucceed = true;

            return isSucceed;
        }

    }
}
