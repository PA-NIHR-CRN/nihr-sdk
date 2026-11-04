using System.ComponentModel.DataAnnotations;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

public class OriginSpec
{
    public string[] Schemes { get; set; } = ["https"];
    [Required]
    public required string Host { get; set; }

    public int? Port { get; set; }
    public bool AllowSubHosts { get; set; } = true;
}