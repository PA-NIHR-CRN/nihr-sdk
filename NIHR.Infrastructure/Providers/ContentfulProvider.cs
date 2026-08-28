using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Models;

namespace NIHR.Infrastructure.Providers
{
    public class ContentfulProvider : IContentProvider
    {
        private readonly IContentfulClient _contentfulClient;

        public ContentfulProvider(IContentfulClient contentfulClient)
        {
            _contentfulClient = contentfulClient;
        }

        public async Task<TContent> GetContentAsync<TContent>(
                    ContentRequestModel contentRequest,
                    CancellationToken cancellationToken = default)
                    where TContent : new()
        {
            if (string.IsNullOrWhiteSpace(contentRequest.ContentValue))
                throw new ArgumentException("Content Value cannot be null or empty.", nameof(contentRequest.ContentValue));

            if (contentRequest.ContentTreeDepth < 1 || contentRequest.ContentTreeDepth > 10)
                throw new ArgumentOutOfRangeException(nameof(contentRequest.ContentTreeDepth), "Content tree depth must be between 1 and 10.");

            var queryBuilder = QueryBuilder<TContent>.New
                .Include(contentRequest.ContentTreeDepth)
                .LocaleIs(contentRequest.Locale)
                .FieldEquals(contentRequest.ContentKey, contentRequest.ContentValue);

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return entries.FirstOrDefault();

        }
    }
}
