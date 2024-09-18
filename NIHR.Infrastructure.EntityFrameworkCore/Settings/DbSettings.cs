using System.ComponentModel.DataAnnotations;

namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public class DbSettings : IValidatableObject
    {
        public static string SectionName => "DbSettings";
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Host { get; set; } = null!;
        public uint? Port { get; set; }
        public string Database { get; set; } = null!;
        public string NotificationDatabase { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                yield return new ValidationResult("Username is required", new[] { nameof(Username) });
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Password is required", new[] { nameof(Password) });
            }

            if (string.IsNullOrWhiteSpace(Host))
            {
                yield return new ValidationResult("Host is required", new[] { nameof(Host) });
            }

            if (Port < 0 || Port > 65535)
            {
                yield return new ValidationResult("Port is out of range", new[] { nameof(Port) });
            }

            if (string.IsNullOrWhiteSpace(Database))
            {
                yield return new ValidationResult("Database is required", new[] { nameof(Database) });
            }
        }
    }
}
