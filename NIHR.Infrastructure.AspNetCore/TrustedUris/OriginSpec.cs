using System.ComponentModel.DataAnnotations;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

public class OriginSpec
{
    public string[] Schemes { get; set; } = ["https"];
    [Required]
    public required string Authority { get; set; }

    public int? Port { get; set; }
    public bool AllowSubAuthorities { get; set; } = true;
}