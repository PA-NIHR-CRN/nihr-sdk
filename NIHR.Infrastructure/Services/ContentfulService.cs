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

            if (contentRequest.ContentTreeDepth < 1 || contentRequest.ContentTreeDepth > 10)
                throw new ArgumentOutOfRangeException(nameof(contentRequest.ContentTreeDepth), "Content tree depth must be between 1 and 10.");

            var queryBuilder = QueryBuilder<TContent>.New
                .Include(contentRequest.ContentTreeDepth)
                .LocaleIs(contentRequest.Locale)
                .FieldEquals(contentRequest.FieldKey, contentRequest.Id);

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return entries.FirstOrDefault();

        }
    }
}
