using System.Collections.Generic;

namespace CustomerService.Core
{
    public class PagedList<T>
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> List { get; set; }

        public PagedList()
        {
            List = new List<T>();
        }
    }
}