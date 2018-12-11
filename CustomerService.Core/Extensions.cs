using System;
using System.Linq;

namespace CustomerService.Core
{
    public static class Extensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> query, int pageIndex = 1, int pageSize = 20) where T : class
        {
            var result = new PagedList<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = query.Count()
            };
            
            var pageCount = (double)result.TotalCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (pageIndex - 1) * pageSize;
            result.List = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static PagedList<T2> Convert<T1, T2>(this PagedList<T1> pagedList, Func<T1, T2> method)
            where T1 : class where T2 : class
        {
            var result = new PagedList<T2>
            {
                PageIndex = pagedList.PageIndex,
                PageCount = pagedList.PageCount,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount,
                List = pagedList.List.Select(method)
            };

            return result;
        }

        public static string InnerMessage(this Exception exception)
        {
            while (true)
            {
                if (exception.InnerException == null) return exception.Message;
                exception = exception.InnerException;
            }
        }
    }
}
