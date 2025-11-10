using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Models;

namespace NIHR.Infrastructure.Services
{
    public class ContentfulService : IContentProvider
    {
        private readonly IContentfulClient _contentfulClient;

        public ContentfulService(IContentfulClient contentfulClient)
        {
            _contentfulClient = contentfulClient;
        }

        public async Task<TContent> GetContentAsync<TContent>(
                    ContentRequestModel contentRequest,
                    CancellationToken cancellationToken = default)
                    where TContent : new()
        {
            if (string.IsNullOrWhiteSpace(contentRequest.Id))
                throw new ArgumentException("Content ID cannot be null or empty.", nameof(contentRequest.Id));

            var queryBuilder = QueryBuilder<TContent>.New
                .Include(contentRequest.ContentTreeDepth)
                .LocaleIs(contentRequest.Locale)
                .FieldEquals("sys.id", contentRequest.Id);

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return entries.FirstOrDefault();

        }
    }
}
