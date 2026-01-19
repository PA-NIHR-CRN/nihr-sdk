using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common
{
    public static class DiExtensions
    {
        public static IServiceCollection AddCpmsAuthentication<TUserProfile, TRefPerson, TUserClaimMembership>
            (this IServiceCollection services)
            where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership>, new()
            where TRefPerson : class, IRefPerson, new()
            where TUserClaimMembership : class, IUserClaimMembership, new()
        {
            return services
                .AddTransient<ICpmsAuthenticator<TUserProfile>,
                    CpmsAuthenticator<TUserProfile, TRefPerson, TUserClaimMembership>>();
        }

        public static void AddAuthBypassSettings(this IServiceCollection services, IConfiguration configuration,
            string sectionName = "AuthenticationBypass")
        {
            services.Configure<AuthenticationBypassSettings>(configuration.GetSection(sectionName));
        }
    }
}