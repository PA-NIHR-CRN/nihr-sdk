using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM.Models
{
    public class SearchRequest
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; } = new List<string> { "urn:ietf:params:scim:api:messages:2.0:SearchRequest" };

        [JsonPropertyName("filter")]
        public string Filter { get; set; } = string.Empty;

        [JsonPropertyName("domain")]
        public string Domain { get; set; } = "PRIMARY";

        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; } = 1;

        [JsonPropertyName("count")]
        public int Count { get; set; } = 10;
    }
}
