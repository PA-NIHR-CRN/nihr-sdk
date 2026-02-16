using System;
using System.Collections.Generic;
using System.Text;

namespace NIHR.Infrastructure.Interfaces
{
    public interface IQueryStringService
    {

        Uri AddToUri(string param, string value, string? deliminator = ",", string? anchorId = null);

        Uri RemoveFromUri(string param, string value, string? deliminator = ",", string? anchorId = null);

        bool isPresentInUrl(string param, string value, string? deliminator = ",", bool isExactMatch = true);
        public Uri removeParam(string param, string? anchorId = null);

        public string getFromURL(string param);

    }
}
