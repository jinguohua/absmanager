using ABS.ABSManagementSite.Models;
using AutoMapper;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Controllers;
using CNABS.Mgr.Deal.Model;
using Newtonsoft.Json;
using SAFS.Utility.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABS.ABSManagementSite.Controllers
{
    public class ModelController : BaseController
    {
        // GET: Model
        public ActionResult Index()
        {
            DealModelViewModel model = new DealModelViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveForm(DealModelViewModel vm)
        {
            JsonResultDataEntity<string> result = new JsonResultDataEntity<string>();

            if (vm.Notes.NoteList != null)
                vm.Notes.NoteList = vm.Notes.NoteList.OrderBy(n => n.PaymentOrdinal).ToList();
            var tuple = SaveModel(vm);
            if (tuple.Item1)
            {
                result.Code = 0;
            }
            else
            {
                result.Code = 1;
                result.Message = tuple.Item2;
            }

            return Json(result);
        }

        private Tuple<bool,string> SaveModel(DealModelViewModel vm)
        {

            SimpleDealModel model = new SimpleDealModel();
            Mapper.Map<DealModelViewModel, SimpleDealModel>(vm, model);
            var isSuccess = false;
            var msg = string.Empty;
            try
            {
                List<string> indexList = new List<string>();
                foreach (var note in model.Notes.NoteList)
                {
                    if (!note.IsFixed && note.FloatingIndex != null && !indexList.Contains(note.FloatingIndex))
                    {
                        indexList.Add(note.FloatingIndex);
                    }
                }
                Model mod = new Model {
                    ModelId = vm.Id,
                    ModelName = string.IsNullOrEmpty( vm.ModelName) ? "模型-" + DateTime.Now.ToString() : vm.ModelName,
                    TimeStamp = DateTime.Now,
                    TimeStampUserName = CurrentUser.Name,
                    ModelFolder = "",
                    ModelJson = JsonConvert.SerializeObject(model),
                };
                isSuccess = new ModelManager().SaveSimpleModel(mod);

                //if (ModelManager.SaveSimpleModel(LoadedModel.model_id, model.ToJson()))
                //{
                //    if (!vm.IsInfoComplete)
                //        ModelManager.UpdateModelByModelGuid(LoadedModel.ModelGuid, EModelStatus.ModelSaved);
                //    else
                //        ModelManager.UpdateModelByModelGuid(LoadedModel.ModelGuid, EModelStatus.Complete);
                //}
                //else
                //{
                //    throw new CustomException("保存失败，请重试");
                //}
            }
            catch (Exception ex)
            {
                msg = ex.Message;
               // Loghelper.BugLog(this.GetType().ToString(), ex.Message, ex.StackTrace);
            }

            return new Tuple<bool, string>(isSuccess, msg);
        }
    }
}