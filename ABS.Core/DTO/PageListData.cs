using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class PageListData<T> : PageListData
    {
        public T Items { get; set; }
    }

    public class PageListData
    {
        public int Current { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPage
        {
            get
            {
                var t = 0;
                if(Total>0 && PageSize > 0)
                {
                    t = (int) Math.Ceiling( Total*1D / PageSize);
                }
                return t;
            }

            set { }
        }
    }
}
