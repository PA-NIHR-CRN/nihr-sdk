using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NIHR.Infrastructure.Settings
{
    public class EmailSettings : IValidatableObject
    {
        public const string SectionName = nameof(EmailSettings);
        public string FromAddress { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FromAddress))
            {
                yield return new ValidationResult("FromAddress is required", new[] { nameof(FromAddress) });
            }
        }
    }
}
