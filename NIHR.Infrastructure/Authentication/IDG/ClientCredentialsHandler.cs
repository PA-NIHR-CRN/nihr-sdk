using Microsoft.Extensions.Options;
using NIHR.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Authentication.IDG
{
    public class ClientCredentialsHandler : DelegatingHandler
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly HttpClient httpClient;

        public ClientCredentialsHandler(IOptions<AuthenticationSettings> authenticationSettings, HttpClient httpClient)
        {
            _authenticationSettings = authenticationSettings.Value;
            this.httpClient = httpClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            WSO2AuthenticationResponse tokenResponse = await GetAdminTokenAsync(cancellationToken);

            if (tokenResponse != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        async Task<WSO2AuthenticationResponse> GetAdminTokenAsync(CancellationToken cancellationToken = default)
        {
            var authenticationResponse = await httpClient.PostAsync(
            _authenticationSettings.AuthorityPath,
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                        { "client_id", _authenticationSettings.ClientId },
                        { "client_secret", _authenticationSettings.ClientSecret },
                        { "grant_type", "client_credentials" },
                        { "scope", "internal_identity_mgt_view internal_identity_mgt_update internal_identity_mgt_create internal_user_mgt_create internal_identity_mgt_delete internal_user_mgt_list"},
                }), cancellationToken);

            authenticationResponse.EnsureSuccessStatusCode();

            var tokenResponse = await authenticationResponse.Content.ReadFromJsonAsync<WSO2AuthenticationResponse>(cancellationToken);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Invalid token response from identity provider.");
            }

            return tokenResponse;
        }
    }
}