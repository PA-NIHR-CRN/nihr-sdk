using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NIHR.CRN.CPMS.Abstractions;
using NIHR.Infrastructure.Settings;

namespace NIHR.CRN.CPMS.Common
{
    public partial class CpmsAuthenticator<TUserProfile, TRefPerson, TUserClaimMembership> : ICpmsAuthenticator<TUserProfile>
        where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership>, new()
        where TRefPerson : class, IRefPerson, new()
        where TUserClaimMembership : class, IUserClaimMembership, new()
    {
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(1);
        
        private readonly ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership> _userStore;
        private readonly IOptions<AuthenticationBypassSettings> _bypassSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CpmsAuthenticator<TUserProfile, TRefPerson, TUserClaimMembership>> _logger;

        public CpmsAuthenticator(
            ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership> userStore,
            IOptions<AuthenticationBypassSettings> bypassSettings,
            IMemoryCache memoryCache,
            ILogger<CpmsAuthenticator<TUserProfile, TRefPerson, TUserClaimMembership>> logger)
        {
            _userStore = userStore;
            _bypassSettings = bypassSettings;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [LoggerMessage(EventId = 10001, Level = LogLevel.Warning,
            Message = "Authentication bypass can only be enabled in a development environment")]
        public partial void LogAttemptToBypassOutsideOfDev();

        [LoggerMessage(EventId = 10002, Level = LogLevel.Error,
            Message = "BypassEmail must be set when authentication bypass is enabled")]
        public partial void LogBypassEmailNotSet();

        [LoggerMessage(EventId = 10003, Level = LogLevel.Error,
            Message = "The email must be set for all requests")]
        public partial void LogEmailHeaderNotSet();
        
        [LoggerMessage(EventId = 10004, Level = LogLevel.Error,
            Message = "The UUID must be set for all requests")]
        public partial void LogUuidHeaderNotSet();

        public async Task<AuthResult<TUserProfile>> SynchronizeUserProfileAsync(bool isDevelopmentEnvironment,
            string? email, string? uuid, string? firstName, string? lastName, string? orcId)
        {
            if (_bypassSettings.Value.Bypass && !isDevelopmentEnvironment)
            {
                LogAttemptToBypassOutsideOfDev();
            }

            TUserProfile userProfile;

            if (isDevelopmentEnvironment && _bypassSettings.Value.Bypass)
            {
                if (string.IsNullOrWhiteSpace(_bypassSettings.Value.BypassEmail))
                {
                    LogBypassEmailNotSet();
                    return AuthResult<TUserProfile>.Fail("Bypass email not set");
                }

                userProfile = await GetOrCreateUserProfile(_bypassSettings.Value.BypassEmail, null);
                await _userStore.SaveChangesAsync();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    LogEmailHeaderNotSet();
                    return AuthResult<TUserProfile>.Fail("Email address not set");
                }

                if (string.IsNullOrWhiteSpace(uuid))
                {
                    LogUuidHeaderNotSet();
                    return AuthResult<TUserProfile>.Fail("UUID not set");
                }

                var cacheKey = new CacheKey(uuid);
                
                // TODO: This is fairly naive, the LastLogin timestamp and the other mutable columns 
                // TODO: (email, firstName, lastName and Orcid) only get updated upon a cache miss
                // TODO: This might be acceptable if the cache ttl remains at 60 seconds, but we
                // TODO: could be a bit cleverer and write back changes, using the cache to save a DB read.
                userProfile = (await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = _cacheTtl;
                    userProfile = await GetOrCreateUserProfile(email, uuid);
                    userProfile.Person.FirstName = firstName ?? string.Empty;
                    userProfile.Person.LastName = lastName ?? string.Empty;
                    userProfile.Person.OrcId = orcId ?? string.Empty;
                    await _userStore.SaveChangesAsync();
                    return userProfile;
                }))!;
            }

            return AuthResult<TUserProfile>.Success(userProfile);
        }

        private record CacheKey
        {
            public CacheKey(string uuid)
            {
                Uuid = uuid;
            }

            public string? Uuid { get; }
        }

        private async Task<TUserProfile> GetOrCreateUserProfile(string email, string? uuid)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            var userProfile = string.IsNullOrWhiteSpace(uuid)
                ? null
                : await _userStore.GetUserProfileByUuidAsync(uuid);

            if (userProfile == null)
            {
                userProfile = await _userStore.GetUserProfileByEmailAsync(email);

                if (userProfile == null)
                {
                    userProfile = new TUserProfile
                    {
                        EmailId = email,
                        LastLogin = DateTime.Now,
                        UserId = uuid
                    };
                    _userStore.AddUserProfile(userProfile);
                }
                else if (!string.IsNullOrWhiteSpace(uuid))
                {
                    userProfile.UserId = uuid;
                }
            }
            else
            {
                userProfile.EmailId = email;
            }

            // If the user has no claims then add PublicUser
            if (userProfile.UserClaimMembership.Count == 0)
            {
                userProfile.UserClaimMembership.Add(new TUserClaimMembership
                {
                    ClaimTypeId = (long)ClaimTypes.PublicUser
                });
            }

            if (userProfile.Person == null)
            {
                var person = await _userStore.GetRefPersonByEmailAsync(userProfile.EmailId);
                userProfile.Person = person ?? new TRefPerson();
            }

            userProfile.Person.Email = email;
            userProfile.LastLogin = DateTime.Now;

            await _userStore.SaveChangesAsync();

            return userProfile;
        }
    }
}