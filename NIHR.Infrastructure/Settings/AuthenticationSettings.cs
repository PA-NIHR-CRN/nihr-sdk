using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NIHR.Infrastructure.Settings
{
    public class AuthenticationSettings : IValidatableObject
    {
        public static string SectionName => "AuthenticationSettings";
        [Required] public Uri BaseUrl { get; set; }
        [Required] public string AuthorityPath { get; set; } = "oauth2/token";
        [Required] public string Scim2Path { get; set; } = "scim2/";
        [Required] public string ClientId { get; set; } = string.Empty;
        [Required] public string ClientSecret { get; set; } = string.Empty;

        public bool Bypass { get; set; }
        public string BypassUserId { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BaseUrl is null)
            {
                yield return new ValidationResult($"{nameof(BaseUrl)} is required", new[] { nameof(BaseUrl) });
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                yield return new ValidationResult($"{nameof(ClientId)} is required", new[] { nameof(ClientId) });
            }

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                yield return new ValidationResult($"{nameof(ClientSecret)} is required", new[] { nameof(ClientSecret) });
            }
        }
    }
}
