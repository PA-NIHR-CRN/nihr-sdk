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
        public FieldMatchSearchType SearchMatchType { get; } = FieldMatchSearchType.EXACT;
        public ContentRequestFieldMatchQuery( string _contentKey, string _contentValue, FieldMatchSearchType _searchType) {
            ContentKey = _contentKey;
            ContentValue = _contentValue;
            SearchMatchType = _searchType;
        }

        public ContentRequestFieldMatchQuery(string _contentKey, string _contentValue)
        {
            ContentKey = _contentKey;
            ContentValue = _contentValue;
        }
    }

    public enum FieldMatchSearchType
    {

        EXACT,
        PARTIAL,
        NOT

    }
}
