using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM.Models
{

    public abstract class ScimBase
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; } = new List<string>();
    }

    public class UserResource : ScimBase
    {
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; } = new List<string>();

        [JsonPropertyName("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
        public UrnIetfParamsScimSchemasExtensionEnterprise20User UrnIetfParamsScimSchemasExtensionEnterprise20User { get; set; }

        [JsonPropertyName("roles")]
        public List<Role> Roles { get; set; } = new List<Role>();
    }


    public class ListResponse : ScimBase
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("Resources")]
        public List<UserResource> Resources { get; set; } = new List<UserResource>();
    }
}
