using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common
{
    public static class DiExtensions
    {
        public static IServiceCollection AddCpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership>
            (this IServiceCollection services, bool enableBypass)
            where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership>, new()
            where TRefPerson : class, IRefPerson, new()
            where TUserClaimMembership : class, IUserClaimMembership, new()
        {
            if (enableBypass)
            {
                return services
                    .AddTransient<ICpmsUserProfileManager<TUserProfile>,
                        CpmsUserProfileManagerWithBypass<TUserProfile, TRefPerson, TUserClaimMembership>>();
            }
            else
            {
                return services
                    .AddTransient<ICpmsUserProfileManager<TUserProfile>,
                        CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership>>();
            }
        }

        public static void AddAuthBypassSettings(this IServiceCollection services, IConfiguration configuration,
            string sectionName = "AuthenticationBypass")
        {
            services.Configure<AuthenticationBypassSettings>(configuration.GetSection(sectionName));
        }
    }
}