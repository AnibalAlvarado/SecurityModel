using Business.Implementations;
using Business.Interfaces;
using Data.Implementations;
using Data.Interfaces;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using System;
using Utilities.Implementation;
using Utilities.Interfaces;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Add services to the container.
            var cryptoApiBaseUrl = builder.Configuration["CryptoApi:BaseUrl"];

            // Registrar HttpClient para servicio externo CryptoApi
            builder.Services.AddHttpClient<ICryptoApiService, CryptoApiService>(client =>
            {
                client.BaseAddress = new Uri(cryptoApiBaseUrl);
                client.DefaultRequestHeaders.Add("accept", "application/json");
                client.DefaultRequestHeaders.Add("x-cg-demo-api-key", builder.Configuration["CryptoApi:ApiKey"]);
              
            });

            // Registro de capas Business y Data
            builder.Services.AddScoped<ICryptoBusiness, CryptoBusiness>();
            builder.Services.AddScoped<ICryptoData, CryptoData>();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Controllers
            builder.Services.AddControllers();

            // Swagger/OpenAPI
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middleware Swagger para ambiente desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Si tienes autenticación, aquí va UseAuthentication()
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
