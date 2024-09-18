using Microsoft.AspNetCore.Http;

namespace NIHR.Infrastructure.Paging
{
    public class RequestQueryParamPaginationService : IPaginationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int _defaultPageSize = 10;
        private const int _defaultPage = 1;

        public RequestQueryParamPaginationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int Page => int.TryParse(_httpContextAccessor?.HttpContext?.Request.Query["page"], out var page) ? page : _defaultPage;


        public int PageSize => int.TryParse(_httpContextAccessor?.HttpContext?.Request.Query["pageSize"], out var pageSize) ? pageSize : _defaultPageSize;

        public Uri GetPageUri(int pageNumber, string? anchorId = null)
        {
            var request = _httpContextAccessor.HttpContext.Request;

            var query = request.Query.ToDictionary();
            query.Remove("page");
            query.Remove("pageSize");

            query.Add("page", pageNumber.ToString());
            query.Add("pageSize", PageSize.ToString());

            var newQueryString = QueryString.Create(query);

            var url = $"{request.PathBase}{request.Path}{newQueryString}";

            if (!string.IsNullOrWhiteSpace(anchorId))
            {
                url = $"{url}#{anchorId}";
            }

            return new Uri(url, UriKind.Relative);
        }
    }
}
