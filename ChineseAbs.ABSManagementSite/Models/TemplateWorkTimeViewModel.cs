using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Models
{
    [ModelBinder(typeof(TemplateWorkModelBinder))]
    public class TemplateWorkTimeViewModel
    {        
        public string ShortCode { get; set; } // WorkGuid
        public string WorkTimeGuid { get; set; }
        public string TimeType { get; set; }

        public string TimeOrigin { get; set; }

        public string NewBuiltType { get; set; }

        public string CycleStartTime { get; set; }
        public string CycleGapTime { get; set; }
        public string CycleUnitType { get; set; }
        public string CycleMaxTime { get; set; }

        public string AppointTimeString { get; set; }

        public string BaseShortCode { get; set; }//baseworkid
        public string BaseStartOrEndTime { get; set; }

        public List<TemplateWorkTimeRuleViewModel> Rules { get; set; }

        internal List<SelectListItem> OriginOptions { get; set; }
        internal List<SelectListItem> NewBuiltOptions { get; set; }
        internal List<SelectListItem> CycleUnitOptions { get; set; }
        internal List<SelectListItem> WorkOptions { get; set; }
        internal List<SelectListItem> TimeRuleOptions { get; set; }

    }
}