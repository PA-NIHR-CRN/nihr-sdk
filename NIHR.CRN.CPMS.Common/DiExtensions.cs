using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common
{
    public static class DiExtensions
    {
        public static IServiceCollection AddCpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership, TAcl>
            (this IServiceCollection services, bool enableBypass)
            where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership, TAcl>, new()
            where TRefPerson : class, IRefPerson<TAcl>, new()
            where TUserClaimMembership : class, IUserClaimMembership, new()
            where TAcl : new()
        {
            if (enableBypass)
            {
                return services
                    .AddTransient<ICpmsUserProfileManager<TUserProfile>,
                        CpmsUserProfileManagerWithBypass<TUserProfile, TRefPerson, TUserClaimMembership, TAcl>>();
            }
            else
            {
                return services
                    .AddTransient<ICpmsUserProfileManager<TUserProfile>,
                        CpmsUserProfileManager<TUserProfile, TRefPerson, TUserClaimMembership, TAcl>>();
            }
        }

        public static void AddAuthBypassSettings(this IServiceCollection services, IConfiguration configuration,
            string sectionName = "AuthenticationBypass")
        {
            services.Configure<AuthenticationBypassSettings>(configuration.GetSection(sectionName));
        }
    }
}