using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NIHR.Infrastructure.Models.ContentRequestQueryTypes
{
    public class ContentRequestFieldMatchQuery 
    {
        public string ContentKey { get; } = "sys.id";
        public string ContentValue { get; }
        public SearchType SearchMatchType { get; } = SearchType.EXACT;
        public ContentRequestFieldMatchQuery( string _contentKey, string _contentValue, SearchType _searchType) {
            ContentKey = _contentKey;
            ContentValue = _contentValue;
            SearchMatchType = _searchType;
        }
    }

    public enum SearchType
    {

        EXACT,
        PARTIAL

    }
}
