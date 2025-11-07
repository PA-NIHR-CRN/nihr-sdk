using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using NIHR.Infrastructure.Interfaces;

namespace NIHR.Infrastructure.Services
{
    public class ContentfulService : IContentProvider
    {
        const int CONTENT_TREE_SIZE_LIMIT = 10;
        private readonly IContentfulClient _contentfulClient;

        public ContentfulService(IContentfulClient contentfulClient)
        {
            _contentfulClient = contentfulClient;
        }

        public async Task<TContent> GetContentAsync<TContent>(string contentId, CancellationToken cancellationToken)
            where TContent : new()
        {
            var contentType = ToCamelCase(typeof(TContent).Name);
            return await GetContentAsync<TContent>(contentId, contentType, cancellationToken);
        }

        public async Task<TContent> GetContentAsync<TContent>(string contentId, string contentType, CancellationToken cancellationToken)
            where TContent : new()
        {
            var queryBuilder = QueryBuilder<TContent>.New
                .Include(CONTENT_TREE_SIZE_LIMIT)
                .FieldEquals("sys.id", contentId);

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return entries.FirstOrDefault();
        }

        private async Task<dynamic> GetContentByKeyAsync(string contentKey, string contentType, CancellationToken cancellationToken)
        {
            var queryBuilder = new QueryBuilder<dynamic>()
                .FieldExists("fields.key")
                .FieldEquals("fields.key", contentKey)
                .ContentTypeIs(contentType);

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return entries.SingleOrDefault();
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
                return name;

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}
