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
            if (MatchesCurrentOrigin(uri) && options.Value.AllowCurrentOrigin)
            {
                return ValidationResult.Success;
            }

            if (options.Value.AllowOrigins.Any(allowedOrigin =>
                    MatchesOriginSpec(uri, allowedOrigin)))
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

    private bool MatchesOriginSpec(Uri uri, OriginSpec originSpec)
    {
        return MatchesScheme(uri, originSpec) && MatchesHost(uri, originSpec) && MatchesPort(uri, originSpec);
    }

    private bool MatchesPort(Uri uri, OriginSpec originSpec)
    {
        var originSpecPort = originSpec.Port ?? GetDefaultPort(uri.Scheme);
        return originSpecPort.HasValue && uri.Port == originSpecPort.Value;
    }

    private bool MatchesHost(Uri uri, OriginSpec originSpec)
    {
        return
            string.Equals(uri.Host, originSpec.Host, StringComparison.InvariantCultureIgnoreCase) ||
            (originSpec.AllowSubHosts && uri.Host.EndsWith("." + originSpec.Host,
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
            Host = httpContextAccessor.HttpContext.Request.Host.Host,
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