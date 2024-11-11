using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.NotificationViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Domain.Enums;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.AgenciesViewModels;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using FranchiseProject.Application.ViewModels.ScoreViewModels;



namespace FranchiseProject.Infrastructures.Mappers
{
    public class MapperConfigurationsProfile : Profile 
    {
        public MapperConfigurationsProfile() {
            #region FranchiseRegister
            CreateMap<ConsultationViewModel, RegisterForm>();
            CreateMap<RegisterConsultationViewModel, RegisterForm>();
            CreateMap<RegisterForm, ConsultationViewModel>().ReverseMap();
            CreateMap<RegisterForm, ConsultationViewModel>()
            .ForMember(dest => dest.ConsultantUserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty));
            #endregion
            #region Agency
            CreateMap<CreateAgencyViewModel, Agency>().ReverseMap();
            CreateMap<Agency, AgencyViewModel>();
            CreateMap<Agency, AgencyAddressViewModel>()
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => $"{src.Address}, {src.Ward}, {src.District}, {src.City}"));
            #endregion
            #region Contract
            CreateMap<CreateContractViewModel, Contract>().ReverseMap();
            CreateMap<Contract, ContractViewModel>().ForMember(dest => dest.AgencyName,otp => otp.MapFrom(src=>src.Agency.Name));

            #endregion
            #region User
            CreateMap<User, UserViewModel>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src =>
            src.UserRoles == null ? null: src.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault()));
            CreateMap<CreateUserByAdminModel, User>()
            .ForMember(dest => dest.AgencyId, opt => opt
            .MapFrom(src => string.IsNullOrEmpty(src.AgencyId) ? (Guid?)null : Guid.Parse(src.AgencyId)))
            .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password));
            CreateMap<CreateUserByAdminModel, User>()
            .ForMember(dest => dest.AgencyId, opt => opt
            .MapFrom(src => string.IsNullOrEmpty(src.AgencyId) ? (Guid?)null : Guid.Parse(src.AgencyId)))
            .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password));
            CreateMap<CreateUserByAdminModel, User>()
            .ForMember(dest => dest.AgencyId, opt => opt
            .MapFrom(src => string.IsNullOrEmpty(src.AgencyId) ? (Guid?)null : Guid.Parse(src.AgencyId)))
            .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password));
            CreateMap<CreateUserByAdminModel, User>()
            .ForMember(dest => dest.AgencyId, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.AgencyId) ? (Guid?)null : Guid.Parse(src.AgencyId)))
            .ForMember(dest => dest.PasswordHash, src => src.MapFrom(x => x.Password));
            CreateMap<UpdateUserByAgencyModel, User>();
            CreateMap<CreateUserByAgencyModel, User>();
            CreateMap<User, CreateUserViewModel>();


            #endregion
            #region Slot
            CreateMap<CreateSlotModel, Slot>();
            CreateMap<Slot, SlotViewModel>();
            CreateMap<Pagination<Slot>, Pagination<SlotViewModel>>().ReverseMap();
            #endregion
            #region CourseCategory
            CreateMap<Pagination<CourseCategory>, Pagination<CourseCategoryViewModel>>();
            CreateMap<CreateCourseCategoryModel, CourseCategory>();
            CreateMap<UpdateCourseCategoryModel, CourseCategory>();
            CreateMap<CourseCategory, CourseCategoryViewModel>();
            #endregion
            #region ClassSchedule
            CreateMap<ClassSchedule, ClassScheduleViewModel>()
              .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.Name))
                 .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room)).ReverseMap();
            CreateMap<ClassSchedule, ClassScheduleViewModel>()
          .ReverseMap();

            CreateMap<Pagination<ClassSchedule>, Pagination<ClassScheduleViewModel>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            #endregion
            #region Notification
            CreateMap<Notification, NotificationViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message)).ReverseMap();
            #endregion

            #region Class
            CreateMap<CreateClassViewModel, Class>();
            CreateMap<Class, ClassViewModel>();
            CreateMap<ClassRoom, StudentClassViewModel>()
             .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.FullName))
             .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
             .ForMember(dest => dest.URLImage, opt => opt.MapFrom(src => src.User.URLImage));

            CreateMap(typeof(Pagination<>), typeof(Pagination<>));

            #endregion
            #region Material
            CreateMap<CourseMaterial, CourseMaterialViewModel>();
            CreateMap<CreateCourseMaterialModel, CourseMaterial>();
            CreateMap<UpdateCourseMaterialModel, CourseMaterial>();
            #endregion
            #region ChapterMaterial
            CreateMap<ChapterMaterial, ChapterMaterialViewModel>();
            CreateMap<CreateChapterMaterialArrangeModel, ChapterMaterial>();
            CreateMap<ChapterMaterial, ChapterMaterial>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ChapterId, opt => opt.Ignore());
            CreateMap<UpdateChapterMaterialModel, ChapterMaterial>();
            CreateMap<CreateChapterMaterialModel, ChapterMaterial>();
            #endregion
            #region Chapter
            CreateMap<Chapter, ChapterDetailViewModel>()
                .ForMember(dest => dest.ChapterMaterials, opt => opt.MapFrom(src => src.ChapterMaterials.Where(m => m.IsDeleted != true)));
            CreateMap<Chapter, ChapterViewModel>();
            CreateMap<CreateChapterModel, Chapter>();
            CreateMap<UpdateChapterModel, Chapter>();
            CreateMap<CreateChapterArrangeModel, Chapter>()
                .ForMember(dest => dest.ChapterMaterials, opt => opt.MapFrom(src => src.ChapterMaterials));
            CreateMap<CreateChapterFileModel, Chapter>()
                .ForMember(dest => dest.ChapterMaterials, opt => opt.MapFrom(src => src.ChapterMaterials))                    
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            #endregion
            #region Session
            CreateMap<Session, SessionViewModel>();
            CreateMap<CreateSessionModel, Session>();
            CreateMap<UpdateSessionModel, Session>();
            #endregion
            #region Assessment
            CreateMap<Assessment, AssessmentViewModel>();
            CreateMap<CreateAssessmentModel, Assessment>();
            CreateMap<UpdateAssessmentModel, Assessment>();
            #endregion
            #region Course
            CreateMap<Course, CourseViewModel>()
                .ForMember(dest => dest.CourseCategoryName, opt => opt
                .MapFrom(src => src.CourseCategory.Name));
            CreateMap<Course, CourseDetailViewModel>()
                .ForMember(dest => dest.CourseMaterials, opt => opt.MapFrom(src => src.CourseMaterials.Where(m => m.IsDeleted != true)))
                .ForMember(dest => dest.Sessions, opt => opt.MapFrom(src => src.Sessions.Where(m => m.IsDeleted != true)))
                .ForMember(dest => dest.Assessments, opt => opt.MapFrom(src => src.Assessments.Where(m => m.IsDeleted != true)))
                .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => src.Chapters.Where(m => m.IsDeleted != true)))
                .ForMember(dest => dest.Syllabus, opt => opt.MapFrom(src => src.Syllabus))
                .ForMember(dest => dest.CourseCategory, opt => opt.MapFrom(src => src.CourseCategory));
            CreateMap<Course, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SyllabusId, opt => opt.Ignore());
            CreateMap<Pagination<Course>, Pagination<CourseViewModel>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateCourseModel, Course>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CourseStatusEnum.Draft));
            CreateMap<UpdateCourseModel, Course>();
            CreateMap<CreateCourseMaterialArrangeModel, CourseMaterial>();
            CreateMap<CreateAssessmentArrangeModel, Assessment>();
            CreateMap<CreateSessionArrangeModel, Session>();
            CreateMap<CourseMaterial, CourseMaterial>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.CourseId, opt => opt.Ignore()); 

            CreateMap<Session, Session>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.CourseId, opt => opt.Ignore()); 

            CreateMap<Assessment, Assessment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.CourseId, opt => opt.Ignore());

            CreateMap<Chapter, Chapter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore());
            CreateMap<Question, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ChapterId, opt => opt.Ignore());
            CreateMap<QuestionOption, QuestionOption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore());
            #endregion
            #region Syllabus
            CreateMap<CreateSyllabusModel, Syllabus>();
            CreateMap<UpdateSyllabusModel, Syllabus>();
            CreateMap<Syllabus, SyllabusViewModel>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Courses.FirstOrDefault().Id)).ReverseMap();
            CreateMap<Syllabus, Syllabus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            #endregion
            #region Question
            CreateMap<CreateQuestionOptionArrangeModel, QuestionOption>();
            CreateMap<CreateQuestionArrangeModel, Question>()
                .ForMember(dest => dest.QuestionOptions, opt => opt.MapFrom(src => src.QuestionOptions));
            CreateMap<UpdateQuestionModel, Question>()
                .ForMember(dest => dest.QuestionOptions, opt => opt.Ignore());
            CreateMap<Question, QuizDetail>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id));
            #endregion
            #region QuestionOption
            CreateMap<QuestionOption, QuestionOptionViewModel>();
            CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.QuestionOptions, opt => opt.MapFrom(src => src.QuestionOptions));

            CreateMap<QuestionOption, QuestionOptionStudentViewModel>();
            CreateMap<Question, QuestionStudentViewModel>()
                .ForMember(dest => dest.QuestionOptions, opt => opt.MapFrom(src => src.QuestionOptions));
            CreateMap<QuestionOptionViewModel, Question>();
            #endregion
            #region RegisterCourse
            CreateMap<User, StudentViewModel>();
            CreateMap<Pagination<User>, Pagination<StudentViewModel>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<User, StudentViewModel>()
           .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().Course.Name));
            CreateMap<User, StudentRegisterViewModel>()
           .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<RegisterCourse, UpdateRegisterCourseViewModel>();
            #endregion
            #region Score
            CreateMap<Score, ScoreViewModel>();
            CreateMap<Score, UserScoreViewModel>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.ScoreNumber));
            #endregion
            #region Quiz
            CreateMap<CreateQuizModel, Quiz>();
            CreateMap<Quiz, QuizDetailStudentViewModel>()
                .ForMember(dest => dest.Questions, opt => opt
                .MapFrom(src => src.QuizDetails
                .Select(qd => qd.Question)));
            CreateMap<Quiz, QuizStudentViewModel>()
                .ForMember(dest => dest.Scores, opt => opt.MapFrom(src => src.Scores.FirstOrDefault()));
            
            CreateMap<Quiz, QuizViewModel>()
            .ForMember(dest => dest.UserScores, opt => opt.MapFrom(src => src.Scores));

            CreateMap<UpdateQuizModel, Quiz>();
            
            #endregion

            #region Payment
            CreateMap<CreateStudentPaymentViewModel, Payment>()
           .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Payment, StudentViewModel>();
            CreateMap<Pagination<Payment>, Pagination<StudentViewModel>>();
            CreateMap<Payment, PaymentStudentViewModel>()
           .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.FullName))
           .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.CreationDate));
            #endregion
            #region Assignment
            CreateMap<CreateAssignmentViewModel, Assignment>();
            CreateMap<Assignment, AssignmentViewModel>();
            CreateMap<List<AssignmentSubmitViewModel>, Pagination<AssignmentSubmitViewModel>>();
            CreateMap<Assignment, AsmSubmitDetailViewModel>()
           
            .ForMember(dest => dest.UserScores, opt => opt.MapFrom(src => src.AssignmentSubmits.Select(us => new UserSubmitScoreViewModel
            {
                UserId = us.UserId,
                Name = us.User.FullName,
                Username = us.User.UserName,
                Score = us.ScoreNumber,
                SubmitFileName = us.FileSbumitName,
                SubmitUrl = us.FileSubmitURL
            }).ToList())); // Chuyển đổi danh sách thành List
        }
        #endregion
    }
    }

