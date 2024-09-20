using FranchiseProject.API.Services;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FranchiseProject.API.Middlewares;
using FluentValidation;
using FranchiseProject.API.Validator.AgencyValidation;
using FranchiseProject.Application.ViewModels.ContractViewModel;
using FranchiseProject.API.Validator.ContractValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Infrastructures.DataInitializer;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.API.Validator.AutheticationValidator;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.API.Validator.SlotValidator;
using FranchiseProject.API.Validator.UserValidator;

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

            
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddSingleton<IFirebaseService, FirebaseService>();
            services.AddHttpContextAccessor();
            services.AddLogging();

            #region Seed
            services.AddHostedService<SetupIdentityDataSeeder>();
            services.AddScoped<RoleInitializer>();
            services.AddScoped<AccountInitializer>();
            #endregion
            #region Validator
            services.AddTransient<IValidator<RegisterConsultationViewModel>, RegisFranchiseViewModelValidator>();
            services.AddTransient<IValidator<CreateAgencyViewModel>,CreateAgencyValidator>();
            services.AddTransient<IValidator<CreateContractViewModel>, CreateContractValidator>();
            services.AddTransient<IValidator<UpdateContractViewModel>, UpdateContracValidator>();
            services.AddTransient<IValidator<UserResetPasswordModel>, UserResetPasswordValidator>();
            services.AddTransient<IValidator<CreateSlotModel>, CreateSlotValidator>();
            services.AddTransient<IValidator<CreateUserModel>,CreateUserValidator>();
            services.AddTransient<IValidator<UpdateUserModel>, UpdateUserValidator>();
            services.AddTransient<IValidator<UpdatePasswordModel>, UpdatePasswordValidator>();
            #endregion

            return services;
        }
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, AppConfiguration configuration)
        {
            /*services.AddScoped<RedisAuthenticationMiddleware>();*/
            services.AddScoped<CustomJwtBearerEvents>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration.JwtOptions.Issuer,
                    ValidAudience = configuration.JwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JwtOptions.Secret)),
                };
                options.EventsType = typeof(CustomJwtBearerEvents);

            });

            return services;
        }
    }
}
