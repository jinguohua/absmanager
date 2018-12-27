using ABS.Core.DTO;
using ABS.Core.Models;
using ABS.Infrastructure;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ABS.Core.Services
{

    public class ProjectService : ServiceBase
    {
        public IRepository<Project, int> ProjctRepository { get; set; }
        public IRepository<ProjectCompany, int> ProjectCompanyRepository { get; set; }

        private CompanyService _CompanyService { get; set; }

       // readonly string projectCacheKey = "ProjectCachekey";

        static object _sync = new object();

        public List<ProjectViewModel> ProjectViewModels
        {
            get
            {
                return CacheHelper.Get<List<ProjectViewModel>>(CacheCenter.ProjectCacheKey, GetProductViewModels);
            }
        }

        public ProjectService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }

        #region Cache 
        public void ClearCache()
        {
            lock (_sync)
            {
                CacheHelper.Remove(CacheCenter.ProjectCacheKey);
            }
        }

        private List<ProjectViewModel> GetProductViewModels()
        {
            var data = ProjctRepository.Entities.ToList();
            var list = AutoMapper.Mapper.Map<List<Project>, List<ProjectViewModel>>(data);

            return list;
        }
        #endregion

        #region Project
        public int AddProject(ProjectViewModel viewModel)
        {
            var entity = AutoMapper.Mapper.Map<ProjectViewModel, Project>(viewModel);
            entity.IdentifyCode = Guid.NewGuid().ToString();
            var id = ProjctRepository.Insert(entity);
            ClearCache();
            return id;
        }

        public bool UpdateProject(ProjectViewModel viewModel)
        {
            var id = viewModel.Id;
            var oldModel = ProjctRepository.Entities.FirstOrDefault(o => o.Id == id);
            var oldCompanyEntities = oldModel.Companies;

            var projectEntity = AutoMapper.Mapper.Map<ProjectViewModel, Project>(viewModel);
            projectEntity.IdentifyCode = oldModel.IdentifyCode;
            projectEntity.CreatedTime = oldModel.CreatedTime;
            projectEntity.Creator = oldModel.Creator;

            #region 新增或更新机构信息
            var companyies = projectEntity.Companies.ToList();
            companyies.ForEach(p =>
            {
                if(p.Id > 0)
                {
                    var pEntity = ProjectCompanyRepository.Entities.FirstOrDefault(o => o.Id == p.Id);
                    if(pEntity != null)
                    {
                        pEntity.CompanyRole = p.CompanyRole;
                        pEntity.CompanyID = p.CompanyID;
                        pEntity.ProjectID = projectEntity.Id;
                        ProjectCompanyRepository.Update(pEntity);
                    }
                }
                else
                {
                    p.ProjectID = projectEntity.Id;
                    p.CreatedTime = DateTime.Now;
                    ProjectCompanyRepository.Insert(p);
                }   
            });
            #endregion

            var isSucceed = false;
            if (projectEntity != null)
            {
                isSucceed = ProjctRepository.Update(projectEntity) > 0;
                if (isSucceed)
                    ClearCache();
            }
            return isSucceed;
        }

        public ProjectViewModel GetProjectById(int id)
        {
            var model = ProjectViewModels.FirstOrDefault(o => o.Id == id);
            if (model == null)
            {
                var project = ProjctRepository.Entities.Where(o => o.Id == id).FirstOrDefault();
                model = AutoMapper.Mapper.Map<Project, ProjectViewModel>(project);
            }
            return model;
        }

        public PageListData<List<ProjectViewModel>> GetProjectsByPage(QueryPageModel model)
        {
            var orderField = model.SortField;
            var orderType = model.SortType;
            var page = model.Page;
            var pageSize = model.PageSize;
            IQueryable<ProjectViewModel> query = ProjectViewModels.AsQueryable();

            var filters = model.Filters;
            if(null != filters)
            {
                var filtersList = filters.Where(o => !string.IsNullOrEmpty(o.Value)).ToList();
                filtersList.ForEach(f =>
                {
                    if (string.Equals(f.Key, "Name", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(o => o.Name.Contains(f.Value));
                    }
                    if (string.Equals(f.Key, "ProjectType", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(o => o.ProjectType == f.Value);
                    }
                    if (string.Equals(f.Key, "Status", StringComparison.OrdinalIgnoreCase))
                    {
                        EProjectStatus projectStatus;
                        var isValid = Enum.TryParse(f.Value, out projectStatus);
                        if (isValid)
                        {
                            query = query.Where(o => o.Status == projectStatus);
                        }
                    }
                    if (string.Equals(f.Key, "startDate", StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime dateTime;
                        var isValid = DateTime.TryParse(f.Value, out dateTime);
                        if (isValid)
                        {
                            query = query.Where(o => o.IssueDate >= dateTime);
                        }
                    }
                    if (string.Equals(f.Key, "endDate", StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime dateTime;
                        var isValid = DateTime.TryParse(f.Value, out dateTime);
                        if (isValid)
                        {
                            query = query.Where(o => o.IssueDate <= dateTime);
                        }
                    }
                });
            }
           

            query = DBHelper.Sorting(query, orderField, orderType, page,pageSize);
            var items = query.ToList();
            var pageList =
               new PageListData<List<ProjectViewModel>>()
               {
                   Items = items,
                   Total = ProjectViewModels.Count,
                   Current = page,
                   PageSize = pageSize
               };
            return pageList;
        }

        public bool DeleteProject(int id)
        {
            var project = ProjctRepository.Entities.Where(o => o.Id == id).FirstOrDefault();
            var isSucceed = false;
            if (project != null)
            {
                isSucceed = ProjctRepository.Delete(project) > 0;
            }
            ClearCache();
            return isSucceed;
        }
        #endregion

        #region ProjectCompany
        public List<ProjectCompanyViewModel> GetCompaniesByProjectId(int projectId)
        {
            var project = ProjectViewModels.FirstOrDefault(o => o.Id == projectId);
            if (project == null)
                return null;
            return project.Companies;
        }
        #endregion

        #region ProjectNote
        public List<ProjectNoteViewModel> GetNotesByProjectId(int id)
        {
            var project = ProjectViewModels.FirstOrDefault(o => o.Id == id);
            var data = project.Notes;
            var totalRecords = data != null ? data.Count : 0;
            return data;
        }
        #endregion






    }
}
