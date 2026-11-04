using System.ComponentModel.DataAnnotations;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

public interface ITrustedUriValidator
{
    ValidationResult? Validate(Uri uri);
}