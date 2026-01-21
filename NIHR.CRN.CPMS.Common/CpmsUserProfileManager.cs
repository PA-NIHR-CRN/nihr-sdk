using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common
{
    public partial class
        CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership> : ICpmsUserProfileManager<TUserProfile>
        where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership>, new()
        where TRefPerson : class, IRefPerson, new()
        where TUserClaimMembership : class, IUserClaimMembership, new()
    {
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(1);

        private readonly ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership> _userStore;
        private readonly IOptions<AuthenticationBypassSettings>? _bypassSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership>>? _logger;

        public CpmsUserProfileManager(
            ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership> userStore,
            IOptions<AuthenticationBypassSettings>? bypassSettings,
            IMemoryCache memoryCache,
            ILogger<CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership>>? logger,
            IHostEnvironment hostEnvironment)
        {
            _userStore = userStore;
            _bypassSettings = bypassSettings;
            _memoryCache = memoryCache;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [LoggerMessage(EventId = 10001, Level = LogLevel.Warning,
            Message = "Authentication bypass can only be enabled in a development environment")]
        private partial void LogAttemptToBypassOutsideOfDev();

        [LoggerMessage(EventId = 10002, Level = LogLevel.Error,
            Message = "BypassEmail must be set when authentication bypass is enabled")]
        private partial void LogBypassEmailNotSet();

        [LoggerMessage(EventId = 10003, Level = LogLevel.Error,
            Message = "The email must be set for all requests")]
        private partial void LogEmailHeaderNotSet();

        [LoggerMessage(EventId = 10004, Level = LogLevel.Error,
            Message = "The UUID must be set for all requests")]
        private partial void LogUuidHeaderNotSet();

        public async Task<TUserProfile> FetchAndUpdateUserProfileAsync(string? email, string? uuid, 
            string? firstName, string? lastName, string? orcId)
        {
            var isDevelopmentEnvironment = _hostEnvironment.IsDevelopment();
            
            if (_bypassSettings?.Value.Bypass == true && !isDevelopmentEnvironment)
            {
                LogAttemptToBypassOutsideOfDev();
            }

            TUserProfile userProfile;

            if (isDevelopmentEnvironment && _bypassSettings?.Value.Bypass == true)
            {
                if (string.IsNullOrWhiteSpace(_bypassSettings.Value.BypassEmail))
                {
                    LogBypassEmailNotSet();
                    throw new Exception("Bypass email not set");
                }

                userProfile = await UpdateOrCreateUserProfile(_bypassSettings.Value.BypassEmail);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    LogEmailHeaderNotSet();
                    throw new Exception("Email address not set");
                }

                if (string.IsNullOrWhiteSpace(uuid))
                {
                    LogUuidHeaderNotSet();
                    throw new Exception("UUID not set");
                }

                var cacheKey = new CacheKey(uuid!);

                void UpdatePersonalDetails(TRefPerson person)
                {
                    person.FirstName = firstName ?? string.Empty;
                    person.LastName = lastName ?? string.Empty;
                    person.OrcId = orcId ?? string.Empty;
                }

                // LastLogin timestamp is intentionally set only on a cache miss or on a profile change. The
                // cache ttl is 60 seconds, so the timestamp will still be updated frequently.
                if (_memoryCache.TryGetValue(cacheKey, out TUserProfile? cachedProfile))
                {
                    if (ProfileHasChanged(cachedProfile!.Person, email!, firstName, lastName, orcId))
                    {
                        // If the personal details have changed, immediately update the record and cache...
                        userProfile = await UpdateOrCreateUserProfile(email!, uuid, UpdatePersonalDetails);
                        _memoryCache.Set(cacheKey, userProfile, _cacheTtl);
                    }
                    else
                    {
                        userProfile = cachedProfile;
                    }
                }
                else
                {
                    userProfile = await UpdateOrCreateUserProfile(email!, uuid, UpdatePersonalDetails);
                    _memoryCache.Set(cacheKey, userProfile);
                }
            }

            return userProfile;
        }

        private bool ProfileHasChanged(TRefPerson person, string email, string? firstName, string? lastName,
            string? orcId)
        {
            return person.Email != email
                   || person.FirstName != firstName
                   || person.LastName != lastName
                   || person.OrcId != orcId;
        }

        private record CacheKey
        {
            public CacheKey(string uuid)
            {
                Uuid = uuid;
            }

            public string? Uuid { get; }
        }

        private async Task<TUserProfile> UpdateOrCreateUserProfile(string email, string? uuid = null,
            Action<TRefPerson>? updatePersonalDetails = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            var userProfile = string.IsNullOrWhiteSpace(uuid)
                ? null
                : await _userStore.GetUserProfileByUuidAsync(uuid!);

            if (userProfile == null)
            {
                userProfile = await _userStore.GetUserProfileByEmailAsync(email);

                if (userProfile == null)
                {
                    var person = await _userStore.GetRefPersonByEmailAsync(email);
                    userProfile = new TUserProfile
                    {
                        EmailId = email,
                        LastLogin = DateTime.Now,
                        UserId = uuid,
                        Person = person ?? new TRefPerson()
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
            
            updatePersonalDetails?.Invoke(userProfile.Person);
            userProfile.Person.Email = email;
            userProfile.LastLogin = DateTime.Now;

            await _userStore.SaveChangesAsync();

            return userProfile;
        }
    }
}