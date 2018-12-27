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
    public class ProjectNodeController : BaseApiController
    {
        public ProjectNoteService ProjectNoteService { get; set; }

        [HttpGet]
        public List<ProjectNoteViewModel> NodeList(int projectId)
        {
            return ProjectNoteService.GetProjectNoteByProjectId(projectId);
        }

        [HttpPost]
        public bool Save(ProjectNoteViewModel model)
        {
            return ProjectNoteService.SaveProjectNote(model);
        }

        [HttpGet]
        public bool Delete(int id)
        {
            return ProjectNoteService.DeleteProjectNote(id);
        }

    }
}
