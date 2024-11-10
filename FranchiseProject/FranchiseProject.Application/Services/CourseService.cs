using AutoMapper;
using ClosedXML.Excel;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.AgenciesViewModels;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace FranchiseProject.Application.Services
{
	public class CourseService : ICourseService
    {
        #region Constructor
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IValidator<CreateCourseModel> _createCourseValidator;
        private readonly IValidator<UpdateCourseModel> _updateCourseValidator;
        private readonly IValidator<CreateSyllabusModel> _createSyllabusValidator;
        private readonly IValidator<List<CreateChapterModel>> _createChapterValidator;
        private readonly IValidator<List<CreateSessionArrangeModel>> _createSessionArrangeValidator;
        private readonly IValidator<List<CreateAssessmentArrangeModel>> _createAssessmentArrangeValidator;
        private readonly IValidator<List<CreateCourseMaterialArrangeModel>> _createCourseMaterialArrangeValidator;
        private readonly IValidator<List<CreateChapterFileModel>> _createChapterFileValidator;
        public CourseService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<UpdateCourseModel> updateCourseValidator, IValidator<CreateCourseModel> createCourseValidator,
            IClaimsService claimsService, UserManager<User> userManager, RoleManager<Role> roleManager,
            IValidator<CreateSyllabusModel> createSyllabusValidator, IValidator<List<CreateChapterModel>> createChapterValidator,
            IValidator<List<CreateSessionArrangeModel>> createSessionArrangeValidator, IValidator<List<CreateAssessmentArrangeModel>> createAssessmentArrangeValidator,
            IValidator<List<CreateCourseMaterialArrangeModel>> createCourseMaterialArrangeModel, IValidator<List<CreateChapterFileModel>> createChapterFileValidator)
        {
            _createCourseValidator = createCourseValidator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _updateCourseValidator = updateCourseValidator;
            _claimsService = claimsService;
            _userManager = userManager;
            _roleManager = roleManager;
            _createSyllabusValidator = createSyllabusValidator;
            _createChapterValidator = createChapterValidator;
            _createSessionArrangeValidator = createSessionArrangeValidator;
            _createAssessmentArrangeValidator = createAssessmentArrangeValidator;
            _createCourseMaterialArrangeValidator = createCourseMaterialArrangeModel;
            _createChapterFileValidator = createChapterFileValidator;
        }
        #endregion
        public async Task<ApiResponse<CourseDetailViewModel>> CreateCourseVersionAsync(Guid courseId)
        {
            var response = new ApiResponse<CourseDetailViewModel>();
            try
            {
             
                var course = await _unitOfWork.CourseRepository.GetCourseDetailForDuplicateAsync(courseId);
                if (course == null) throw new Exception("Course does not exist!");
                var newCourse = _mapper.Map<Course>(course);
                newCourse.Status = CourseStatusEnum.Draft;
                newCourse.Version = 0;
                await _unitOfWork.CourseRepository.AddAsync(newCourse);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Duplicate failed!");
                var courseModel = _mapper.Map<CourseDetailViewModel>(newCourse);
                response = ResponseHandler.Success(courseModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<CourseDetailViewModel>(ex.Message);
            }

            return response;
        }
        public async Task<ApiResponse<bool>> CreateCourseAsync(CreateCourseModel createCourseModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createCourseValidator.ValidateAsync(createCourseModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var course = _mapper.Map<Course>(createCourseModel);
                course.Version = 0;
                course.Status = CourseStatusEnum.Draft;
                await _unitOfWork.CourseRepository.AddAsync(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo chương học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(courseId);
                if (course == null) return ResponseHandler.Success<bool>(false, "Khóa học không khả dụng!");

                var checkCourse = await CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.CourseRepository.SoftRemove(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
                response = ResponseHandler.Success(true, "Xoá tài khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<CourseDetailViewModel>> GetCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<CourseDetailViewModel>();
            try
            {
                var course = await _unitOfWork.CourseRepository.GetCourseDetailAsync(courseId);
                if (course == null) throw new Exception("Course does not exist!");
                var courseModel = _mapper.Map<CourseDetailViewModel>(course);
                response = ResponseHandler.Success(courseModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<CourseDetailViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateCourseAsync(Guid courseId, UpdateCourseModel updateCourseModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateCourseValidator.ValidateAsync(updateCourseModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(courseId);
                if (course == null) return ResponseHandler.Success<bool>(false, "Khóa học không khả dụng!");

                var checkCourse = await CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                course = _mapper.Map(updateCourseModel, course);
                _unitOfWork.CourseRepository.Update(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật Khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<CourseViewModel>>> FilterCourseAsync(FilterCourseModel filterCourseViewModel)
        {
            var response = new ApiResponse<Pagination<CourseViewModel>>();
            try
            {
                Expression<Func<Course, bool>> filter = s =>
                (!filterCourseViewModel.MaxPrice.HasValue || filterCourseViewModel.MaxPrice >= s.Price) &&
                (!filterCourseViewModel.MinPrice.HasValue || filterCourseViewModel.MinPrice <= s.Price) &&
                (string.IsNullOrEmpty(filterCourseViewModel.Search) ||
                 s.Name.Contains(filterCourseViewModel.Search) ||
                 s.Description.Contains(filterCourseViewModel.Search) ||
                 s.Code.Contains(filterCourseViewModel.Search)) &&
                (!filterCourseViewModel.Status.HasValue || s.Status == filterCourseViewModel.Status) &&
                (!filterCourseViewModel.CourseCategoryId.HasValue || s.CourseCategoryId == filterCourseViewModel.CourseCategoryId);

                Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = null;
                if (filterCourseViewModel.SortBy.HasValue && filterCourseViewModel.SortDirection.HasValue)
                {
                    switch (filterCourseViewModel.SortBy)
                    {
                        case SortCourseStatusEnum.Name:
                            orderBy = filterCourseViewModel.SortDirection == SortDirectionEnum.Descending ?
                                query => query.OrderByDescending(p => p.Name) :
                                query => query.OrderBy(p => p.Name);
                            break;
                        case SortCourseStatusEnum.Price:
                            orderBy = filterCourseViewModel.SortDirection == SortDirectionEnum.Descending ?
                                query => query.OrderByDescending(p => p.Price) :
                                query => query.OrderBy(p => p.Price);
                            break;

                    }
                }
                var courses = await _unitOfWork.CourseRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties: "CourseCategory",
                    pageIndex: filterCourseViewModel.PageIndex,
                    pageSize: filterCourseViewModel.PageSize
                    );
                var courseViewModels = _mapper.Map<Pagination<CourseViewModel>>(courses);
                if (courseViewModels.Items.IsNullOrEmpty()) return ResponseHandler.Success(courseViewModels, "Không tìm thấy khóa học phù hợp!");

                response = ResponseHandler.Success(courseViewModels, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<CourseViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> CheckCourseAvailableAsync(Guid? courseId, CourseStatusEnum status)
        {
            var response = new ApiResponse<bool>();
            var courseNoAvalable = "Khóa học không khả dụng!";
            var courseCanOnlyBeEditedInDraftState = "Chỉ có thể sửa đổi thông tin của khóa học ở trạng thái nháp!";
            try
            {
                if (courseId == null) throw new ArgumentNullException(nameof(courseId));
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync((Guid)courseId);
                if (course == null) return ResponseHandler.Success(false, courseNoAvalable);

                if (course.Status != status)
                {
                    response.Message = courseNoAvalable;
                    switch (status)
                    {
                        case CourseStatusEnum.Draft: response.Message = courseCanOnlyBeEditedInDraftState; break;
                        case CourseStatusEnum.AvailableForFranchise: response.Message = courseNoAvalable; break;
                    }
                    return ResponseHandler.Success(false, response.Message);
                }
                response = ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateCourseStatusAsync(Guid courseId, CourseStatusEnum courseStatusEnum)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new Exception("Authenticate failed!");
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if (role == null) throw new Exception("User role not found!");
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(courseId);
                if (course == null) return ResponseHandler.Success<bool>(false, "Khóa học không khả dụng!");
                switch (course.Status)
                {
                    case CourseStatusEnum.Draft:
                        if ((courseStatusEnum != CourseStatusEnum.PendingApproval))
                            throw new Exception("Can only update to PendingApproval status");
                        if ((role != RolesEnum.SystemInstructor.ToString()) && (role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.PendingApproval:
                        if ((courseStatusEnum != CourseStatusEnum.AvailableForFranchise) && (courseStatusEnum != CourseStatusEnum.Closed))
                            throw new Exception("Can only update to AvailableForFranchise status or Closed status");
                        if ((role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.AvailableForFranchise:
                        if ((courseStatusEnum != CourseStatusEnum.TemporarilySuspended) && (courseStatusEnum != CourseStatusEnum.Closed))
                            throw new Exception("Can only update to TemporarilySuspended status or Closed status");
                        if ((role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.TemporarilySuspended:
                        if ((courseStatusEnum != CourseStatusEnum.AvailableForFranchise) && (courseStatusEnum != CourseStatusEnum.Closed))
                            throw new Exception("Can only update to AvailableForFranchise status or Closed status");
                        if ((role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.Closed:
                        throw new Exception("Cannot update in this state");
                }
                course.Status = courseStatusEnum;
                //Update course cũ
                if (course.Status == CourseStatusEnum.AvailableForFranchise)
                {
                    var oldCourse = (await _unitOfWork.CourseRepository.FindAsync
                        (e => e.Code == course.Code && e.Status != CourseStatusEnum.Draft
                        && e.Status != CourseStatusEnum.PendingApproval))
                        .OrderByDescending(e => e.Version).FirstOrDefault();
                    if (oldCourse != null && oldCourse.Id != courseId)
                    {
                        oldCourse.Status = CourseStatusEnum.Closed;
                        course.Version = oldCourse.Version + 1;
                        _unitOfWork.CourseRepository.Update(oldCourse);
                        // Sửa danh sách học sinh đang ở course cũ 
                            
                        var registerCourses = (await _unitOfWork.RegisterCourseRepository
                            .FindAsync(e => e.CourseId == oldCourse.Id && 
                                            e.StudentCourseStatus != StudentCourseStatusEnum.Enrolled &&
                                            e.StudentCourseStatus != StudentCourseStatusEnum.Cancel)).ToList();
                        foreach (var registerCourse in registerCourses)
                        {
                            registerCourse.CourseId = course.Id;
                        }
                        _unitOfWork.RegisterCourseRepository.UpdateRange(registerCourses);
                    }
                    else course.Version = 1;
                }
                _unitOfWork.CourseRepository.Update(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật trạng thái khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> CreateCourseByFileAsync(CourseFilesModel courseFilesModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (courseFilesModel.CourseFile == null || courseFilesModel.CourseFile.Length == 0) throw new Exception("Course file is empty.");
                if (courseFilesModel.QuestionFile == null || courseFilesModel.QuestionFile.Length == 0) throw new Exception("Question file is empty.");
                if (courseFilesModel.ChapterMaterialFile == null || courseFilesModel.ChapterMaterialFile.Length == 0) throw new Exception("ChapterMaterial file is empty.");
                using (var stream = new MemoryStream())
                {
                    var course = new Course();
                    await courseFilesModel.CourseFile.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        int sheetCount = workbook.Worksheets.Count();
                        if (sheetCount != 6)
                            return ResponseHandler.Failure<bool>(
                                sheetCount < 6 ? "Số lượng bảng tính ít hơn số lượng thành phần trong khoá học." :
                                                              "Số lượng bảng tính nhiều hơn số lượng thành phần trong khoá học."
                            );
                        course = await ExtractCourseFromWorksheetAsync(workbook.Worksheets.First(), course);
                        course = await ExtractSyllabusFromWorksheetAsync(workbook.Worksheets.Skip(1).First(), course);
                        var courseCheck = await ExtractChapterFromWorksheetAsync(workbook.Worksheets.Skip(2).First(),
                            courseFilesModel.QuestionFile, courseFilesModel.ChapterMaterialFile, course);
                        if (!courseCheck.isSuccess && courseCheck.Message != null) 
                            return ResponseHandler.Failure<bool>(courseCheck.Message);
                        course = courseCheck.Data;
                        course = await ExtractSessionFromWorksheetAsync(workbook.Worksheets.Skip(3).First(), course);
                        course = await ExtractAssessmentFromWorksheetAsync(workbook.Worksheets.Skip(4).First(), course);
                        course = await ExtractMaterialFromWorksheetAsync(workbook.Worksheets.Skip(5).First(), course);
                        course = _mapper.Map<Course>(course);
                        await _unitOfWork.CourseRepository.AddAsync(course);
                         
                        var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                        if (!isSuccess) throw new Exception("Create failed!");
                    }
                }

                response = ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        private async Task<Course> ExtractCourseFromWorksheetAsync(IXLWorksheet worksheet, Course course)
        {
            var rows = worksheet.RangeUsed().RowsUsed();
            var headerRow = rows.First();
            var courseModel = new CreateCourseModel();
            var categoryName = "";
            foreach (var row in rows.Skip(1))
            {
                foreach (var cell in row.Cells())
                {
                    var columnIndex = cell.Address.ColumnNumber - 1;

                    switch (columnIndex)
                    {
                        case 0:
                            courseModel.Name = cell.Value.ToString();
                            break;
                        case 1:
                            courseModel.Description = cell.Value.ToString();
                            break;
                        case 2:
                            courseModel.URLImage = cell.Value.ToString();
                            break;
                        case 3:
                            courseModel.NumberOfLession = (int)cell.Value;
                            break;
                        case 4:
                            courseModel.Price = (int)cell.Value;
                            break;
                        case 5:
                            courseModel.Code = cell.Value.ToString();
                            break;
                        case 6:
                            categoryName = cell.Value.ToString();
                            break;
                    }
                }
            }
            ValidationResult validationResult = await _createCourseValidator.ValidateAsync(courseModel);
            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);
            if (!categoryName.IsNullOrEmpty()) 
            { 
                var category = (await _unitOfWork.CourseCategoryRepository.FindAsync(e => e.Name.Contains(categoryName))).FirstOrDefault();
                if(category != null) courseModel.CourseCategoryId = category.Id;
            }
            course = _mapper.Map<Course>(courseModel);

            return course;
        }
        private async Task<Course> ExtractSyllabusFromWorksheetAsync(IXLWorksheet worksheet, Course course)
        {
            var rows = worksheet.RangeUsed().RowsUsed();
            var headerRow = rows.First();
            var syllabusModel = new CreateSyllabusModel();
            foreach (var row in rows.Skip(1))
            {
                foreach (var cell in row.Cells())
                {
                    var columnIndex = cell.Address.ColumnNumber - 1;

                    switch (columnIndex)
                    {
                        case 0:
                            syllabusModel.Description = cell.Value.ToString();
                            break;
                        case 1:
                            syllabusModel.StudentTask = cell.Value.ToString();
                            break;
                        case 2:
                            syllabusModel.TimeAllocation = cell.Value.ToString();
                            break;
                        case 3:
                            syllabusModel.ToolsRequire = cell.Value.ToString();
                            break;
                        case 4:
                            if (double.TryParse(cell.Value.ToString(), out double scaleValue))
                            {
                                syllabusModel.Scale = scaleValue;
                            }
                            break;
                        case 5:
                            if (double.TryParse(cell.Value.ToString(), out double minAvgMarkValue))
                            {
                                syllabusModel.MinAvgMarkToPass = minAvgMarkValue;
                            }
                            break;
                    }
                }
            }
            ValidationResult validationResult = await _createSyllabusValidator.ValidateAsync(syllabusModel);
            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);
            var syllabus = _mapper.Map<Syllabus>(syllabusModel);
            course.Syllabus = syllabus;
            return course;
        }
        private async Task<ApiResponse<Course>> ExtractChapterFromWorksheetAsync(IXLWorksheet worksheet, IFormFile questionsFile, IFormFile materialsFile, Course course)
        {
            var response = new ApiResponse<Course>();
            var rows = worksheet.RangeUsed().RowsUsed();
            var chapterModels = new List<CreateChapterFileModel>();
            using (var questionsStream = new MemoryStream())
            using (var materialsStream = new MemoryStream())
            {
                await questionsFile.CopyToAsync(questionsStream);
                await materialsFile.CopyToAsync(materialsStream);
                questionsStream.Position = 0;
                materialsStream.Position = 0;
                using (var questionWorkbook = new XLWorkbook(questionsStream))
                using (var materialWorkbook = new XLWorkbook(materialsStream))
                {
                    int sheetQuestionCount = questionWorkbook.Worksheets.Count();
                    int sheetMaterialCount = materialWorkbook.Worksheets.Count();
                    int rowCount = rows.Skip(1).Count();
                    if (sheetQuestionCount != rowCount)
                        return ResponseHandler.Failure<Course>(
                            sheetQuestionCount < rowCount ? "Số lượng bảng tính của files câu hỏi ít hơn số lượng chapters trong khoá học." :
                                                          "Số lượng bảng tính của files câu hỏi nhiều hơn số lượng chapters trong khoá học."
                        );

                    if (sheetMaterialCount != rowCount)
                        return ResponseHandler.Failure<Course>(
                            sheetQuestionCount < rowCount ? "Số lượng bảng tính của files tài nguyên ít hơn số lượng chapters trong khoá học." :
                                                          "Số lượng bảng tính của files tài nguyên nhiều hơn số lượng chapters trong khoá học."
                        );
                    var chapterIndex = 0;
                    foreach (var row in rows.Skip(1))
                    {
                        var chapterModel = new CreateChapterFileModel
                        {
                            Number = (int)row.Cell(1).Value,
                            Topic = row.Cell(2).Value.ToString(),
                            Description = row.Cell(3).Value.ToString(),
                            Questions = new List<CreateQuestionArrangeModel>(),
                            ChapterMaterials = new List<CreateChapterMaterialArrangeModel>()
                        };
                        var questionWorksheet = questionWorkbook.Worksheets.Skip(chapterIndex).First();
                        var questionRows = questionWorksheet.RangeUsed().RowsUsed();
                        foreach (var questionRow in questionRows.Skip(1)) 
                        {
                            var questionModel = new CreateQuestionArrangeModel
                            {
                                Description = questionRow.Cell(1).Value.ToString(),
                                ImageURL = questionRow.Cell(2).Value.ToString(),
                                QuestionOptions = new List<CreateQuestionOptionArrangeModel>()
                            };
                            for (var j = 3; j <= questionRow.Cells().Count(); j += 3)
                            {
                                string cellValue = questionRow.Cell(j + 2).Value.ToString().Trim().ToLowerInvariant();
                                bool status;

                                if (cellValue == "true")
                                {
                                    status = true;
                                }
                                else if (cellValue == "false")
                                {
                                    status = false;
                                }
                                else
                                {
                                    throw new Exception($"Invalid boolean value '{cellValue}' in cell.");
                                }
                                var questionOption = new CreateQuestionOptionArrangeModel
                                {
                                    Description = questionRow.Cell(j).Value.ToString(),
                                    ImageURL = questionRow.Cell(j + 1).Value.ToString(),
                                    Status = status
                                };
                                questionModel.QuestionOptions.Add(questionOption);
                            }
                            chapterModel.Questions.Add(questionModel);
                        }
                        var materialWorksheet = materialWorkbook.Worksheets.Skip(chapterIndex).First();
                        var materialRows = materialWorksheet.RangeUsed().RowsUsed();
                        var materialIndex = 0;
                        foreach (var materialRow in materialRows.Skip(1))
                        {
                            var materialModel = new CreateChapterMaterialArrangeModel
                            {
                                Number = materialIndex + 1,
                                URL = materialRow.Cell(1).Value.ToString(),
                                Description = materialRow.Cell(2).Value.ToString(),
                            };
                            materialIndex++;
                            chapterModel.ChapterMaterials.Add(materialModel);
                        }
                        chapterModels.Add(chapterModel);
                        chapterIndex++;
                    }
                }
            }
            ValidationResult validationResult = await _createChapterFileValidator.ValidateAsync(chapterModels);
            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);
            var chapters = _mapper.Map<List<Chapter>>(chapterModels);
            course.Chapters = chapters;
            response.isSuccess = true;
            response.Data = course;
            return response;
        }
        private async Task<Course> ExtractSessionFromWorksheetAsync(IXLWorksheet worksheet, Course course)
        {
            var rows = worksheet.RangeUsed().RowsUsed();
            var headerRow = rows.First();
            var sessionModels = new List<CreateSessionArrangeModel>();
            foreach (var row in rows.Skip(1))
            {
                var session = new CreateSessionArrangeModel();
                foreach (var cell in row.Cells())
                {
                    var columnIndex = cell.Address.ColumnNumber - 1;
                    switch (columnIndex)
                    {
                        case 0:
                            session.Number = (int)cell.Value;
                            break;
                        case 1:
                            session.Topic = cell.Value.ToString();
                            break;
                        case 2:
                            session.Chapter = cell.Value.ToString();
                            break;
                        case 3:
                            session.Description = cell.Value.ToString();
                            break;
                    }
                }
                sessionModels.Add(session);
            }
            ValidationResult validationResult = await _createSessionArrangeValidator.ValidateAsync(sessionModels);
            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);
            var sessions = _mapper.Map<List<Session>>(sessionModels);
            course.Sessions = sessions;
            return course;
        }
        private async Task<Course> ExtractAssessmentFromWorksheetAsync(IXLWorksheet worksheet, Course course)
        {
            var rows = worksheet.RangeUsed().RowsUsed();
            var headerRow = rows.First();
            var assessmentModels = new List<CreateAssessmentArrangeModel>();
            foreach (var row in rows.Skip(1))
            {
                var assessment = new CreateAssessmentArrangeModel();
                foreach (var cell in row.Cells())
                {
                    var columnIndex = cell.Address.ColumnNumber - 1;
                    switch (columnIndex)
                    {
                        case 0:
                            assessment.Number = (int)cell.Value;
                            break;
                        case 1:
                            assessment.Type = cell.Value.ToString();
                            break;
                        case 2:
                            assessment.Content = cell.Value.ToString();
                            break;
                        case 3:
                            assessment.Quantity = (int)cell.Value;
                            break;
                        case 4:
                            if (double.TryParse(cell.Value.ToString(), out double weightValue))
                            {
                                assessment.Weight = weightValue;
                            }
                            break;
                        case 5:
                            if (double.TryParse(cell.Value.ToString(), out double completionCriteriaValue))
                            {
                                assessment.CompletionCriteria = completionCriteriaValue;
                            }
                            break;
                        case 6:
                            if (int.TryParse(cell.Value.ToString(), out int enumValue) &&
                                Enum.IsDefined(typeof(AssessmentMethodEnum), enumValue))
                            {
                                assessment.Method = (AssessmentMethodEnum)enumValue;
                            }
                            break;
                        case 7:
                            assessment.Duration = cell.Value.ToString();
                            break;
                        case 8:
                            assessment.QuestionType = cell.Value.ToString();
                            break;
                    }
                }
                assessmentModels.Add(assessment);
            }
            ValidationResult validationResult = await _createAssessmentArrangeValidator.ValidateAsync(assessmentModels);
            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);
            var assessments = _mapper.Map<List<Assessment>>(assessmentModels);
            course.Assessments = assessments;
            return course;
        }
        private async Task<Course> ExtractMaterialFromWorksheetAsync(IXLWorksheet worksheet, Course course)
        {
            var rows = worksheet.RangeUsed().RowsUsed();
            var headerRow = rows.First();
            var courseMaterialModels = new List<CreateCourseMaterialArrangeModel>();
            foreach (var row in rows.Skip(1))
            {
                var courseMaterial = new CreateCourseMaterialArrangeModel();
                foreach (var cell in row.Cells())
                {
                    var columnIndex = cell.Address.ColumnNumber - 1;
                    switch (columnIndex)
                    {
                        case 0:
                            courseMaterial.URL = cell.Value.ToString();
                            break;
                        case 1:
                            courseMaterial.Description = cell.Value.ToString();
                            break;
                    }
                }
                courseMaterialModels.Add(courseMaterial);
            }
            ValidationResult validationResult = await _createCourseMaterialArrangeValidator.ValidateAsync(courseMaterialModels);
            if (!validationResult.IsValid) throw new Exception(ValidatorHandler.HandleValidation<bool>(validationResult).Message);
            var courseMaterials = _mapper.Map<List<CourseMaterial>>(courseMaterialModels);
            course.CourseMaterials = courseMaterials;
            return course;
        }
		public async Task<ApiResponse<IEnumerable<CourseViewModel>>> GetAllCoursesAvailableAsync()
		{
			var response = new ApiResponse<IEnumerable<CourseViewModel>>();
			try
			{
				var courses = await _unitOfWork.CourseRepository.FindAsync(x => x.Status == CourseStatusEnum.AvailableForFranchise);
				var courseViewModel = _mapper.Map<IEnumerable<CourseViewModel>>(courses);
				if (courseViewModel.IsNullOrEmpty()) return ResponseHandler.Success(courseViewModel, "Không có khóa học nào khả dụng!");
				response = ResponseHandler.Success(courseViewModel, "Lấy tất cả khóa học khả dụng thành công!");
			}
			catch (Exception ex)
			{
				response = ResponseHandler.Failure<IEnumerable<CourseViewModel>>(ex.Message);
			}
			return response;
		}
	}
}
