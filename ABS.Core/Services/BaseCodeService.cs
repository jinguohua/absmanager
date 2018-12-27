using ABS.Core.Models;
using ABS.Infrastructure;
using Newtonsoft.Json;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Services
{
    public class BaseCodeService : ServiceBase
    {
        public IRepository<CodeCategory, int> CodeCategoryRepository { get; set; }
        public IRepository<CodeItem, int>  CodeItemRepository { get; set; }

        static object _sync = new object();

        public BaseCodeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        readonly string cacheKey = "CodeCategorys";

        private List<CodeCategoryViewModel> LoadAllDataToCache()
        {
            var categories = CodeCategoryRepository.GetInclude(o => o.Items).ToList();
            var categoryViewModels = AutoMapper.Mapper.Map<List<CodeCategory>, List<CodeCategoryViewModel>>(categories);

            return categoryViewModels;
        }

        private void CleanCache()
        {
            CacheHelper.Remove(cacheKey);
        }

        protected List<CodeCategoryViewModel> Categories
        {
            get
            {
                return CacheHelper.Get<List<CodeCategoryViewModel>>(cacheKey, LoadAllDataToCache);
            }
        }

        public int DeleteCodeCategory(int id)
        {
            UnitOfWork.TransactionEnabled = true;
            int count = CodeCategoryRepository.Delete(id);
            CodeItemRepository.Delete(c => c.CategoryID.Equals(id));
            UnitOfWork.SaveChanges();
            UnitOfWork.TransactionEnabled = false;
            CleanCache();
            return count;
        }

        public int DeleteCodeItem(int id)
        {
            int count = CodeItemRepository.Delete(id);
            CleanCache();
            return count;
        }

        public CodeCategoryViewModel GetCodeCategoryViewModel(int id)
        {
            return Categories.Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public List<CodeCategoryViewModel> GetCategories()
        {
            return Categories.ToList();
        }

        public List<CodeItemViewModel> GetItemsByCategory(string category)
        {
            var categoryData = Categories.FirstOrDefault(o => o.Code.Equals(category, StringComparison.CurrentCulture));
            if (categoryData != null)
                return categoryData.Items.ToList();
            else
                return new List<CodeItemViewModel>();
        }

        public CodeItemViewModel GetCodeItemViewModel(int id)
        {
            var model = CodeItemRepository.GetByKey(id);
            var viewModel = AutoMapper.Mapper.Map<CodeItem, CodeItemViewModel>(model);
            return viewModel;
        }

        public string GetDispalyName(string catagory, string key)
        {
            var item = Categories.FirstOrDefault(o => o.Code == catagory).Items.FirstOrDefault(o => o.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));
            return item.Value;
        }

        public void SaveCodeCategory(CodeCategoryViewModel viewModel)
        {
            if (viewModel.Id == 0)
            {
                var model = AutoMapper.Mapper.Map<CodeCategoryViewModel, CodeCategory>(viewModel);
                CodeCategoryRepository.Insert(model);
            }
            else
            {
                var model = CodeCategoryRepository.GetByKey(viewModel.Id);
                AutoMapper.Mapper.Map<CodeCategoryViewModel, CodeCategory>(viewModel, model);
                CodeCategoryRepository.Update(model);
            }
            CleanCache();
        }
        public void SaveCodeItem(CodeItemViewModel viewModel)
        {
            if (viewModel.Id == 0)
            {
                var model = AutoMapper.Mapper.Map<CodeItemViewModel, CodeItem>(viewModel);
                CodeItemRepository.Insert(model);
            }
            else
            {
                var model = CodeItemRepository.GetByKey(viewModel.Id);
                AutoMapper.Mapper.Map<CodeItemViewModel, CodeItem>(viewModel, model);
                CodeItemRepository.Update(model);
            }
            CleanCache();
        }
    }
}
