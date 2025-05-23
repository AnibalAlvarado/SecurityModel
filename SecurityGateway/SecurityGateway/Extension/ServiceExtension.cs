

using SecurityGateway.Services.Implementations;
using SecurityGateway.Services.Interfaces;

namespace Web.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {

            // Inyectar TokenService
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}