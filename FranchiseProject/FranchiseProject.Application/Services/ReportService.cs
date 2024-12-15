using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgenciesViewModels;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Application.ViewModels.ReportViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FranchiseProject.Application.ViewModels.NotificationViewModels;
namespace FranchiseProject.Application.Services
{
    public class ReportService : IReportService
    {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IClaimsService _claimsService;
            private readonly ICurrentTime _currentTime;
            private readonly IValidator<CreateAgencyViewModel> _validator;
            private readonly IValidator<UpdateAgencyViewModel> _validatorUpdate;
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private readonly IHubContext<NotificationHub> _hubContext;
            private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
             private readonly UserManager<User> _userManager;
        public ReportService(IMapper mapper, IUnitOfWork unitOfWork,
                IClaimsService claimsService, IValidator<CreateAgencyViewModel> validator,
                IUserService userService, IHubContext<NotificationHub> hubContext,
                IEmailService emailService, IValidator<UpdateAgencyViewModel> validatorUpdate,
                ICurrentTime currentTime, UserManager<User> userManager, INotificationService notificationService)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _claimsService = claimsService;
                _mapper = mapper;
                _userService = userService;
                _hubContext = hubContext;
                _emailService = emailService;
                _validatorUpdate = validatorUpdate;
                _currentTime = currentTime;
            _userManager = userManager;
            _notificationService = notificationService;
        }
        public async Task<ApiResponse<bool>> CreateCourseReport(CreateReportCourseViewModel model)
        {
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<bool>(false,"User không tồn tại hoặc không thuộc về Agency nào.");
                }

                var course = await _unitOfWork.CourseRepository.GetByIdAsync(model.CourseId.Value);
                if (course == null)
                {
                    return ResponseHandler.Success<bool>(false,"Khóa học không tồn tại.");
                }

                var report = new Report
                {
                    Description = model.Description,
                    Status = ReportStatusEnum.Pending,
                    Type = ReportTypeEnum.Course,
                    AgencyId = userCurrent.AgencyId,
                    CourseId = model.CourseId,
                    CreationDate = _currentTime.GetCurrentTime()
                };

                await _unitOfWork.ReportRepository.AddAsync(report);
                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Tạo báo cáo khóa học thành công.");
                }
                else
                {
                    return ResponseHandler.Success<bool>(false,"Không thể lưu báo cáo khóa học.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi tạo báo cáo khóa học: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> CreateEquipmentReport(CreateReportEquipmentViewModel model)
        {
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<bool>(false,"User không tồn tại hoặc không thuộc về Agency nào.");
                }
                var equipments = new List<Equipment>();
                foreach (var equipmentId in model.EquipmentIds)
                {
                    var equipment = await _unitOfWork.EquipmentRepository.GetByIdAsync(equipmentId);
                    if (equipment == null)
                    {
                        return ResponseHandler.Success<bool>(false,$"Thiết bị với ID {equipmentId} không tồn tại.");
                    }
                   // equipment.Status = EquipmentStatusEnum.Repair;
                    equipments.Add(equipment);
                }

                var report = new Report
                {
                    Description = model.Description,
                    Status = ReportStatusEnum.Pending,
                    Type=ReportTypeEnum.Equipment,
                    AgencyId = userCurrent.AgencyId,
                    CreationDate = _currentTime.GetCurrentTime(),
                    Equipments = equipments
                };

                await _unitOfWork.ReportRepository.AddAsync(report);
                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Tạo báo cáo thiết bị thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể lưu báo cáo thiết bị.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi tạo báo cáo thiết bị: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> UpdateCourseReport(Guid reportId, UpdateReportCourseViewModel model)
        {
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<bool>(false,"User không tồn tại hoặc không thuộc về Agency nào.");
                }

                var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    return ResponseHandler.Success<bool>(false,"Báo cáo không tồn tại.");
                }

                if (report.AgencyId != userCurrent.AgencyId)
                {
                    return ResponseHandler.Success<bool>(false,"Bạn không có quyền cập nhật báo cáo này.");
                }

                if (report.Type != ReportTypeEnum.Course)
                {
                    return ResponseHandler.Success<bool>(false,"Báo cáo này không phải là báo cáo khóa học.");
                }

                if (model.CourseId.HasValue && model.CourseId != report.CourseId)
                {
                    var course = await _unitOfWork.CourseRepository.GetByIdAsync(model.CourseId.Value);
                    if (course == null)
                    {
                        return ResponseHandler.Success<bool>(false,"Khóa học không tồn tại.");
                    }
                    report.CourseId = model.CourseId;
                }

                report.Description = model.Description;
                report.ModificationDate = _currentTime.GetCurrentTime();

                _unitOfWork.ReportRepository.Update(report);
                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Cập nhật báo cáo khóa học thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể cập nhật báo cáo khóa học.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi cập nhật báo cáo khóa học: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateEquipmentReport(Guid reportId, UpdateReportEquipmentViewModel model)
        {
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<bool>(false,"User không tồn tại hoặc không thuộc về Agency nào.");
                }

                var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    return ResponseHandler.Success<bool>(false,"Báo cáo không tồn tại.");
                }

                if (report.AgencyId != userCurrent.AgencyId)
                {
                    return ResponseHandler.Success<bool>(false,"Bạn không có quyền cập nhật báo cáo này.");
                }

                if (report.Type != ReportTypeEnum.Equipment)
                {
                    return ResponseHandler.Success<bool>(false,"Báo cáo này không phải là báo cáo thiết bị.");
                }

                var equipments = new List<Equipment>();
                foreach (var equipmentId in model.EquipmentIds)
                {
                    var equipment = await _unitOfWork.EquipmentRepository.GetByIdAsync(equipmentId);
                    if (equipment == null)
                    {
                        return ResponseHandler.Success<bool>(false,$"Thiết bị với ID {equipmentId} không tồn tại.");
                    }
                    equipments.Add(equipment);
                }

                report.Description = model.Description;
                report.ModificationDate = _currentTime.GetCurrentTime();
                report.Equipments = equipments;

                _unitOfWork.ReportRepository.Update(report);
                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Cập nhật báo cáo thiết bị thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể cập nhật báo cáo thiết bị.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi cập nhật báo cáo thiết bị: {ex.Message}");
            }
        }
        public async Task<ApiResponse<Pagination<ReportViewModel>>> FilterReportAsync(FilterReportModel filterReportModel)
        {
            var response = new ApiResponse<Pagination<ReportViewModel>>();
            try
            {
                /*var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Failure<Pagination<ReportViewModel>>("User hoặc Agency không khả dụng!");
                }*/

                Expression<Func<Report, bool>> filter = r =>
                    (!filterReportModel.AgencyId.HasValue || r.AgencyId == filterReportModel.AgencyId) &&
                    (!filterReportModel.Status.HasValue || r.Status == filterReportModel.Status) &&
                    (!filterReportModel.FromDate.HasValue || r.CreationDate >= filterReportModel.FromDate) &&
                    (!filterReportModel.ToDate.HasValue || r.CreationDate <= filterReportModel.ToDate) &&
                    (!filterReportModel.ReportType.HasValue || r.Type == filterReportModel.ReportType);

                var reports = await _unitOfWork.ReportRepository.GetFilterAsync(
                    filter: filter,
                    pageIndex: filterReportModel.PageNumber,
                    pageSize: filterReportModel.PageSize,
                    orderBy: r => r.OrderByDescending(x => x.CreationDate)
                );

                var reportViewModels = _mapper.Map<Pagination<ReportViewModel>>(reports);

                foreach (var reportViewModel in reportViewModels.Items)
                {
                    if (reportViewModel.AgencyId.HasValue)
                    {
                        var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(reportViewModel.AgencyId.Value);
                        reportViewModel.AgencyName = agency?.Name;
                    }

                    if (reportViewModel.CourseId.HasValue)
                    {
                        var course = await _unitOfWork.CourseRepository.GetByIdAsync(reportViewModel.CourseId.Value);
                        reportViewModel.CourseName = course?.Name;
                    }

                    if (reportViewModel.Equipments != null)
                    {
                        var equipments = await _unitOfWork.ReportRepository.GetEquipmentsByReportIdAsync(reportViewModel.Id);
                        reportViewModel.Equipments = _mapper.Map<List<EquipmentViewModel>>(equipments);
                    }
                }

                if (reportViewModels.Items.IsNullOrEmpty())
                    return ResponseHandler.Success(reportViewModels, "Không tìm thấy báo cáo phù hợp!");

                response = ResponseHandler.Success(reportViewModels, "Lấy danh sách báo cáo thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<ReportViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateReportStatusAsync(Guid reportId, ReportStatusEnum newStatus)
        {
            try
            {
                var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                if (report == null)
                {
                    return ResponseHandler.Failure<bool>("Báo cáo không tồn tại.");
                }

                report.Status = newStatus;
                _unitOfWork.ReportRepository.Update(report);

                if (report.Type == ReportTypeEnum.Equipment)
                {
                    var equipments = await _unitOfWork.ReportRepository.GetEquipmentsByReportIdAsync(reportId);

                    switch (newStatus)
                    {
                        case ReportStatusEnum.Processing:
                            foreach (var equipment in equipments)
                            {
                                equipment.Status = EquipmentStatusEnum.Repair;
                                _unitOfWork.EquipmentRepository.Update(equipment);
                            }
                            break;

                        case ReportStatusEnum.Completed:
                            foreach (var equipment in equipments)
                            {
                                equipment.Status = EquipmentStatusEnum.Available;
                                _unitOfWork.EquipmentRepository.Update(equipment);
                            }
                            break;

                       

                        default:
                    
                            break;
                    }
                }

                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Cập nhật trạng thái báo cáo thành công.");
                }
                else
                {
                    return ResponseHandler.Success<bool>(false, "Không thể cập nhật trạng thái báo cáo.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi cập nhật trạng thái báo cáo: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RespondToReportAsync(Guid reportId, string response)
        {
            try
            {
                var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    return ResponseHandler.Success<bool>(false, "Báo cáo không tồn tại.");
                }

                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null)
                {
                    return ResponseHandler.Success<bool>(false, "Không thể xác định người dùng hiện tại.");
                }

                report.Response = response;
                report.Response = userCurrentId.ToString();

                _unitOfWork.ReportRepository.Update(report);


                var result = await _unitOfWork.SaveChangeAsync() > 0;
                if (result)
                {
                    var agencyManagerUserId = await _unitOfWork.AgencyRepository.GetAgencyManagerUserIdByAgencyIdAsync(report.AgencyId.Value);

                    if (agencyManagerUserId != string.Empty)
                    {
                        var noti = new SendNotificationViewModel
                        {
                            message = "Báo cáo của bạn đã được phản hồi!",
                            userIds = new List<string> { agencyManagerUserId }
                        };
                        await _notificationService.CreateAndSendNotificationNoReponseAsync(noti);
                        await _unitOfWork.SaveChangeAsync();
                    }

                    var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(report.AgencyId.Value);
                    return ResponseHandler.Success(true, "Phản hồi báo cáo thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể lưu phản hồi báo cáo.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi phản hồi báo cáo: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> DeleteReportAsync(Guid reportId)
        {
            try
            {
                var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    return ResponseHandler.Success<bool>(false,"Báo cáo không tồn tại.");
                }

                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<bool>(false,"User không tồn tại hoặc không thuộc về Agency nào.");
                }

                // Check if the user has permission to delete the report
                if (report.AgencyId != userCurrent.AgencyId && !await _userManager.IsInRoleAsync(userCurrent, AppRole.Admin))
                {
                    return ResponseHandler.Success<bool>(false,"Bạn không có quyền xóa báo cáo này.");
                }

                _unitOfWork.ReportRepository.SoftRemove(report);
                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Xóa báo cáo thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể xóa báo cáo.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi xóa báo cáo: {ex.Message}");
            }
        }
    }
}
