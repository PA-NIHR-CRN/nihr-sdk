using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Models;
using NIHR.Infrastructure.Models.ContentRequestQueryTypes;

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

            if (contentRequest.ContentTreeDepth < 1 || contentRequest.ContentTreeDepth > 10)
                throw new ArgumentOutOfRangeException(nameof(contentRequest.ContentTreeDepth), "Content tree depth must be between 1 and 10.");

            var query = buildContentQuery<TContent>(contentRequest);

            var entries = await _contentfulClient.GetEntries(query, cancellationToken);
            return entries.FirstOrDefault();

        }

        public async Task<(List<TContent>, int)> GetContentAsListWithTotalAsync<TContent>(
                ContentRequestModel contentRequest,
                CancellationToken cancellationToken = default)
                where TContent : new()
        {

            if (contentRequest.ContentTreeDepth < 1 || contentRequest.ContentTreeDepth > 10)
                throw new ArgumentOutOfRangeException(nameof(contentRequest.ContentTreeDepth), "Content tree depth must be between 1 and 10.");

            var query = buildContentQuery<TContent>(contentRequest);

            var entries = await _contentfulClient.GetEntries(query, cancellationToken);
            return (entries.ToList(), entries.Total);

        }


        private QueryBuilder<TContent> buildContentQuery<TContent>(ContentRequestModel contentRequest)
        {
            // base query
            var _queryBuilder =  QueryBuilder<TContent>.New
                .Include(contentRequest.ContentTreeDepth)
                .LocaleIs(contentRequest.Locale);

            // Field match
            if (contentRequest.FieldMatchQuery != null && contentRequest.FieldMatchQuery.Count > 0)
            {
                foreach ( ContentRequestFieldMatchQuery fieldMatch in contentRequest.FieldMatchQuery)
                {
                    if(fieldMatch.ContentKey == null || fieldMatch.ContentValue == null)
                    {
                        throw new ArgumentException("Content Value and Content Key cannot be null.", nameof(fieldMatch) );
                    }
                    switch (fieldMatch.SearchMatchType)
                    {
                        case FieldMatchSearchType.EXACT:
                            _queryBuilder.FieldEquals(fieldMatch.ContentKey, fieldMatch.ContentValue);
                            break;
                        case FieldMatchSearchType.PARTIAL:
                            _queryBuilder.FieldMatches(fieldMatch.ContentKey, fieldMatch.ContentValue);
                            break;
                        case FieldMatchSearchType.NOT:
                            _queryBuilder.FieldDoesNotEqual(fieldMatch.ContentKey, fieldMatch.ContentValue);
                            break;
                    }
                }
            }

            // Field includes
            if (contentRequest.FieldIncludesQuery != null && contentRequest.FieldIncludesQuery.Count > 0)
            {
                foreach (ContentRequestFieldIncludesQuery fieldIncludes in contentRequest.FieldIncludesQuery)
                {
                    if (fieldIncludes.ContentKey == null || (fieldIncludes.ContentList != null && fieldIncludes.ContentList.Count() == 0))
                    {
                        throw new ArgumentException("Content Value and Content Key cannot be null.", nameof(fieldIncludes));
                    }


                    switch (fieldIncludes.FieldIncludesSearchType)
                    {
                        case FieldIncludesSearchType.INCLUDES:
                            _queryBuilder.FieldIncludes(fieldIncludes.ContentKey, fieldIncludes.ContentList);
                            break;
                        case FieldIncludesSearchType.EXCLUDES:
                            _queryBuilder.FieldExcludes(fieldIncludes.ContentKey, fieldIncludes.ContentList);
                            break;

                    }

                    
                }
            }

            if (!string.IsNullOrEmpty(contentRequest.FullTextSearchQuery))
            {
                _queryBuilder.FullTextSearch(contentRequest.FullTextSearchQuery);
            }

            //content type restriction
            if (contentRequest.contentType != null && !string.IsNullOrEmpty(contentRequest.contentType))
            {
                _queryBuilder.ContentTypeIs(contentRequest.contentType);
            }


            //limit,skip, orderBY
            if (contentRequest.limit != 0)
            {
                _queryBuilder.Limit(contentRequest.limit);
            }
            if (contentRequest.skip != 0)
            {
                _queryBuilder.Skip(contentRequest.skip);
            }
            if (contentRequest.orderBy != null && !string.IsNullOrEmpty(contentRequest.orderBy))
            {

                _queryBuilder.OrderBy(contentRequest.orderBy);
            }

            return _queryBuilder;


        }

    }
}
