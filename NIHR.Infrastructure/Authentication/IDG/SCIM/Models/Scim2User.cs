using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM.Models
{
    public class NewUser
    {
        [JsonPropertyName("schemas")]
        public List<string> Schemas { get; set; } = new List<string>();

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = Guid.NewGuid().ToString("D");

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("emails")]
        public List<Email> Emails { get; set; } = new List<Email>();

        [JsonPropertyName("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
        public UrnIetfParamsScimSchemasExtensionEnterprise20User UrnIetfParamsScimSchemasExtensionEnterprise20User { get; set; }
    }

    public class Email
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Manager
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }

        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }
    }

    public class UrnIetfParamsScimSchemasExtensionEnterprise20User
    {
        [JsonPropertyName("employeeNumber")]
        public string EmployeeNumber { get; set; }

        [JsonPropertyName("manager")]
        public Manager Manager { get; set; }
    }
}
