using System.Collections.Generic;
using System.Linq;

namespace NIHR.Infrastructure.Paging
{
    public static class PageExtensions
    {
        public static Page<T> Page<T>(this IOrderedEnumerable<T> source, int pageSize, int currentPage)
        {
            var items = source.PageItems(pageSize, currentPage).ToList();
            var totalCount = source.Count();

            return new Page<T>(items, pageSize, currentPage, totalCount);
        }
        private static IEnumerable<T> PageItems<T>(this IOrderedEnumerable<T> source, int pageSize, int currentPage) => source.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }
}
