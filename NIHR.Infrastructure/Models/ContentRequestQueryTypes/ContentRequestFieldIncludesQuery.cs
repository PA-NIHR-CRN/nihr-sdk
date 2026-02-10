using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace NIHR.Infrastructure.Models.ContentRequestQueryTypes
{
    public class ContentRequestFieldIncludesQuery 
    {

        public string ContentKey { get; }
        public IEnumerable<string> ContentList { get; }

        public FieldIncludesSearchType FieldIncludesSearchType { get; set; }

        public ContentRequestFieldIncludesQuery(string _contentKey, IEnumerable<string> _contentList, FieldIncludesSearchType _fieldIncludesSearchType)
        {
            ContentKey = _contentKey;
            ContentList = _contentList;
            FieldIncludesSearchType = _fieldIncludesSearchType;
        }
        public ContentRequestFieldIncludesQuery(string _contentKey, IEnumerable<string> _contentList)
        {
            ContentKey = _contentKey;
            ContentList = _contentList;
        }

    }

    public enum FieldIncludesSearchType
    {
        INCLUDES,
        EXCLUDES

    }
}
