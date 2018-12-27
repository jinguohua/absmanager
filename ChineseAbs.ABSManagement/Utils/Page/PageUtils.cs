using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class PageUtils
    {
        static public List<T> GetRange<T>(List<T> list, PageInfo pageInfo)
        {

            var begin = (pageInfo.CurrentPage - 1) * pageInfo.ItemsPerPage;
            var end = pageInfo.CurrentPage * pageInfo.ItemsPerPage;

            var result = new List<T>();
            for (int i = 0; i < list.Count; ++i)
            {
                if (i >= (pageInfo.CurrentPage - 1) * pageInfo.ItemsPerPage
                    && i < pageInfo.CurrentPage * pageInfo.ItemsPerPage)
                {
                    result.Add(list[i]);
                }
            }

            return result;
        }
    }
}
