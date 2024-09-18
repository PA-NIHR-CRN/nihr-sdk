using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM.Models
{
    public class ScimError
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; } = new List<string>();

        [JsonPropertyName("scimType")]
        public string ScimType { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
