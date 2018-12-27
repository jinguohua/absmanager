using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models
{
    public class Page <T> where T : new()
    {
        public Page()
        {
        }

        public Page<T> Parse<DbTable>(PetaPoco.Page<DbTable> page)
        {
            CurrentPage = page.CurrentPage;
            TotalPages = page.TotalPages;
            TotalItems = page.TotalItems;
            ItemsPerPage = page.ItemsPerPage;
            return this;
        }

        public long CurrentPage { get; set; }

        public long TotalPages { get; set; }

        public long TotalItems { get; set; }

        public long ItemsPerPage { get; set; }

        public List<T> Items;
    }
}
