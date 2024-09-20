using FranchiseProject.Application;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.Services;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Infrastructures.Mappers;
using FranchiseProject.Infrastructures.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IFeedbackAnswerService, FeedbackAnswerService>();
            services.AddScoped<IFeedbackOptionService, FeedbackOptionService>();
            services.AddScoped<IFeedbackQuestionService, FeedbackQuestionService>();
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
            services.AddScoped<IStudentClassService, StudentClassService>();
            services.AddScoped<IStudentCourseService, StudentCourseService>();
            services.AddScoped<ISyllabusService, SyllabusService>();
            services.AddScoped<ITermService, TermService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IConsultationService,ConsultationService>();
            services.AddScoped<IEmailService, EmailService>();
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
            services.AddScoped<IFeedbackAnswerRepository, FeedbackAnswerRepository>();
            services.AddScoped<IFeedbackOptionRepository, FeedbackOptionRepository>();
            services.AddScoped<IFeedbackQuestionRepository, FeedbackQuestionRepository>();
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
            services.AddScoped<IStudentClassRepository, StudentClassRepository>();
            services.AddScoped<IStudentCourseRepository, StudentCourseRepository>();
            services.AddScoped<ISyllabusRepository, SyllabusRepository>();
            services.AddScoped<ITermRepository, TermRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConsultationRepository, ConsultationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
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
    }
}
