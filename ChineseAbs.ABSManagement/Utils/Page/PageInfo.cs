using System;

namespace ChineseAbs.ABSManagement.Utils
{
    public class PageInfo
    {
        public PageInfo()
        {
        }

        public PageInfo(int totalItems, int? itemsPerPage, int? currentPage)
        {
            TotalItems = totalItems;

            if (itemsPerPage.HasValue && currentPage.HasValue)
            {
                CurrentPage = Math.Max(0, currentPage.Value);
                ItemsPerPage = Math.Max(0, itemsPerPage.Value);
            }
            else
            {
                CurrentPage = TotalItems == 0 ? 0 : 1;
                ItemsPerPage = TotalItems;
            }

            if (ItemsPerPage != 0)
            {
                TotalPages = (TotalItems % ItemsPerPage == 0) ? (TotalItems / ItemsPerPage) : (TotalItems / ItemsPerPage + 1);
            }
            else
            {
                TotalPages = 0;
            }
        }

        public long CurrentPage { get; set; }

        public long TotalPages { get; set; }

        public long TotalItems { get; set; }

        public long ItemsPerPage { get; set; }
    }
}
