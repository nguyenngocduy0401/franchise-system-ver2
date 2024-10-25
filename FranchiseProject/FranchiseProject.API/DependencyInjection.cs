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
using FranchiseProject.Application.ViewModels.ContractViewModels;
using FranchiseProject.API.Validator.ContractValidator;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Infrastructures.DataInitializer;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.API.Validator.AutheticationValidator;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.API.Validator.SlotValidator;
using FranchiseProject.API.Validator.UserValidator;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.API.Validator.CourseCategoryValidator;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.API.Validator.ClassScheduleValidator;
using Microsoft.AspNetCore.SignalR;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.API.Validator.StudentValidator;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.API.Validator.CourseMaterialValidator;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.API.Validator.ChapterValidator;
using FranchiseProject.API.Validator.SessionValidator;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using FranchiseProject.API.Validator.SyllabusValidator;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.API.Validator.AssessmentValidator;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.API.Validator.CourseValidator;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.API.Validator.PaymentValidator;
using Google;


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
            services.AddSingleton<PerformanceMiddleware>();
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
            services.AddTransient<IValidator<RegisterConsultationViewModel>, RegisterFranchiseViewModelValidator>();
            services.AddTransient<IValidator<CreateAgencyViewModel>,CreateAgencyValidator>();
            services.AddTransient<IValidator<CreateContractViewModel>, CreateContractValidator>();
            services.AddTransient<IValidator<UpdateContractViewModel>, UpdateContracValidator>();
            services.AddTransient<IValidator<UserResetPasswordModel>, UserResetPasswordValidator>();
            services.AddTransient<IValidator<CreateSlotModel>, CreateSlotValidator>();
            services.AddTransient<IValidator<CreateUserByAdminModel>,CreateUserValidator>();
            services.AddTransient<IValidator<UpdateUserByAdminModel>, UpdateUserValidator>();
            services.AddTransient<IValidator<UpdatePasswordModel>, UpdatePasswordValidator>();
            services.AddTransient<IValidator<UpdateUserByAgencyModel>, UpdateUserByAgencyValidator>();
            services.AddTransient<IValidator<CreateUserByAgencyModel>, CreateUserByAgencyValidator>();
            services.AddTransient<IValidator<CreateCourseCategoryModel>, CreateCourseCategoryValidator>();
            services.AddTransient<IValidator<UpdateCourseCategoryModel>, UpdateCourseCategoryValidator>();
            services.AddTransient<IValidator<CreateClassScheduleViewModel>, CreateClassScheduleValidor>();
            services.AddTransient<IValidator<CreateClassScheduleDateRangeViewModel>, CreateClassScheduleDateRangeValidator>();
            services.AddTransient<IValidator<CreateStudentPaymentViewModel>, PaymentStudentValidator>();
            services.AddTransient<IValidator<RegisterCourseViewModel>, RegisterCourseValidator>();
            services.AddTransient<IValidator<CreateCourseMaterialModel>, CreateCourseMaterialValidator>();
            services.AddTransient<IValidator<UpdateCourseMaterialModel>, UpdateCourseMaterialValidator>();
            services.AddTransient<IValidator<CreateChapterModel>, CreateChapterValidator>();
            services.AddTransient<IValidator<UpdateChapterModel>, UpdateChapterValidator>();
            services.AddTransient<IValidator<CreateSessionModel>, CreateSessionValidator>();
            services.AddTransient<IValidator<UpdateSessionModel>, UpdateSessionValidator>();
            services.AddTransient<IValidator<CreateSyllabusModel>, CreateSyllabusValidator>();
            services.AddTransient<IValidator<UpdateSyllabusModel>, UpdateSyllabusValidator>();
            services.AddTransient<IValidator<CreateAssessmentModel>, CreateAssessmentValidator>();
            services.AddTransient<IValidator<UpdateAssessmentModel>, UpdateAssessmentValidator>();
            services.AddTransient<IValidator<CreateCourseModel>, CreateCourseValidator>();
            services.AddTransient<IValidator<UpdateCourseModel>, UpdateCourseValidator>();
            services.AddTransient<IValidator<CreateCourseMaterialArrangeModel>, SingleCourseMaterialArrangeValidator>();
            services.AddTransient<IValidator<List<CreateCourseMaterialArrangeModel>>, CreateCourseMaterialArrangeValidator>();
            services.AddTransient<IValidator<CreateAssessmentArrangeModel>, SingleAssessmentArrangeValidator>();
            services.AddTransient<IValidator<List<CreateAssessmentArrangeModel>>, CreateAssessmentArrangeValidator>();
            services.AddTransient<IValidator<CreateChapterArrangeModel>, SingleChapterArrangeValidator>();
            services.AddTransient<IValidator<List<CreateChapterArrangeModel>>, CreateChapterArrangeValidator>();
            services.AddTransient<IValidator<CreateSessionArrangeModel>, SingleSessionArrangeValidator>();
            services.AddTransient<IValidator<List<CreateSessionArrangeModel>>, CreateSessionArrangeValidator>();

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
