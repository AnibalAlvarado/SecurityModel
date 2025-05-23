namespace SecurityGateway.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtValidation(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequireAuthenticationMiddleware>();
            app.UseMiddleware<JwtInactivityMiddleware>(); // validar inactividad
            return app.UseMiddleware<JwtValidationMiddleware>();

        }
    }
}
