using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABS.Core.Models
{
    public class MenuViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public string CssClassName { get; set; }
        public string FunctionID { get; set; }
        public string ParentID { get; set; }
        public string URL { get; set; }
        public string Extension { get; set; }
        public List<MenuViewModel> Children{get ;set;}
    }
}