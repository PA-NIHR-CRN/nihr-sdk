using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM.Models
{
    public class Meta
    {
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }
    }

    public class Role
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class CreatedUser : ScimBase
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


}
