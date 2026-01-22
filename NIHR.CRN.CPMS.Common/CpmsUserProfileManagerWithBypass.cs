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
        CpmsUserProfileManagerWithBypass<TUserProfile, TRefPerson, TUserClaimMembership> : 
        CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership>
        where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership>, new()
        where TRefPerson : class, IRefPerson, new()
        where TUserClaimMembership : class, IUserClaimMembership, new()
    {
        private readonly IOptions<AuthenticationBypassSettings>? _bypassSettings;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership>>? _logger;

        public CpmsUserProfileManagerWithBypass(
            ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership> userStore,
            IOptions<AuthenticationBypassSettings>? bypassSettings,
            IMemoryCache memoryCache,
            ILogger<CpmsUserProfileManagerWithBypass<TUserProfile, TRefPerson, TUserClaimMembership>>? logger,
            IHostEnvironment hostEnvironment,
            TimeProvider timeProvider)
        : base(userStore, memoryCache, logger, timeProvider)
        {
            _bypassSettings = bypassSettings;
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

        public async override Task<TUserProfile> FetchAndUpdateUserProfileAsync(string? email, string? uuid, 
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
                return await base.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
            }

            return userProfile;
        }
    }
}