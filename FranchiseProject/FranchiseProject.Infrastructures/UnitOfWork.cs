using FranchiseProject.Application;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssignmentSubmitRepository _assignmentSubmitRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IClassRepository _classRepository;
        private readonly IClassScheduleRepository _classScheduleRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICourseCategoryRepository _courseCategoryRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IQuestionOptionRepository _questionOptionRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizDetailRepository _quizDetailRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IScoreRepository _scoreRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly IStudentAnswerRepository _studentAnswerRepository;
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly IRegisterCourseRepository _registerFormRepository;
        private readonly ISyllabusRepository _syllabusRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRegisterFormRepository _fanchiseRegistrationRequestRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICourseMaterialRepository _courseMaterialRepository;
        private readonly IRegisterCourseRepository _registerCourseRepository;
        private readonly IChapterMaterialRepository _chapterMaterialRepository;
        private readonly IAgencyDashboardRepository _agencyDashboardRepository;
        private readonly IWorkRepository _workRepository;
        private readonly IUserAppointmentRepository _userAppointmentRepository; 
        private readonly IAppointmentRepository _appointmentRepository;
        public UnitOfWork(AppDbContext appDbContext, IAgencyRepository agencyRepository, IAssignmentRepository assignmentRepository,
            IAttendanceRepository attendanceRepository, IChapterRepository chapterRepository, IClassRepository classRepository,
            IClassScheduleRepository classScheduleRepository, IContractRepository contractRepository, ICourseCategoryRepository courseCategoryRepository,
            ICourseRepository courseRepository, IFeedbackRepository feedbackRepository, IQuestionOptionRepository questionOptionRepository,
            IQuestionRepository questionRepository, IQuizDetailRepository quizDetailRepository, IQuizRepository  quizRepository, IReportRepository reportRepository,
            IScoreRepository scoreRepository, ISessionRepository sessionRepository, ISlotRepository slotRepository, IStudentAnswerRepository studentAnswerRepository,
            IClassRoomRepository classRoomRepository, IRegisterCourseRepository registerFormRepository, ISyllabusRepository syllabusRepository,
            IUserRepository userRepository, IAssignmentSubmitRepository assignmentSubmitRepository, IRefreshTokenRepository refreshTokenRepository,
            IRegisterFormRepository franchiseRegistrationRequestRepository, IAssessmentRepository assessmentRepository,INotificationRepository notificationRepository,
            IPaymentRepository paymentRepository,IStudentRepository studentRepository,
            ICourseMaterialRepository courseMaterialRepository,IRegisterCourseRepository registerCourseRepository,
            IChapterMaterialRepository chapterMaterialRepository,IAgencyDashboardRepository agencyDashboardRepository,
            IAppointmentRepository appointmentRepository, IWorkRepository workRepository,
            IUserAppointmentRepository userAppointmentRepository)

           
        {
            _dbContext = appDbContext;
            _agencyRepository = agencyRepository;
            _assignmentRepository = assignmentRepository;   
            _assignmentSubmitRepository = assignmentSubmitRepository;
            _attendanceRepository = attendanceRepository;
            _chapterRepository = chapterRepository;
            _classRepository = classRepository;
            _classScheduleRepository = classScheduleRepository;
            _contractRepository = contractRepository;
            _courseCategoryRepository = courseCategoryRepository;
            _courseRepository = courseRepository;
            _feedbackRepository = feedbackRepository;
            _questionRepository = questionRepository;
            _questionOptionRepository = questionOptionRepository;
            _quizDetailRepository = quizDetailRepository;
            _quizRepository = quizRepository;
            _reportRepository = reportRepository;
            _scoreRepository = scoreRepository;
            _sessionRepository = sessionRepository;
            _slotRepository = slotRepository;
            _studentAnswerRepository = studentAnswerRepository;
            _classRoomRepository = classRoomRepository;
            _registerFormRepository = registerFormRepository;
            _syllabusRepository = syllabusRepository;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _fanchiseRegistrationRequestRepository = franchiseRegistrationRequestRepository;
            _assessmentRepository = assessmentRepository;
            _notificationRepository = notificationRepository;
            _paymentRepository = paymentRepository;
            _studentRepository = studentRepository;
            _registerCourseRepository= registerCourseRepository;
            _chapterMaterialRepository= chapterMaterialRepository;
            _courseMaterialRepository = courseMaterialRepository;
            _agencyDashboardRepository = agencyDashboardRepository;
            _appointmentRepository = appointmentRepository;
            _userAppointmentRepository = userAppointmentRepository;
            _workRepository = workRepository;
        }
        public IAgencyRepository AgencyRepository => _agencyRepository;

        public IAssignmentRepository AssignmentRepository => _assignmentRepository;

        public IAssignmentSubmitRepository AssignmentSubmitRepository => _assignmentSubmitRepository;

        public IAttendanceRepository AttendanceRepository => _attendanceRepository;

        public IChapterRepository ChapterRepository => _chapterRepository;

        public IClassRepository ClassRepository => _classRepository;

        public IClassScheduleRepository ClassScheduleRepository => _classScheduleRepository;

        public IContractRepository ContractRepository => _contractRepository;

        public ICourseCategoryRepository CourseCategoryRepository => _courseCategoryRepository;

        public ICourseRepository CourseRepository => _courseRepository;

        public IFeedbackRepository FeedbackRepository => _feedbackRepository;

        public IQuestionOptionRepository QuestionOptionRepository => _questionOptionRepository;

        public IQuestionRepository QuestionRepository => _questionRepository;

        public IQuizDetailRepository QuizDetailRepository => _quizDetailRepository;

        public IQuizRepository QuizRepository => _quizRepository;

        public IReportRepository ReportRepository => _reportRepository;

        public IScoreRepository ScoreRepository => _scoreRepository;

        public ISessionRepository SessionRepository => _sessionRepository;

        public ISlotRepository SlotRepository => _slotRepository;

        public IStudentAnswerRepository StudentAnswerRepository => _studentAnswerRepository;

        public IClassRoomRepository ClassRoomRepository => _classRoomRepository;

        public IRegisterCourseRepository RegisterFormRepository => _registerFormRepository;

        public ISyllabusRepository SyllabusRepository => _syllabusRepository;

        public IUserRepository UserRepository => _userRepository;

        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository;

        public IRegisterFormRepository FranchiseRegistrationRequestRepository=> _fanchiseRegistrationRequestRepository;

        public INotificationRepository NotificationRepository => _notificationRepository;
        
        public IAssessmentRepository AssessmentRepository => _assessmentRepository;

        public IStudentRepository StudentRepository => _studentRepository;

        public IPaymentRepository PaymentRepository=>_paymentRepository;

        public ICourseMaterialRepository CourseMaterialRepository => _courseMaterialRepository;

        public IRegisterCourseRepository RegisterCourseRepository => _registerCourseRepository;

        public IChapterMaterialRepository ChapterMaterialRepository => _chapterMaterialRepository;

        public IAgencyDashboardRepository AgencyDashboardRepository => _agencyDashboardRepository;

        public IUserAppointmentRepository UserAppointmentRepository => _userAppointmentRepository;

        public IWorkRepository WorkRepository => _workRepository;   
        public IAppointmentRepository AppointmentRepository => _appointmentRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
