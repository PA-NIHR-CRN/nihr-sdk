using System.ComponentModel.DataAnnotations;
using System.IO;

namespace NIHR.Application;

internal class Layer0
{
    public string NihrAppName { get; set; }
    public string NihrEnvironment { get; set;}
    public string? NihrConfigurationRoot { get; set; }

    public bool HasConfigurationRoot => !string.IsNullOrWhiteSpace(NihrConfigurationRoot);

    public string? AppConfigRoot => HasConfigurationRoot ? Path.Combine(NihrConfigurationRoot, NihrAppName) : null;

    public virtual void ValidateAndThrow()
    {
        if (string.IsNullOrWhiteSpace(NihrEnvironment))
        {
            throw new ValidationException($"{nameof(NihrEnvironment)} must be specified.");
        }
    }
}