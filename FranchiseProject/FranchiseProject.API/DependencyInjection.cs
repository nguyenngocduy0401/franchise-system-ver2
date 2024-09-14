using FranchiseProject.API.Services;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application;
using FranchiseProject.Infrastructures;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace FranchiseProject.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "CavisAPI", Version = "v1" });
                option.EnableAnnotations();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
           /* services.AddSingleton<PerformanceMiddleware>();*/
            services.AddSingleton<Stopwatch>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddHealthChecks();
            services.AddSingleton<Stopwatch>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddHttpContextAccessor();
  /*          services.AddHostedService<SetupIdentityDataSeeder>();*/
            services.AddLogging();

            #region Seed
            /*services.AddHostedService<SetupIdentityDataSeeder>();*/
            /*services.AddScoped<RoleInitializer>();
            services.AddScoped<AccountInitializer>();*/
            #endregion
            #region Validator
            
            #endregion

            return services;
        }
    }
}