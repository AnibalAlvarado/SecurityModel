
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using SecurityGateway.Middleware;
using SecurityGateway.Services.Implementations;
using SecurityGateway.Services.Interfaces;
using System.Text;
using Web.Config;
using Web.Extensions;

namespace SecurityGateway
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            //swager
            builder.Services.AddCustomSwagger();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Repositorios y servicios
            builder.Services.AddAppServices();
            // Cargar configuración JwtSettings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // JWT Authentication
            builder.Services.AddJwtAuthentication(builder.Configuration);
          

            // Configurar YARP
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            string securityServiceUrl = builder.Configuration.GetValue<string>("SecurityService:BaseUrl");
            builder.Services.AddHttpClient("securityService", client =>
            {
                client.BaseAddress = new Uri(securityServiceUrl); // URL del microservicio de seguridad
            });



            var app = builder.Build();
            app.UseHttpsRedirection();
            // Middleware para autenticación e inactividad (debes crear esta clase JwtInactivityMiddleware)
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequireAuthenticationMiddleware>(); // Usa aquí tu middleware personalizado

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Security API v1");
                options.DocumentTitle = "Security API Docs";
                options.DefaultModelsExpandDepth(-1); // Ocultar esquema de modelos por defecto
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                app.MapOpenApi();
            }

            app.UseJwtValidation(); 
            // Middleware personalizado JWT
            app.MapReverseProxy();
            app.MapControllers();

            app.MapGet("/", () => "API Gateway corriendo");

            app.Run();
        }
    }
}
