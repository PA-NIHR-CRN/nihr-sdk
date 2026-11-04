namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

public class TrustedUriSettings
{
    public bool AllowRelative { get; set; } = true;
    public bool AllowCurrentAuthority { get; set; } = true;
    public OriginSpec[] AllowOrigins { get; set; } = [];
}