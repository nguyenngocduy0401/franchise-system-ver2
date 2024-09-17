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
        private readonly IFeedbackAnswerRepository _feedbackAnswerRepository;
        private readonly IFeedbackOptionRepository _feedbackOptionRepository;
        private readonly IFeedbackQuestionRepository _feedbackQuestionRepository;
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
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly ISyllabusRepository _syllabusRepository;
        private readonly ITermRepository _termRepository;
        private readonly IUserRepository _userRepository;
        public UnitOfWork(AppDbContext appDbContext, IAgencyRepository agencyRepository, IAssignmentRepository assignmentRepository,
            IAttendanceRepository attendanceRepository, IChapterRepository chapterRepository, IClassRepository classRepository,
            IClassScheduleRepository classScheduleRepository, IContractRepository contractRepository, ICourseCategoryRepository courseCategoryRepository,
            ICourseRepository courseRepository, IFeedbackAnswerRepository feedbackAnswerRepository,  IFeedbackOptionRepository feedbackOptionRepository,
            IFeedbackQuestionRepository feedbackQuestionRepository, IFeedbackRepository feedbackRepository, IQuestionOptionRepository questionOptionRepository,
            IQuestionRepository questionRepository, IQuizDetailRepository quizDetailRepository, IQuizRepository  quizRepository, IReportRepository reportRepository,
            IScoreRepository scoreRepository, ISessionRepository sessionRepository, ISlotRepository slotRepository, IStudentAnswerRepository studentAnswerRepository,
            IStudentClassRepository studentClassRepository, IStudentCourseRepository studentCourseRepository, ISyllabusRepository syllabusRepository,
            ITermRepository termRepository, IUserRepository userRepository, IAssignmentSubmitRepository assignmentSubmitRepository)
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
            _feedbackAnswerRepository = feedbackAnswerRepository;
            _feedbackOptionRepository = feedbackOptionRepository;
            _feedbackQuestionRepository = feedbackQuestionRepository;
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
            _studentClassRepository = studentClassRepository;
            _studentCourseRepository = studentCourseRepository;
            _syllabusRepository = syllabusRepository;
            _termRepository = termRepository;
            _userRepository = userRepository;
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

        public IFeedbackAnswerRepository FeedbackAnswerRepository => _feedbackAnswerRepository;

        public IFeedbackOptionRepository FeedbackOptionRepository => _feedbackOptionRepository;

        public IFeedbackQuestionRepository FeedbackQuestionRepository => _feedbackQuestionRepository;

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

        public IStudentClassRepository StudentClassRepository => _studentClassRepository;

        public IStudentCourseRepository StudentCourseRepository => _studentCourseRepository;

        public ISyllabusRepository SyllabusRepository => _syllabusRepository;

        public ITermRepository TermRepository => _termRepository;

        public IUserRepository UserRepository => _userRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
