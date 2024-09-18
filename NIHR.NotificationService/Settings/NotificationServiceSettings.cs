using System.ComponentModel.DataAnnotations;

namespace NIHR.NotificationService.Settings
{
    public class NotificationServiceSettings : IValidatableObject
    {
        public static string SectionName => "NotificationServiceSettings";
        public string ApiKey { get; set; }
        public string BearerToken { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                yield return new ValidationResult("ApiKey is required", new[] { nameof(ApiKey) });
            }
            if (string.IsNullOrWhiteSpace(BearerToken))
            {
                yield return new ValidationResult("BearerToken is required", new[] { nameof(BearerToken) });
            }
        }
    }
}
