using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FranchiseProject.Domain.Enums;
using FranchiseProject.Application.ViewModels.UserViewModels;
using DocumentFormat.OpenXml;
using FranchiseProject.Application.ViewModels.ClassViewModel;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ICourseMaterialService _materialService;
        private readonly IAssessmentService _assessmentService;
        private readonly IChapterService _chapterService;
        private readonly IQuestionService _questionService;
        private readonly IClassService _classService;
        private readonly IUserChapterMaterialService _userChapterMaterialService;
        public CourseController(ICourseService courseService, ICourseMaterialService materialService,
            IAssessmentService assessmentService, 
            IChapterService chapterService, IQuestionService questionService,
            IClassService classService, IUserChapterMaterialService userChapterMaterialService)
        {
            _courseService = courseService;
            _materialService = materialService;
            _assessmentService = assessmentService;
            _chapterService = chapterService;
            _questionService = questionService;
            _classService = classService;
            _userChapterMaterialService = userChapterMaterialService;
        }
        [Authorize()]
        [SwaggerOperation(Summary = "Lấy tỉ lệ hoàn thành khóa học  {Authorize}")]
        [HttpGet("{courseId}/users/{userId}/percents")]
        public async Task<ApiResponse<double>> GetCompletedPercentCourseAsync(string userId, Guid courseId)
        {
            return await _userChapterMaterialService.GetCompletedPercentCourseAsync(userId, courseId);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật trạng thái của học bằng id {Authorize = SystemInstructor, Manager}")]
        [HttpPut("{id}/status")]
        public async Task<ApiResponse<bool>> UpdateCourseStatusAsync(Guid id, CourseStatusEnum courseStatusEnum)
        {
            return await _courseService.UpdateCourseStatusAsync(id, courseStatusEnum);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa khoá học bằng id {Authorize = SystemInstructor, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteCourseAsync(Guid id)
        {
            return await _courseService.DeleteCourseByIdAsync(id);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "dupliate khoá học bằng id {Authorize = SystemInstructor, Manager}")]
        [HttpPost("{id}/versions")]
        public async Task<ApiResponse<CourseDetailViewModel>> DuplicateCourseAsync(Guid id)
        {
            return await _courseService.CreateCourseVersionAsync(id);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo mới khoá học {Authorize = SystemInstructor, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateCourseAsync(CreateCourseModel createCourseModel)
        {
            return await _courseService.CreateCourseAsync(createCourseModel);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật khoá học {Authorize = SystemInstructor, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateCourseAsync(Guid id, UpdateCourseModel updateCourseModel)
        {
            return await _courseService.UpdateCourseAsync(id, updateCourseModel);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật tài nguyên của khoá học {Authorize = SystemInstructor, Manager}")]
        [HttpPost("{id}/course-materials")]
        public async Task<ApiResponse<bool>> CreateMaterialByCourseIdAsync(Guid id, List<CreateCourseMaterialArrangeModel> createMaterialArrangeModel)
        {
            return await _materialService.CreateMaterialArrangeAsync(id, createMaterialArrangeModel);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật đánh giá của khoá học {Authorize = SystemInstructor, Manager}")]
        [HttpPost("{id}/assessments")]
        public async Task<ApiResponse<bool>> CreateAssessmentByCourseIdAsync(Guid id, List<CreateAssessmentArrangeModel> createAssessmentArrangeModel)
        {
            return await _assessmentService.CreateAssessmentArangeAsync(id, createAssessmentArrangeModel);
        }
        [SwaggerOperation(Summary = "tìm khoá học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<CourseDetailViewModel>> GetCourseByIdAsync(Guid id)
        {
            return await _courseService.GetCourseByIdAsync(id);
        }
        [SwaggerOperation(Summary = "tìm kiếm khoá học")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<CourseViewModel>>> FilterCourseAsync([FromQuery] FilterCourseModel filterCourseModel)
        {
            return await _courseService.FilterCourseAsync(filterCourseModel);
        }
        [SwaggerOperation(Summary = "tạo chương trình học bằng file {Authorize = SystemIntructor, Manager}")]
        [HttpPost("files")]
        public async Task<ApiResponse<bool>> CreateCourseByFileAsync(CourseFilesModel file)
            => await _courseService.CreateCourseByFileAsync(file);
		[SwaggerOperation(Summary = "Lấy tất cả khóa học khả dụng")]
		[HttpGet("available")]
		public async Task<ApiResponse<IEnumerable<CourseViewModel>>> GetAllCoursesAvailableAsync()
		{
			return await _courseService.GetAllCoursesAvailableAsync();
		}
	
        [SwaggerOperation(Summary = "lấy tất cả chương của khóa học bằng courseId")]
        [HttpGet("{id}/chapters")]
        public async Task<ApiResponse<List<ChapterViewModel>>> GetChapterByCourseIdAsync(Guid id)
        {
            return await _chapterService.GetChapterByCourseIdAsync(id);
        }
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff + "," + AppRole.Instructor + "," + AppRole.Student)]
        [SwaggerOperation(Summary = "lấy danh sách lớp bằng course Id{Authorize = AgencyManager ,AgencyStaff , Instructor,Student}")]
        [HttpGet("courses/{id}/classes")]
        public async Task<ApiResponse<List<ClassViewModel>>> GetAllClassByCourseId(string id)
        {
            return await _classService.GetAllClassByCourseId(id);
        }
       
    }
}
