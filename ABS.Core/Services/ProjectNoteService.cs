using ABS.Core.DTO;
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
    public class ProjectNoteService : ServiceBase
    {
        public IRepository<ProjectNote, int> ProjctNoteRepository { get; set; }
        public IRepository<NoteRating,int> NoteRatingRepository { get; set; }


      //  readonly string projctNoteCachekey = "ProjctNoteCachekey";

        static object _sync = new object();

        public ProjectNoteService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }

        private void ClearCache()
        {
            lock (_sync)
            {
                CacheHelper.Remove(CacheCenter.ProjectCacheKey);
                CacheHelper.Remove(CacheCenter.ProjctNoteCachekey);
            }
        }

        private List<ProjectNoteViewModel> ProjectNoteViewModels
        {
            get
            {
                return CacheHelper.Get<List<ProjectNoteViewModel>>(CacheCenter.ProjctNoteCachekey, GetProjectNoteViewModels);
            }
        }

        private List<ProjectNoteViewModel> GetProjectNoteViewModels()
        {
            var models =  ProjctNoteRepository.Entities.ToList();

            var viewModels = AutoMapper.Mapper.Map<List<ProjectNote>, List<ProjectNoteViewModel>>(models);
            return viewModels;
        }  

        public List<ProjectNoteViewModel> GetProjectNoteByProjectId(int projectId)
        {
            var models = ProjctNoteRepository.Entities.Where(o=> o.ProjectID == projectId).ToList();
            var viewModels = ProjectNoteViewModels.Where(o => o.ProjectID == projectId).OrderBy(o=> o.Name).ToList();
            return viewModels;
        }

        public bool SaveProjectNote(ProjectNoteViewModel viewModel)
        {
            var isSucceed = false;
            if (viewModel == null)
                return false;
            var noteId = 0;
            if (viewModel.Id > 0)
            {
                var oldEntity = ProjctNoteRepository.Entities.FirstOrDefault(o => o.Id == viewModel.Id);
                if (oldEntity == null)
                    throw new Exception($"不存在Id为：{viewModel.Id}的ProjctNote");
                var entity = AutoMapper.Mapper.Map<ProjectNoteViewModel, ProjectNote>(viewModel);
                entity.CreatedTime = oldEntity.CreatedTime;
                isSucceed = ProjctNoteRepository.Update(entity) > 0;
                noteId = entity.Id;
            }
            else
            {
                var entity = AutoMapper.Mapper.Map<ProjectNoteViewModel, ProjectNote>(viewModel);
                entity.CreatedTime = DateTime.Now;
                entity.Creator = "";
                isSucceed = ProjctNoteRepository.Insert(entity) > 0;
                noteId = entity.Id;
            }
            #region NoteRating 评级
            if (viewModel.Ratings != null)
            {
                viewModel.Ratings.ForEach(viewNote =>
                {
                    viewNote.NoteID = noteId;
                    if (viewNote.Id > 0)
                    {
                        var oldNote = NoteRatingRepository.Entities.FirstOrDefault(o => o.Id == viewNote.Id);
                        var entity = AutoMapper.Mapper.Map<NoteRatingViewModel,NoteRating>(viewNote);
                        entity.CreatedTime = oldNote.CreatedTime;
                        NoteRatingRepository.Update(entity);
                    }
                    else
                    {
                        var entity = AutoMapper.Mapper.Map<NoteRatingViewModel, NoteRating>(viewNote);
                        entity.CreatedTime = DateTime.Now;
                        entity.Creator = "";
                        NoteRatingRepository.Insert(entity);
                    }
                });
            }
            #endregion

            if (isSucceed)
                ClearCache();
            return isSucceed;
        }

        public bool DeleteProjectNote(int noteId)
        {
            var isSucceed = false;
            var entity = ProjctNoteRepository.Entities.FirstOrDefault(o => o.Id == noteId);
            if (entity == null)
                throw new Exception($"不存在Id为：{noteId}的ProjctNote");
            isSucceed = ProjctNoteRepository.Delete(entity) >0;
            if(isSucceed)
                ClearCache();
            return isSucceed;
        }

    }
}
