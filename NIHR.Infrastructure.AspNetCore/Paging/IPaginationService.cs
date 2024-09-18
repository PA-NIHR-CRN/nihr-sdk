namespace NIHR.Infrastructure.Paging
{
    public interface IPaginationService
    {
        int Page { get; }
        int PageSize { get; }

        Uri GetPageUri(int pageNumber, string? anchorId = null);
    }
}
