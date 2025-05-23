using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Web.Config
{
    public static class JwtConfig
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtener configuración JWT
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   ValidateLifetime = true,          // validar expiración
                   ValidIssuer = jwtSettings["Issuer"],
                   ValidAudience = jwtSettings["Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ClockSkew = TimeSpan.Zero, // Sin tolerancia para expiración
                   RoleClaimType = ClaimTypes.Role 
               };
           });
            // Configurar autenticación JWT

            services.AddAuthorization();

            return services;
        }
    }
}
