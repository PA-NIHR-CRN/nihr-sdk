using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NIHR.Infrastructure.Interfaces;

namespace NIHR.Infrastructure.Services
{
    public class ContentfulService : IContentProvider
    {
        private readonly IContentfulClient _contentfulClient;
        private readonly HtmlRenderer _htmlRenderer;

        public ContentfulService(IContentfulClient contentfulClient, HtmlRenderer htmlRenderer)
        {
            _contentfulClient = contentfulClient;
            _htmlRenderer = htmlRenderer;
        }

        public async Task<TContent> GetContentAsync<TContent>(string contentId, CancellationToken cancellationToken) where TContent : new() => await GetContentAsync<TContent>(contentId, ToCamelCase(typeof(TContent).Name), cancellationToken);

        private string ToCamelCase(string name) => char.ToLowerInvariant(name[0]) + name.Substring(1);


        public async Task<TContent> GetContentAsync<TContent>(string contentId, string contentType, CancellationToken cancellationToken) where TContent : new()
        {
            var queryBuilder = QueryBuilder<TContent>.New
                .Include(10)
                //.LocaleIs("cy-GB")
                .FieldEquals("sys.id", contentId);

            var x = (await _contentfulClient.GetEntriesRaw(queryBuilder.Build(), cancellationToken));

            return (await _contentfulClient.GetEntries(queryBuilder, cancellationToken)).FirstOrDefault();

            //var content = /*await GetContentByKeyAsync(contentId, contentType, cancellationToken) ??*/ await _contentfulClient.GetEntry<dynamic>(contentId, cancellationToken: cancellationToken);

            //var resultType = typeof(TContent);
            //TContent retval = new TContent();
            //var properties = resultType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            //var token = (JObject)content;

            //var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };

            //serializer.Converters.Add(new AssetJsonConverter());
            //serializer.Converters.Add(new ContentJsonConverter());

            //foreach (var property in properties)
            //{
            //    var source = token.Property(property.Name, System.StringComparison.InvariantCultureIgnoreCase);

            //    // TODO: support more target property types.
            //    // TODO: support more Contentful source types.

            //    if (property.PropertyType == typeof(string))
            //    {
            //        if (source.HasValues)
            //        {
            //            if (source.Value.GetType() == typeof(JValue))
            //            {
            //                property.SetValue(retval, source.Value.ToString());
            //            }
            //            else if (source.Value.GetType() == typeof(JObject))
            //            {
            //                if (source.Value["nodeType"].ToString() == "document")
            //                {
            //                    var richTextDocument = source.Value.ToObject<Document>(serializer);
            //                    property.SetValue(retval, await _htmlRenderer.ToHtml(richTextDocument));
            //                }
            //            }
            //        }
            //    }
            //}

            //return retval;
        }

        private async Task<dynamic> GetContentByKeyAsync(string contentKey, string contentType, CancellationToken cancellationToken)
        {
            var queryBuilder = new QueryBuilder<dynamic>().FieldExists("fields.key")
                .FieldEquals("fields.key", contentKey)
                .ContentTypeIs(contentType);

            var entries = await _contentfulClient.GetEntries(queryBuilder, cancellationToken);

            return entries.SingleOrDefault();
        }
    }
}
