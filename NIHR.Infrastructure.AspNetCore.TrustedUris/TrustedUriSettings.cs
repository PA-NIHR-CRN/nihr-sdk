namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

public class TrustedUriSettings
{
    public bool AllowRelative { get; set; } = true;
    public bool AllowCurrentOrigin { get; set; } = true;
    public OriginSpec[] AllowOrigins { get; set; } = [];
}