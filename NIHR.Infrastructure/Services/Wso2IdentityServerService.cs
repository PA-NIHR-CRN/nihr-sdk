using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NIHR.Infrastructure.DTOs;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Settings;

namespace NIHR.Infrastructure.Services
{
    public class Wso2IdentityServerService : IIdentityProviderService
    {
        private readonly IDistributedCache _tokenCache;
        private readonly IOptions<IdentityProviderApiSettings> _config;
        private readonly HttpClient _httpClient;
        private const string TokenCacheKey = "api_bearer_token";

        public Wso2IdentityServerService(IDistributedCache tokenCache,
            IOptions<IdentityProviderApiSettings> config,
            HttpClient httpClient)
        {
            _tokenCache = tokenCache;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> GetOrAcquireTokenAsync(CancellationToken cancellationToken = default)
        {
            var cachedToken = await _tokenCache.GetStringAsync(TokenCacheKey, cancellationToken);
            if (string.IsNullOrWhiteSpace(cachedToken))
            {
                var authenticationResponse = await _httpClient.PostAsync(
                    "/oauth2/token",
                    new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            { "client_id", _config.Value.ClientId },
                            { "client_secret", _config.Value.ClientSecret },
                            { "grant_type", "client_credentials" },
                            { "scope", "internal_user_mgt_view internal_user_mgt_list" },
                        }), cancellationToken);

                if (!authenticationResponse.IsSuccessStatusCode)
                {
                    var errorContent = await authenticationResponse.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Failed to acquire token from WSO2. Status: {authenticationResponse.StatusCode}, Body: {errorContent}");
                }

                var tokenResponse =
                    await authenticationResponse.Content
                        .ReadFromJsonAsync<Wso2AuthenticationResponseDto>(cancellationToken);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    throw new InvalidOperationException("Invalid token response from identity provider.");
                }

                if (tokenResponse.ExpiresIn > 120)
                {
                    // Subtracting a buffer from expiresIn to ensure token is refreshed before expiry
                    await _tokenCache.SetStringAsync(
                        TokenCacheKey,
                        tokenResponse.AccessToken,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 120)
                        }, cancellationToken);
                }

                return tokenResponse.AccessToken;
            }

            return cachedToken ??
                   throw new InvalidOperationException("Identity Provider API token found in cache was null.");
        }
    }
}
