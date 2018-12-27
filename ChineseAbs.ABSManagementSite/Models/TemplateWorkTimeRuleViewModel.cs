using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class TemplateWorkTimeRuleViewModel
    {
        public string RuleGuid { get; set; }
        public string RuleOrder { get; set; }
        public string RuleType { get; set; }

        public string SpecialDayType { get; set; }
        public string MoveSpecialDayDirection { get; set; }
        public string MoveSpecialDayCount { get; set; }

        public string UnitType { get; set; }
        public string UnitSequenceDay { get; set; }
        
    }
}