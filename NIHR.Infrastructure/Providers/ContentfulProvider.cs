using System;
using System.Collections.Generic;
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

            if (contentRequest.contentType != null)
            {
                queryBuilder.ContentTypeIs(contentRequest.contentType);
            }

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return entries.FirstOrDefault();

        }

        public async Task<(List<TContent>, int)> GetContentAsListWithTotalAsync<TContent>(
                ContentRequestModel contentRequest,
                CancellationToken cancellationToken = default)
                where TContent : new()
        {

            if (contentRequest.ContentTreeDepth < 1 || contentRequest.ContentTreeDepth > 10)
                throw new ArgumentOutOfRangeException(nameof(contentRequest.ContentTreeDepth), "Content tree depth must be between 1 and 10.");

            var queryBuilder = QueryBuilder<TContent>.New
                .Include(contentRequest.ContentTreeDepth)
                .LocaleIs(contentRequest.Locale);


            if (contentRequest.ContentKey != null)
            {
                queryBuilder.FieldEquals(contentRequest.ContentKey, contentRequest.ContentValue);
            }

            if (contentRequest.contentType != null)
            {
                queryBuilder.ContentTypeIs(contentRequest.contentType);
            }

            if (contentRequest.limit != 0)
            {
                queryBuilder.Limit(contentRequest.limit);
            }
            if (contentRequest.skip != 0)
            {
                queryBuilder.Skip(contentRequest.skip);
            }
            if (contentRequest.orderBy != null)
            {

                queryBuilder.OrderBy(contentRequest.orderBy);
            }

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);
            return (entries.ToList(), entries.Total);

        }

    }
}
