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
        public IFeedbackAnswerRepository FeedbackAnswerRepository { get; }
        public IFeedbackOptionRepository FeedbackOptionRepository { get; }  
        public IFeedbackQuestionRepository FeedbackQuestionRepository { get; }
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
        public IStudentClassRepository StudentClassRepository { get; }
        public IStudentCourseRepository StudentCourseRepository { get; }
        public ISyllabusRepository SyllabusRepository { get; }
        public ITermRepository TermRepository { get; }
        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IConsultationRepository FranchiseRegistrationRequestRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        public Task<int> SaveChangeAsync();
    }
}
