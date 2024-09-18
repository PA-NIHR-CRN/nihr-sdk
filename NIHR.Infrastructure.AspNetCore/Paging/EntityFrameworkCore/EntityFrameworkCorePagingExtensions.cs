namespace NIHR.Infrastructure.Paging
{
    public static class EntityFrameworkCorePagingExtensions
    {
        public static PageDeferred<T> DeferredPage<T>(this IOrderedQueryable<T> source, IPaginationService paginationService) => source.DeferredPage(paginationService.PageSize, paginationService.Page);

        public static Page<T> Page<T>(this IOrderedQueryable<T> source, IPaginationService paginationService) => source.Page(paginationService.PageSize, paginationService.Page);

        public static async Task<Page<T>> PageAsync<T>(this IOrderedQueryable<T> source, IPaginationService paginationService, CancellationToken token = default) => await source.PageAsync(paginationService.PageSize, paginationService.Page, token);
    }
}
