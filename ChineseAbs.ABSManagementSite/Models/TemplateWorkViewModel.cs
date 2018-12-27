using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Models
{
    [ModelBinder(typeof(TemplateWorkModelBinder))]
    public class TemplateWorkViewModel
    {
        public string TemplateGuid { get; set; }
        public string ShortCode { get; set; }//WorkGuid
        public string WorkId { get; set; }
        public string WorkName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        //public string PreviousWork { get; set; }
        public List<PreviousWork> PreviousWorks { get; set; }
        public string ExtensionType { get; set; }
        public string Target { get; set; }
        public string Description { get; set; }
        public List<TemplateWorkTimeViewModel> WorkTimes { get; set; }

        internal List<SelectListItem>  ExtensionTypeOptions { get; set; }  

        //public IEnumerable<SelectListItem> GetExtensionType()
        //{
        //    var extensionTypes = new List<SelectListItem>();
        //    extensionTypes.Add(new SelectListItem { Text = "扩展类型1", Value = "1" });
        //    extensionTypes.Add(new SelectListItem { Text = "扩展类型2", Value = "2" });
        //    extensionTypes.Add(new SelectListItem { Text = "扩展类型3", Value = "3" });
        //    return extensionTypes;
        //}
    }

    public class PreviousWork
    {
        public string CurrentWorkId { get; set; }
        public string ShortCode { get; set; } //WorkId
        public string WorkName { get; set; }
    }

    public class TemplateWorkModelBinder:IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext modelBindingContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentException("controllerContext is null");
            }
            if (modelBindingContext == null)
            {
                throw new ArgumentException("modelBindingContext is null");
            }
            controllerContext.HttpContext.Request.InputStream.Position = 0;
            var serialize = new DataContractJsonSerializer(modelBindingContext.ModelType);
            return serialize.ReadObject(controllerContext.HttpContext.Request.InputStream);
        }
    }

   
}