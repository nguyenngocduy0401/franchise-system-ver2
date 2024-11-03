using AutoMapper;
using DocumentFormat.OpenXml.Math;
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
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateAssignmentViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public AssignmentService(IEmailService emailService,IHubContext<NotificationHub> hubContext ,IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateAssignmentViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
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
                   
                    _hubContext.Clients.User(student.Id.ToString())
                        .SendAsync("ReceivedNotification", $"Bạn có bài Tập mới bắt đầu lúc {assignment.StartTime.ToString()}.");
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
        public async Task<ApiResponse<bool>> UpdateAssignmentAsync(CreateAssignmentViewModel update,string assignmentId)
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
                if (assignement == null) return ResponseHandler.Failure<bool>("Bài tập không tồn tại!");
                assignement = _mapper.Map(update,assignement);
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
                    return ResponseHandler.Failure<AssignmentViewModel>("User hoặc Agency không khả dụng!");
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
        public async Task<ApiResponse<bool>> DeleteSlotByIdAsync(string  assId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var ass = await _unitOfWork.AssignmentRepository.GetExistByIdAsync(Guid.Parse(assId));
                if (ass == null) return ResponseHandler.Success(false, "Slot học không khả dụng!");

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
        public async Task<ApiResponse<Pagination<AssignmentViewModel>>> GetAssignmentByClassIdAsync(string classId,int pageIndex, int pageSize)
        {
            var response = new ApiResponse<Pagination<AssignmentViewModel>>();
            try
            {
                var classID = Guid.Parse(classId);
                var assignments = await _unitOfWork.AssignmentRepository.GetAllAsync1(a => a.ClassId == classID);
                if (assignments == null || !assignments.Any())
                {
                    return ResponseHandler.Success<Pagination<AssignmentViewModel>>(null,"Không tìm thấy bài tập nào cho lớp này.");
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
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<AssignmentViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> SubmitAssignmentAsync(string  assignmentId, string fileSubmitUrl)
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
                var existingSubmission = await _unitOfWork.AssignmentSubmitRepository.GetFirstOrDefaultAsync(rc => rc.UserId==currentUserId &&rc.AssignmentId==assId);

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
                var assId = Guid.Parse(assignmentId);
                var currentUserId = _claimsService.GetCurrentUserId.ToString();

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return ResponseHandler.Failure<Pagination<AssignmentSubmitViewModel>>("User chưa đăng nhập!");
                }
                var submissions = await _unitOfWork.AssignmentSubmitRepository.GetAllAsync1(rs => rs.AssignmentId == assId);

                if (submissions == null)
                {
                    return ResponseHandler.Failure<Pagination<AssignmentSubmitViewModel>>("Bài nộp không tồn tại!");
                }

                var assignmentSubmitViewModels = new List<AssignmentSubmitViewModel>();

                foreach (var submission in submissions)
                {
                    var user = await _userManager.FindByIdAsync(submission.UserId);
                    assignmentSubmitViewModels.Add(new AssignmentSubmitViewModel
                    {
                        AssignmentId = submission.AssignmentId,
                        AssignmentName = submission.Assignment?.Title,
                        UserId = user?.Id, 
                        UserName = user?.UserName, 
                        FileSubmitURL = submission.FileSubmitURL,
                        SubmitDate = submission.SubmitDate
                    });
                }

                var totalItemsCount = assignmentSubmitViewModels.Count();
                var paginatedAssignments = assignmentSubmitViewModels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var assignmentPagination = new Pagination<AssignmentSubmitViewModel>
                {
                    Items = paginatedAssignments,
                    TotalItemsCount = totalItemsCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var assignmentViewModelPagination = _mapper.Map<Pagination<AssignmentViewModel>>(assignmentPagination);

                response = ResponseHandler.Success(assignmentPagination, "Lấy thông tin bài nộp thành công!");
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
               if(model.ScoreNumber<0 || model.ScoreNumber > 10)
                {
                    response = ResponseHandler.Success(false, "điểm không hợp lệ");
                }
                var ass = _mapper.Map<Score>(model);
                var students = await _userManager.FindByIdAsync(model.UserId);
               var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(Guid.Parse(model.AssignmentId));
                    await _hubContext.Clients.User(students.Id.ToString())
                        .SendAsync("ReceivedNotification", $" bài Tập {assignment.Title.ToString()} đã được chấm điểm.");
                
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Chấm điểm thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;

        }
    }
}
