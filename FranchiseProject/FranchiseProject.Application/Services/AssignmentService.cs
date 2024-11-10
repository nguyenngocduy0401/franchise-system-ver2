using AutoMapper;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace FranchiseProject.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateAssignmentViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public AssignmentService(IEmailService emailService, IHubContext<NotificationHub> hubContext, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateAssignmentViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
            _hubContext = hubContext;
            _emailService = emailService;

        }
        #endregion
        public async Task<ApiResponse<bool>> CreateAssignmentAsync(CreateAssignmentViewModel assignment)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(assignment);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);

                }
                var ass = _mapper.Map<Assignment>(assignment);
                await _unitOfWork.AssignmentRepository.AddAsync(ass);
                var students = await _unitOfWork.ClassRepository.GetStudentsByClassIdAsync(Guid.Parse(assignment.ClassId));
                foreach (var student in students)
                {

                await    _hubContext.Clients.User(student.Id.ToString())
                        .SendAsync("ReceivedNotification", $"Bạn có bài tập mới bắt đầu lúc {assignment.StartTime.ToString()}.");
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo bài tập thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateAssignmentAsync(CreateAssignmentViewModel update, string assignmentId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(update);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);

                }
                var assignement = await _unitOfWork.AssignmentRepository.GetExistByIdAsync(Guid.Parse(assignmentId));
                if (assignement == null) return ResponseHandler.Success<bool>(false, "Bài tập không tồn tại!");
                assignement = _mapper.Map(update, assignement);
                _unitOfWork.AssignmentRepository.Update(assignement);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "cập nhật bài tập  thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<AssignmentViewModel>> GetAssignmentByIdAsync(string slotId)
        {

            var response = new ApiResponse<AssignmentViewModel>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<AssignmentViewModel>(null, "User hoặc Agency không khả dụng!");
                }
                var ass = await _unitOfWork.AssignmentRepository.GetByIdAsync(Guid.Parse(slotId));
                if (ass == null) throw new Exception("Bài tập  không tồn tại!");
                var assViewModel = _mapper.Map<AssignmentViewModel>(ass);
                response = ResponseHandler.Success(assViewModel, "Lấy thông tin bài tập  thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<AssignmentViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteAssignmentByIdAsync(string assId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var ass = await _unitOfWork.AssignmentRepository.GetExistByIdAsync(Guid.Parse(assId));
                if (ass == null) return ResponseHandler.Success(false, "Bài tập  không khả dụng!");

                _unitOfWork.AssignmentRepository.SoftRemove(ass);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá bài tập thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<AssignmentViewModel>>> GetAssignmentByClassIdAsync(string classId, int pageIndex, int pageSize)
        {
            var response = new ApiResponse<Pagination<AssignmentViewModel>>();
            try
            {
                var classID = Guid.Parse(classId);
                var assignments = await _unitOfWork.AssignmentRepository.GetAllAsync1(a => a.ClassId == classID);
                if (assignments == null || !assignments.Any())
                {
                    return ResponseHandler.Success<Pagination<AssignmentViewModel>>(null, "Không tìm thấy bài tập nào cho lớp này.");
                }

                var totalItemsCount = assignments.Count();
                var paginatedAssignments = assignments.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var assignmentPagination = new Pagination<Assignment>
                {
                    Items = paginatedAssignments,
                    TotalItemsCount = totalItemsCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var assignmentViewModelPagination = _mapper.Map<Pagination<AssignmentViewModel>>(assignmentPagination);
                response = ResponseHandler.Success<Pagination<AssignmentViewModel>>(assignmentViewModelPagination, "Lấy danh sách bài tập thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<AssignmentViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> SubmitAssignmentAsync(string assignmentId, string fileSubmitUrl)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var assId = Guid.Parse(assignmentId);
                var currentUserId = _claimsService.GetCurrentUserId.ToString();
                if (currentUserId == null)
                {
                    return ResponseHandler.Success(false, "User chưa đăng nhập!");
                }
                var assignment = await _unitOfWork.AssignmentRepository.GetFirstOrDefaultAsync(a => a.Id == assId);
                if (assignment == null)
                {
                    return ResponseHandler.Success(false, "Bài tập không tồn tại!");
                }
                var existingSubmission = await _unitOfWork.AssignmentSubmitRepository.GetFirstOrDefaultAsync(rc => rc.UserId == currentUserId && rc.AssignmentId == assId);

                if (existingSubmission != null)
                {

                    await _unitOfWork.AssignmentSubmitRepository.DeleteAsync(existingSubmission);
                }
                var assignmentSubmit = new AssignmentSubmit
                {
                    AssignmentId = Guid.Parse(assignmentId),
                    UserId = currentUserId.ToString(),
                    FileSubmitURL = fileSubmitUrl,
                    SubmitDate = DateTime.UtcNow
                };
                await _unitOfWork.AssignmentSubmitRepository.AddAsync(assignmentSubmit);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                if (!isSuccess)
                {
                    throw new Exception("Nộp bài thất bại!");
                }

                response = ResponseHandler.Success(true, "Nộp bài thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }

            return response;
        }
        public async Task<ApiResponse<Pagination<AssignmentSubmitViewModel>>> GetAssignmentSubmissionAsync(string assignmentId, int pageIndex, int pageSize)
        {
            var response = new ApiResponse<Pagination<AssignmentSubmitViewModel>>();

            try
            {
                var assid = Guid.Parse(assignmentId);
                var currentUserId = _claimsService.GetCurrentUserId.ToString();


                var assSubmits = await _unitOfWork.AssignmentSubmitRepository.GetAllSubmitAsync(assid);

                var totalCount = assSubmits.Count();
                var submissions = assSubmits
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AssignmentSubmitViewModel
                    {
                        AssignmentId = a.AssignmentId,
                        AssignmentName = a.Assignment?.Title,
                        UserId = a.UserId,
                        UserName = a.User?.UserName,
                        FileSubmitURL = a.FileSubmitURL,
                        SubmitDate = a.SubmitDate,
                        ScoreNumber = a.ScoreNumber ?? 0
                    })
                    .ToList();
                var assignmentPagination = new Pagination<AssignmentSubmitViewModel>
                {
                    Items = submissions,
                    TotalItemsCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var assignmentViewModelPagination = _mapper.Map<Pagination<AssignmentSubmitViewModel>>(assignmentPagination);

                response = ResponseHandler.Success<Pagination<AssignmentSubmitViewModel>>(assignmentViewModelPagination, "Lấy thông tin bài nộp thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<AssignmentSubmitViewModel>>(ex.Message);
            }

            return response;
        }

        /*    public async Task<ApiResponse<Pagination<AssignmentSubmitViewModel>>> FilterAsignmentAsync(FilterAssignmentViewModel filterModel)
            {
                var response = new ApiResponse<Pagination<AssignmentSubmitViewModel>>();
                try
                {
                    var userCurrentId = _claimsService.GetCurrentUserId;
                    var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                    if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                    {
                        return ResponseHandler.Failure<Pagination<AssignmentSubmitViewModel>>("User hoặc Agency không khả dụng!");
                    }
                    Expression<Func<Assignment, bool>> filter = s =>
                    (!filterModel.StartTime.HasValue || filterModel.StartTime <= s.StartTime) &&
                    (!filterModel.StartTime.HasValue || filterModel.EndTime >= s.EndTime) &&
                     (s.AgencyId == userCurrent.AgencyId);
                    var ass = await _unitOfWork.AssignmentRepository.GetFilterAsync(
                        filter: filter,
                        pageIndex: filterModel.PageIndex,
                        pageSize: filterModel.PageSize
                        );
                    var assViewModels = _mapper.Map<Pagination<AssignmentSubmitViewModel>>(ass);
                    if (assViewModels.Items.IsNullOrEmpty()) return ResponseHandler.Success(assViewModels, "Không tìm thấy slot phù hợp!");

                    response = ResponseHandler.Success(assViewModels, "Successful!");

                }
                catch (Exception ex)
                {
                    response = ResponseHandler.Failure<Pagination<AssignmentSubmitViewModel>>(ex.Message);
                }
                return response;
            }*/

        public async Task<ApiResponse<bool>> GradeStudentAssAsync(StudentAssScorseNumberViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                /*    var currentUserIid = _claimsService.GetCurrentUserId.ToString();
                    var currentUser = _userManager.FindByIdAsync(currentUserIid);*/

                var assignmentsubmit = await _unitOfWork.AssignmentSubmitRepository.GetFirstOrDefaultAsync(rc => rc.AssignmentId == model.AssignmentId && rc.UserId == model.UserId);
                var assignment = await _unitOfWork.AssignmentRepository.GetExistByIdAsync(assignmentsubmit.AssignmentId.Value);
                if (model.ScoreNumber < 0 || model.ScoreNumber > 10)
                {
                    response = ResponseHandler.Success(false, "điểm không hợp lệ");
                }

                var students = await _userManager.FindByIdAsync(model.UserId);


                await _hubContext.Clients.User(students.Id.ToString())
                    .SendAsync("ReceivedNotification", $" bài tập {assignment.Title.ToString()} đã được chấm điểm.");
                assignmentsubmit.ScoreNumber = model.ScoreNumber;
                await _unitOfWork.AssignmentSubmitRepository.UpdatesAsync(assignmentsubmit);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;


                response = ResponseHandler.Success(true, "Chấm điểm thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;

        }
        public async Task<ApiResponse<List<AsmSubmitDetailViewModel>>> GetAllAsmByClassId(Guid classId)
        {
            var response = new ApiResponse<List<AsmSubmitDetailViewModel>>();
            try
            {
               var classs = await _unitOfWork.ClassRepository.GetExistByIdAsync(classId);
                    if (classs == null || classs.Status != ClassStatusEnum.Active)
                        return ResponseHandler.Success<List<AsmSubmitDetailViewModel>>(null, "Lớp học không khả dụng!");

                    var asm = await _unitOfWork.AssignmentRepository.GetAsmsByClassId(classId);
                    var asmModel = _mapper.Map<List<AsmSubmitDetailViewModel>>(asm);
                    response = ResponseHandler.Success(asmModel);
              
                return response;
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<AsmSubmitDetailViewModel>>(ex.Message);

                return response;
            }
        }
    }
}
