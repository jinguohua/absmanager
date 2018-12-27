using ABS.Core.DTO;
using ABS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class ProjectAccountController :  BaseApiController
    {
        public ProjectAccountService ProjectAccountService { get; set; }

        [HttpPost]
        public List<ProjectAccountViewModel> List(int projectId)
        {
            return ProjectAccountService.GetProjectAccountsByProjectId(projectId);
        }

        [HttpPost]
        public ProjectAccountViewModel Detail(int id, int projectId)
        {
            return ProjectAccountService.GetProjectAccountsByProjectId(projectId)
                .Where(o=>o.Id == id).FirstOrDefault();
        }

        [HttpPost]
        public bool Save(ProjectAccountViewModel model)
        {
            return ProjectAccountService.Save(model);
        } 

        [HttpPost]
        public bool Delete(int id)
        {
            return ProjectAccountService.Delete(id);
        }
    }
}
