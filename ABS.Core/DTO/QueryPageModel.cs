using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class QueryPageModel
    {
        public int Page { get; set; }

        int pageSize = 2;

        public int PageSize { get { return pageSize; } set { if (value != 0) pageSize = value; } }

        public string SortField { get; set; }

        public string SortType { get; set; }

        public Dictionary<string, string> Filters { get; set; }
    }
}
