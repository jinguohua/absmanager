using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class SearchFilter
    {
        public List<Filter> Filters { get; set; }
    }

    public class Filter
    {
        public string Field { get; set; }
        public string Rule { get; set; }
        public string Value { get; set; }
    }
}
