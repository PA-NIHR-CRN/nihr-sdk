using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common
{
    public partial class
        CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership, TAcl> : ICpmsUserProfileManager<
        TUserProfile>
        where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership, TAcl>, new()
        where TRefPerson : class, IRefPerson<TAcl>, new()
        where TUserClaimMembership : class, IUserClaimMembership, new()
        where TAcl : IAcl, new()
    {
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(1);

        private readonly ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership, TAcl> _userStore;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeProvider _timeProvider;

        private readonly ILogger<CpmsUserProfileManager<TUserProfile, TRefPerson,
            TUserClaimMembership, TAcl>>? _logger;

        public CpmsUserProfileManager(
            ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership, TAcl> userStore,
            IMemoryCache memoryCache,
            ILogger<CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership, TAcl>>? logger,
            TimeProvider timeProvider)
        {
            _userStore = userStore;
            _memoryCache = memoryCache;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        private const long SystemUserId = 1;

        [LoggerMessage(EventId = 10005, Level = LogLevel.Warning,
            Message = "The first name extended attribute is null or empty")]
        private partial void LogFirstNameNotSet();

        [LoggerMessage(EventId = 10006, Level = LogLevel.Warning,
            Message = "The last name extended attribute is null or empty")]
        private partial void LogLastNameNotSet();

        [LoggerMessage(EventId = 10003, Level = LogLevel.Error,
            Message = "The email must be set for all requests")]
        private partial void LogEmailHeaderNotSet();

        [LoggerMessage(EventId = 10004, Level = LogLevel.Error,
            Message = "The UUID must be set for all requests")]
        private partial void LogUuidHeaderNotSet();


        public virtual async Task<TUserProfile> FetchAndUpdateUserProfileAsync(string? email, string? uuid,
            ExtendedUserAttributes? extendedUserAttributes)
        {
            TUserProfile result;

            if (string.IsNullOrWhiteSpace(email))
            {
                LogEmailHeaderNotSet();
                throw new ArgumentException("Email address not set", nameof(email));
            }

            if (string.IsNullOrWhiteSpace(uuid))
            {
                LogUuidHeaderNotSet();
                throw new ArgumentException("UUID not set", nameof(uuid));
            }

            var cacheKey = new CacheKey(uuid!);

            // LastLogin timestamp is intentionally set only on a cache miss or on a profile change. The
            // cache ttl is 60 seconds, so the timestamp will still be updated frequently.
            if (_memoryCache.TryGetValue(cacheKey, out TUserProfile? cachedProfile))
            {
                if (ProfileHasChanged(cachedProfile!.Person, email!, extendedUserAttributes))
                {
                    // If the personal details have changed, immediately update the record and cache...
                    result = await UpdateOrCreateUserProfile(email!, uuid, extendedUserAttributes);
                    _memoryCache.Set(cacheKey, result, _cacheTtl);
                }
                else
                {
                    result = cachedProfile;
                }
            }
            else
            {
                result = await UpdateOrCreateUserProfile(email!, uuid, extendedUserAttributes);
                _memoryCache.Set(cacheKey, result, _cacheTtl);
            }


            return result;
        }

        private bool ProfileHasChanged(TRefPerson person, string email, ExtendedUserAttributes? extendedUserAttributes)
        {
            return person.Email != email ||
                   (extendedUserAttributes != null &&
                    (person.FirstName != extendedUserAttributes.FirstName
                     || person.LastName != extendedUserAttributes.LastName
                     || person.OrcId != extendedUserAttributes.Orcid));
        }

        private record CacheKey
        {
            public CacheKey(string uuid)
            {
                Uuid = uuid;
            }

            public string? Uuid { get; }
        }

        protected async Task<TUserProfile> UpdateOrCreateUserProfile(string email, string? uuid = null,
            ExtendedUserAttributes? extendedUserAttributes = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            var userProfile = string.IsNullOrWhiteSpace(uuid)
                ? null
                : await _userStore.GetSingleOrDefaultUserProfileByUuidAsync(uuid!);

            if (userProfile == null)
            {
                userProfile = await _userStore.GetFirstOrDefaultUserProfileByEmailAsync(email);

                if (userProfile == null)
                {
                    var person = await _userStore.GetSingleOrDefaultRefPersonByEmailAsync(email);
                    userProfile = new TUserProfile
                    {
                        EmailId = email,
                        LastLogin = DateTime.Now,
                        UserId = uuid,
                        Active = true,

                        Person = person ?? new TRefPerson
                        {
                            Active = true,
                            CreatedDate = _timeProvider.GetLocalNow().DateTime,
                            ModifiedDate = _timeProvider.GetLocalNow().DateTime,
                            CreatedBy = SystemUserId,
                            ModifiedBy = SystemUserId,
                            Acl = new TAcl
                            {
                                LastUpdatedDate = _timeProvider.GetLocalNow().DateTime,
                                CreatedDate = _timeProvider.GetLocalNow().DateTime
                            }
                        }
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
                    ClaimTypeId = (long)ClaimTypes.PublicUser,
                    CreatedDate = _timeProvider.GetLocalNow().DateTime,
                    Active = true,
                });
            }

            if (extendedUserAttributes != null)
            {
                if (string.IsNullOrWhiteSpace(extendedUserAttributes.FirstName))
                {
                    LogFirstNameNotSet();
                }
                else
                {
                    userProfile.Person.FirstName = extendedUserAttributes.FirstName;
                }

                if (string.IsNullOrWhiteSpace(extendedUserAttributes.LastName))
                {
                    LogLastNameNotSet();
                }
                else
                {
                    userProfile.Person.LastName = extendedUserAttributes.LastName;
                }

                // OrcId is optional
                userProfile.Person.OrcId = extendedUserAttributes.Orcid ?? string.Empty;
            }

            userProfile.Person.Email = email;
            userProfile.LastLogin = _timeProvider.GetLocalNow().DateTime;

            await _userStore.SaveChangesAsync();

            return userProfile;
        }
    }
}