using Contentful.Core.Models;
using NIHR.Infrastructure.Models.ContentRequestQueryTypes;
using System.Collections.Generic;

namespace NIHR.Infrastructure.Models
{
    public class ContentRequestModel
    {
        public List<ContentRequestFieldMatchQuery> FieldMatchQuery { get; set; }
        public List<ContentRequestFieldIncludesQuery> FieldIncludesQuery { get; set; }
        public string FullTextSearchQuery { get; set; }
        public string Locale { get; set; } = "en-GB";
        public int ContentTreeDepth { get; set; } = 5;
        public bool UsePreviewApi { get; set; } = false;
        public int limit { get; set; }
        public int skip { get; set; }

        // orderby can be a comma serperated list for multiple field sort.
        public string orderBy { get; set; }

        public string contentType { get; set; }
    }

}
