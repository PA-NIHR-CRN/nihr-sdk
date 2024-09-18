using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace NIHR.Infrastructure.Paging
{
    public static class Extensions
    {
        public static PageDeferred<T> DeferredPage<T>(this IOrderedQueryable<T> source, int pageSize, int currentPage)
        {
            var items = source.PageItems(pageSize, currentPage).Future();
            var totalCount = source.DeferredCount().FutureValue();

            return new PageDeferred<T>(items, pageSize, currentPage, totalCount);
        }

        public static Page<T> Page<T>(this IOrderedQueryable<T> source, int pageSize, int currentPage)
        {
            var items = source.PageItems(pageSize, currentPage).ToList();
            var totalCount = source.Count();

            return new Page<T>(items, pageSize, currentPage, totalCount);
        }

        public static async Task<Page<T>> PageAsync<T>(this IOrderedQueryable<T> source, int pageSize, int currentPage, CancellationToken token = default)
        {
            var items = await source.PageItems(pageSize, currentPage).ToListAsync(token);
            var totalCount = await source.CountAsync(token);

            return new Page<T>(items, pageSize, currentPage, totalCount);
        }

        private static IQueryable<T> PageItems<T>(this IOrderedQueryable<T> source, int pageSize, int currentPage) => source.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }
}
