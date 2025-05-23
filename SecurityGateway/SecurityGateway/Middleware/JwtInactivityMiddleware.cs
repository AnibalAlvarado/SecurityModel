using System.Collections.Concurrent;

namespace SecurityGateway.Middleware
{
    public class JwtInactivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _inactivityMinutes;

        // Para demo simple, usamos un diccionario thread-safe en memoria
        private static ConcurrentDictionary<string, DateTime> _lastAccessTracker = new();

        public JwtInactivityMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _inactivityMinutes = config.GetValue<int>("JwtSettings:TokenInactivityMinutes");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    var now = DateTime.UtcNow;

                    if (_lastAccessTracker.TryGetValue(token, out var lastAccess))
                    {
                        if ((now - lastAccess).TotalMinutes > _inactivityMinutes)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Token expired due to inactivity.");
                            return;
                        }
                        else
                        {
                            _lastAccessTracker[token] = now; // Actualizar último acceso
                        }
                    }
                    else
                    {
                        _lastAccessTracker[token] = now; // Primer acceso
                    }
                }
            }

            await _next(context);
        }
    }
}
