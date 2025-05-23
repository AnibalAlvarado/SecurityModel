using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityGateway.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SecurityGateway.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;

        public JwtValidationMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var requireAuth = endpoint?.Metadata.GetMetadata<RequireAuthenticationMetadata>()?.RequireAuthentication ?? false;

            // Si la ruta no requiere autenticación, continuar sin validar token
            if (!requireAuth)
            {
                await _next(context);
                return;
            }

            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authorization header missing or invalid");
                return;
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Validar tiempo de inactividad (ejemplo, claim "lastActivity")
                var jwtToken = (JwtSecurityToken)validatedToken;
                var lastActivityClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "lastActivity")?.Value;

                if (lastActivityClaim != null)
                {
                    if (DateTime.TryParse(lastActivityClaim, out DateTime lastActivity))
                    {
                        var inactivityLimit = TimeSpan.FromMinutes(_jwtSettings.TokenInactivityMinutes);
                        if (DateTime.UtcNow - lastActivity > inactivityLimit)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Token expired due to inactivity");
                            return;
                        }
                    }
                }

                // Aquí puedes actualizar el claim de actividad si quieres (no cubierto en este middleware)

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Invalid token: {ex.Message}");
            }
        }
    }
}
