using ABS.Core.DTO;
using ABS.Core.Models;
using ABS.Core.Services;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class ProjectController : BaseApiController
    {
        public ProjectService ProjectService { get; set; }

        [HttpGet]
        [ActionName("detail")]
        public ProjectViewModel Project(int id)
        {
            return ProjectService.GetProjectById(id); 
        }

        [HttpPost]
        [ActionName("list")]
        public PageListData<List<ProjectViewModel>> Projects(QueryPageModel model )
        {       
            return ProjectService.GetProjectsByPage(model);
        }

        [HttpGet]
        public List<ProjectCompanyViewModel> CompaniesOfProject(int projectId)
        {
            return ProjectService.GetCompaniesByProjectId(projectId);
        }

        [HttpPost]
        public bool Delete(int id)
        {
            return ProjectService.DeleteProject(id);
        }

        [HttpPost]
        public string Save(ProjectViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("数据验证失败");
            }
            if (viewModel.Id > 0)
            {
                ProjectService.UpdateProject(viewModel);
            }
            else
            {
                viewModel.Id = ProjectService.AddProject(viewModel);
            }
            return viewModel.Id.ToString();
        }

    }
}
