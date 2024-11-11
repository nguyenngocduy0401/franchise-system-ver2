using FranchiseProject.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application
{
    public interface IUnitOfWork
    {
        public IAgencyRepository AgencyRepository { get; }
        public IAssignmentRepository AssignmentRepository { get; }
        public IAssignmentSubmitRepository AssignmentSubmitRepository { get; }
        public IAttendanceRepository AttendanceRepository { get; }
        public IChapterRepository ChapterRepository { get; }
        public IClassRepository ClassRepository { get; }
        public IClassScheduleRepository ClassScheduleRepository { get; }
        public IContractRepository ContractRepository { get; }
        public ICourseCategoryRepository CourseCategoryRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IFeedbackRepository FeedbackRepository { get; }
        public IQuestionOptionRepository QuestionOptionRepository { get; }
        public IQuestionRepository QuestionRepository { get; }
        public IQuizDetailRepository QuizDetailRepository { get; }
        public IQuizRepository QuizRepository { get; }
        public IReportRepository ReportRepository { get; }
        public IScoreRepository ScoreRepository { get; }
        public ISessionRepository SessionRepository { get; }
        public ISlotRepository SlotRepository { get; }
        public IStudentAnswerRepository StudentAnswerRepository { get; }
        public IClassRoomRepository ClassRoomRepository { get; }
        public IRegisterCourseRepository RegisterCourseRepository { get; }
        public ISyllabusRepository SyllabusRepository { get; }
        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IRegisterFormRepository FranchiseRegistrationRequestRepository { get; }
        public IAssessmentRepository AssessmentRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public IStudentRepository StudentRepository { get; }
        public ICourseMaterialRepository CourseMaterialRepository { get; }
        public IChapterMaterialRepository ChapterMaterialRepository { get; }
        public IAgencyDashboardRepository AgencyDashboardRepository { get; }
        public IUserAppointmentRepository UserAppointmentRepository { get; }
        public IWorkRepository WorkRepository { get; }  
        public IAppointmentRepository AppointmentRepository { get; }
        public Task<int> SaveChangeAsync();
    }
}
