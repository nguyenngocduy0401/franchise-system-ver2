using FranchiseProject.Application;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Jobs;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.Services;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Infrastructures.Mappers;
using FranchiseProject.Infrastructures.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string appConfiguration)
        {
            #region Service DI
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IAgencyService, AgencyService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IAssignmentSubmitService, AssignmentSubmitService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IClassScheduleService, ClassScheduleService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseCategoryService, CourseCategoryService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuestionOptionService, QuestionOptionService>();
            services.AddScoped<IQuizDetailService, QuizDetailService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISlotService, SlotService>();
            services.AddScoped<IStudentAnswerService, StudentAnswerService>();
            services.AddScoped<IClassRoomService, ClassRoomService>();
            services.AddScoped<IRegisterCourseService, RegisterCourseService>();
            services.AddScoped<ISyllabusService, SyllabusService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRegisterFormSevice,RegisterFormService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IChapterMaterialService, ChapterMaterialService>();
            services.AddScoped<ICourseMaterialService, CourseMaterialService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IAgencyDashboardService, AgencyDashboardService>();
            services.AddScoped<IWorkService, WorkService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IUserAppointmentService, UserAppointmentService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IEquipmentService,EquipmentService> ();

            #endregion
            #region Repository DI
            services.AddScoped<IAgencyRepository, AgencyRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IAssignmentSubmitRepository, AssignmentSubmitRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IClassScheduleRepository, ClassScheduleRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
            services.AddScoped<IQuizDetailRepository, QuizDetailRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IScoreRepository, ScoreRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ISlotRepository, SlotRepository>();
            services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
            services.AddScoped<IClassRoomRepository, ClassRoomRepository>();
            services.AddScoped<IRegisterCourseRepository, RegisterCourseRepository>();
            services.AddScoped<ISyllabusRepository, SyllabusRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRegisterFormRepository, RegisterFormRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
            services.AddScoped<IChapterMaterialRepository, ChapterMaterialRepository>();
            services.AddScoped<IAgencyDashboardRepository, AgencyDashboardRepository>();
            services.AddScoped<IUserAppointmentRepository, UserAppointmentRepository>();
            services.AddScoped<IWorkRepository, WorkRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            services.AddScoped<IFranchiseFeesRepository, FranchiseFeeRepository>();
            services.AddScoped<IEquipmentSerialNumberHistoryRepository, EquipmentSerialNumberHistoryRepository>();
            #endregion
            services.AddIdentity<User, Role>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Set your desired password requirements here
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6; // Set your desired minimum length
                options.Password.RequiredUniqueChars = 0; // Set your desired number of unique characters
            });
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(appConfiguration));
            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);
            return services;
        }
        public static IServiceCollection AddQuarztJobsService(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {

                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                // Just use the name of your job that you created in the Jobs folder.
                var jobKey = new JobKey("SendContractRenewalEmailJob");
                q.AddJob<SendContractRenewalEmailJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("SendContractRenewalEmailJob-trigger")
                    //This Cron interval can be described as "run every minute" (when second is zero)
                    .StartAt(currentTime.AddDays(3))
                    .WithCronSchedule("0 4 1/21 * * ?", cron => cron.InTimeZone(vietnamTimeZone))
                );
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            return services;
        }
    }
}
