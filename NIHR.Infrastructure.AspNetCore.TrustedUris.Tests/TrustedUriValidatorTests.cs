using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NIHR.Infrastructure.AspNetCore.TrustedUris;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris.Tests;

public class TrustedUriValidatorTests
{
    [Theory]
    [InlineData("/relative/path")]
    [InlineData("../relative/path")]
    [InlineData("relative/path?query=value")]
    [InlineData("#section")]
    public void Validate_AllowsRelativeUrisWhenConfigured(string redirectUri)
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowRelative = true
        });

        var result = validator.Validate(new Uri(redirectUri, UriKind.Relative));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_RejectsRelativeUrisWhenNotConfigured()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowRelative = false
        });

        var result = validator.Validate(new Uri("/relative/path", UriKind.Relative));

        Assert.Equal("Relative URIs are not permitted", result?.ErrorMessage);
    }
    

    [Fact]
    public void Validate_AllowsAbsoluteUriMatchingCurrentOrigin()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = true
        });

        var result = validator.Validate(new Uri("https://app.example.com/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_MatchesCurrentOriginCaseInsensitively()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = true
        });

        var result = validator.Validate(new Uri("HTTPS://APP.EXAMPLE.COM/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_RejectsCurrentHostWithDifferentScheme()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = true
        });

        var result = validator.Validate(new Uri("http://app.example.com/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsCurrentOriginWhenNotAllowed()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false
        });

        var result = validator.Validate(new Uri("https://app.example.com/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    [Fact]
    public void Validate_AllowsCurrentOriginWithConfiguredNonDefaultPort()
    {
        var validator = CreateValidator(
            new TrustedUriSettings { AllowCurrentAuthority = true },
            requestPort: 8443);

        var result = validator.Validate(new Uri("https://app.example.com:8443/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_AllowsOriginMatchingConfiguredAuthority()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"]
                }
            ]
        });

        var result = validator.Validate(new Uri("https://trusted.example.com/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_MatchesConfiguredSchemeAndAuthorityCaseInsensitively()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["HTTPS"]
                }
            ]
        });

        var result = validator.Validate(new Uri("https://TRUSTED.EXAMPLE.COM/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_UsesDefaultHttpsPortWhenOriginPortIsNotSpecified()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"]
                }
            ]
        });

        var result = validator.Validate(new Uri("https://trusted.example.com:443/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_UsesDefaultHttpPortWhenOriginPortIsNotSpecified()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["http"]
                }
            ]
        });

        var result = validator.Validate(new Uri("http://trusted.example.com/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_AllowsConfiguredNonDefaultPort()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"],
                    Port = 8443
                }
            ]
        });

        var result = validator.Validate(new Uri("https://trusted.example.com:8443/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_RejectsConfiguredOriginOnDifferentPort()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"],
                    Port = 8443
                }
            ]
        });

        var result = validator.Validate(new Uri("https://trusted.example.com:9443/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    [Fact]
    public void Validate_AllowsSubdomainWhenConfigured()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"],
                    AllowSubAuthorities = true
                }
            ]
        });

        var result = validator.Validate(new Uri("https://api.trusted.example.com/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_AllowsNestedSubdomainWhenConfigured()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"],
                    AllowSubAuthorities = true
                }
            ]
        });

        var result = validator.Validate(new Uri("https://api.eu.trusted.example.com/return"));

        Assert.Null(result);
    }

    [Fact]
    public void Validate_RejectsSubdomainWhenNotConfigured()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"],
                    AllowSubAuthorities = false
                }
            ]
        });

        var result = validator.Validate(new Uri("https://api.trusted.example.com/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsHostThatOnlyEndsWithConfiguredAuthorityText()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"],
                    AllowSubAuthorities = true
                }
            ]
        });

        var result = validator.Validate(new Uri("https://nottrusted.example.com/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsConfiguredAuthorityWithUnlistedScheme()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false,
            AllowOrigins =
            [
                new OriginSpec
                {
                    Authority = "trusted.example.com",
                    Schemes = ["https"]
                }
            ]
        });

        var result = validator.Validate(new Uri("http://trusted.example.com/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsAbsoluteOriginNotInAllowList()
    {
        var validator = CreateValidator(new TrustedUriSettings
        {
            AllowCurrentAuthority = false
        });

        var result = validator.Validate(new Uri("https://untrusted.example.com/return"));

        Assert.Equal("Origin not permitted", result?.ErrorMessage);
    }

    private static TrustedUriValidator CreateValidator(
        TrustedUriSettings settings,
        string requestScheme = "https",
        string requestHost = "app.example.com",
        int? requestPort = null)
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = requestScheme,
                Host = requestPort.HasValue
                    ? new HostString(requestHost, requestPort.Value)
                    : new HostString(requestHost)
            }
        };

        return new TrustedUriValidator(
            new HttpContextAccessor { HttpContext = httpContext },
            Options.Create(settings));
    }
}