using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

internal class TrustedUriValidator(IHttpContextAccessor httpContextAccessor, IOptions<TrustedUriSettings> options)
    : ITrustedUriValidator
{
    public ValidationResult? Validate(Uri uri)
    {
        if (uri.IsAbsoluteUri)
        {
            if (MatchesCurrentOrigin(uri) && options.Value.AllowCurrentAuthority)
            {
                return ValidationResult.Success;
            }

            if (options.Value.AllowOrigins.Any(authority =>
                    MatchesOriginSpec(uri, authority)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Origin not permitted");
        }

        if (options.Value.AllowRelative)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Relative URIs are not permitted");
    }

    private bool MatchesOriginSpec(Uri uri, OriginSpec authority)
    {
        return MatchesScheme(uri, authority) && MatchesAuthority(uri, authority) && MatchesPort(uri, authority);
    }

    private bool MatchesPort(Uri uri, OriginSpec authority)
    {
        var authorityPort = authority.Port ?? GetDefaultPort(uri.Scheme);
        return authorityPort.HasValue && uri.Port == authorityPort.Value;
    }

    private bool MatchesAuthority(Uri uri, OriginSpec originSpec)
    {
        return
            string.Equals(uri.Host, originSpec.Authority, StringComparison.InvariantCultureIgnoreCase) ||
            (originSpec.AllowSubAuthorities && uri.Host.EndsWith("." + originSpec.Authority,
                StringComparison.InvariantCultureIgnoreCase));
    }

    private bool MatchesScheme(Uri uri, OriginSpec originSpec)
    {
        return originSpec.Schemes.Any(scheme =>
            string.Equals(scheme, uri.Scheme, StringComparison.InvariantCultureIgnoreCase));
    }

    private bool MatchesCurrentOrigin(Uri uri)
    {
        return httpContextAccessor.HttpContext != null && MatchesOriginSpec(uri, new OriginSpec
        {
            Authority = httpContextAccessor.HttpContext.Request.Host.Host,
            Schemes = [httpContextAccessor.HttpContext.Request.Scheme],
            Port = httpContextAccessor.HttpContext.Request.Host.Port
        });
    }

    private int? GetDefaultPort(string requestScheme)
    {
        return _defaultPorts.TryGetValue(requestScheme, out var port) ? port : null;
    }

    private readonly Dictionary<string, int> _defaultPorts = new(StringComparer.InvariantCultureIgnoreCase)
    {
        ["https"] = 443,
        ["http"] = 80
    };
}