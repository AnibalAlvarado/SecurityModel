namespace SecurityGateway.Middleware
{
    public class RequireAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequireAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Rutas sin autenticación
            if (path.StartsWith("/security/api/auth/login"))
            {
                await _next(context);
                return;
            }

            // Para todas las demás rutas /security/* requiere token
            if (path.StartsWith("/security"))
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido o no provisto");
                    return;
                }
            }

            await _next(context);
        }
    }
}
