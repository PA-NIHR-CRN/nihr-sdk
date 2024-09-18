using NIHR.Infrastructure.Authentication.IDG.SCIM.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NIHR.Infrastructure.Authentication.IDG.SCIM
{
    public class Scim2UserManagement : IUserAccountStore
    {
        private const string PASSWORD_POLICY_ERROR = "Password pattern policy violated.";
        private readonly MediaTypeHeaderValue scimjson = new MediaTypeHeaderValue("application/scim+json");
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        private readonly HttpClient _httpClient;

        public Scim2UserManagement(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<CreatedUser> CreateUserAsync(NewUser newUser, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(newUser.UserName))
            {
                throw new ArgumentException(nameof(newUser.UserName));
            }

            if ((newUser.Emails?.Count ?? 0) < 1)
            {
                throw new ArgumentException(nameof(newUser.Emails));
            }

            if (string.IsNullOrWhiteSpace(newUser.Password))
            {
                throw new ArgumentException(nameof(newUser.Password));
            }

            if (string.IsNullOrEmpty(newUser?.Name?.FamilyName))
            {
                throw new ArgumentException(nameof(newUser.Name.FamilyName));
            }

            if (string.IsNullOrEmpty(newUser?.Name?.GivenName))
            {
                throw new ArgumentException(nameof(newUser.Name.GivenName));
            }

            var content = JsonContent.Create(newUser, scimjson, jsonSerializerOptions);

            var response = await _httpClient.PostAsync("Users", content, token);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadFromJsonAsync<ScimError>(token);

                if (error != null)
                {
                    if (error.Detail.Contains(PASSWORD_POLICY_ERROR))
                    {
                        throw new PasswordPolicyException(error.Detail[(error.Detail.LastIndexOf(PASSWORD_POLICY_ERROR) + PASSWORD_POLICY_ERROR.Length)..].Trim());
                    }
                    else
                    {
                        throw new Scim2CreateUserException(error.Detail);
                    }
                }
            }

            response.EnsureSuccessStatusCode();

            var createdUser = await response.Content.ReadFromJsonAsync<CreatedUser>(token);

            if (createdUser is null)
            {
                throw new Scim2CreateUserException("New user information not found.");
            }

            return createdUser;
        }

        public async Task<string> CreateNewUserAsync(string email, string firstName, string lastName, string password, CancellationToken token)
        {
            var newUser = new NewUser()
            {
                Emails = new List<Email> { new Email { Value = email } },
                Name = new Name { FamilyName = lastName, GivenName = firstName },
                Password = password,
            };

            var user = await CreateUserAsync(newUser, token);

            return user.Username;
        }

        public async Task<bool> UserWithEmailExistsAsync(string email, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            var search = new SearchRequest
            {
                Count = 1,
                Filter = $"emails eq \"{email}\"",
            };

            var content = JsonContent.Create(search, scimjson, jsonSerializerOptions);

            var response = await _httpClient.PostAsync("Users/.search", content, token);

            response.EnsureSuccessStatusCode();

            var results = await response.Content.ReadFromJsonAsync<ListResponse>(token);

            return results?.TotalResults != 0;
        }
    }
}
