using AutoMapper;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;

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

namespace FranchiseProject.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly  IMapper _mapper;
        private readonly  IValidator<CreateStudentPaymentViewModel> _validator;
        private readonly  IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        public PaymentService(UserManager<User> userManager,IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateStudentPaymentViewModel>validator
            ,IEmailService emailService,IHubContext<NotificationHub>hubContext,IClaimsService claimsService)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _hubContext = hubContext;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<ApiResponse<bool>> CreatePaymentStudent(CreateStudentPaymentViewModel create,   StudentPaymentStatusEnum status )
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var student = await _userManager.FindByIdAsync(create.UserId);
                if (student == null) throw new Exception("Student does not exist!");

                //cần check Student thuộc agency 
                var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(userCurrent.AgencyId.Value);
                if (student.AgencyId != userCurrent.AgencyId)
                {
                    throw new Exception("Student does not belong to your agency!");
                }
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(create);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                if (status == StudentPaymentStatusEnum.Completed)
                {
                    if (student.StudentStatus == StudentStatusEnum.Pending)
                    {
                        student.StudentStatus = StudentStatusEnum.Waitlisted;
                        await _userManager.UpdateAsync(student);
                        var registerCourses = await _unitOfWork.RegisterCourseRepository
                             .GetRegisterCoursesByUserIdAndStatusNullAsync(student.Id); 

                        if (registerCourses == null || !registerCourses.Any())
                        {
                            response = ResponseHandler.Failure<bool>("Không có khóa học nào với trạng thái null cho học sinh này.");
                            return response;
                        }

                        foreach (var registerCourse in registerCourses)
                        {
                            if (registerCourse.StudentCourseStatus == StudentCourseStatusEnum.Pending)
                            {
                                registerCourse.StudentCourseStatus = StudentCourseStatusEnum.NotStudied;
                                await _unitOfWork.RegisterCourseRepository.Update1Async(registerCourse);
                                break; // Stop after updating the first "Pending" status
                            }
                        }
                        var courseNames = await _unitOfWork.RegisterCourseRepository
                      .GetCourseNamesByUserIdAsync(create.UserId);
                        var random = new Random();
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        var password = new string(Enumerable.Repeat(chars, 6)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                        var user1=  _userManager.AddPasswordAsync(student, password);
                        var emailMessage = EmailTemplate.StudentPaymentSuccsess(student.Email, student.FullName, create.Amount, agency.Name,student.UserName, password);
                        bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                        if (!emailSent)
                        {
                            response = ResponseHandler.Failure<bool>("Lỗi khi gửi mail"); return response;
                        }
                    }
                    else
                    {
                        response = ResponseHandler.Failure<bool>("Không thể chuyển trạng thái học sinh"); return response;
                    }
                }
                else if (status == StudentPaymentStatusEnum.Pending_Payment)
                {
                    if (student.StudentStatus == StudentStatusEnum.Pending)
                    {
                        //student.StudentStatus = StudentStatusEnum.Waitlisted;
                        var courseNames = await _unitOfWork.RegisterCourseRepository
                      .GetCourseNamesByUserIdAsync(create.UserId);
                        var emailMessage = EmailTemplate.StudentPaymentSuccsessNotCompleted(student.Email, student.FullName, create.Amount, agency.Name);
                        bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                        if (!emailSent)
                        {
                            response = ResponseHandler.Failure<bool>("Lỗi khi gửi mail");
                            return response;
                        }
                    }
                    else
                    {
                        response = ResponseHandler.Failure<bool>("Không thể chuyển trạng thái học sinh");
                        return response;
                    }
                }
                var payment = _mapper.Map<Payment>(create);
                student.StudentPaymentStatus = status;
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo hóa đơn  thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<PaymentStudentViewModel>>> FilterPaymentAsync(FilterStudentPaymentViewModel filterModel)
        {
            var response = new ApiResponse<Pagination<PaymentStudentViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var currentAgencyId = userCurrent.AgencyId;
                var usersInAgency = await _unitOfWork.UserRepository.GetAllAsync(u =>
                    u.AgencyId == currentAgencyId &&
                    (string.IsNullOrEmpty(filterModel.StudentName) || u.FullName.Contains(filterModel.StudentName))
                );

                var userIdsInAgency = usersInAgency.Select(u => u.Id).ToList();
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync(p =>
                    userIdsInAgency.Contains(p.UserId),
                    orderBy: q => q.OrderByDescending(p => p.CreatedDate
                );
                var paymentViewModels = _mapper.Map<List<PaymentStudentViewModel>>(payments);
                var pagedResult = new Pagination<PaymentStudentViewModel>
                {
                    TotalItemsCount = paymentViewModels.Count,
                    PageSize = filterModel.PageSize,
                    PageIndex = filterModel.PageIndex,
                    Items = paymentViewModels
                        .Skip((filterModel.PageIndex - 1) * filterModel.PageSize)
                        .Take(filterModel.PageSize)
                        .ToList()
                };

                response = ResponseHandler.Success(pagedResult, "Lọc thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<PaymentStudentViewModel>>(ex.Message);
            }
            return response;
        }



        public async Task<ApiResponse<PaymentStudentViewModel>> GetPaymentByIdAsync(string paymentId)
        {
            var response = new ApiResponse<PaymentStudentViewModel>();
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(Guid.Parse (paymentId));
                if (payment == null)
                {
                    return ResponseHandler.Failure<PaymentStudentViewModel>("Không tìm thấy thanh toán với ID này!");
                }
                var paymentViewModel = _mapper.Map<PaymentStudentViewModel>(payment);

                response = ResponseHandler.Success(paymentViewModel, "Lấy thông tin thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<PaymentStudentViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<PaymentStudentViewModel>>> GetPaymentByLoginAsync(int pageIndex = 1, int pageSize = 10)
        {
            var response = new ApiResponse<Pagination<PaymentStudentViewModel>>();
            try
            {
              
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();

                var payments = await _unitOfWork.PaymentRepository.GetAllAsync(p => p.UserId == userCurrentId);

            
                var paymentViewModels = _mapper.Map<List<PaymentStudentViewModel>>(payments);

          
                var pagedResult = new Pagination<PaymentStudentViewModel>
                {
                    TotalItemsCount = paymentViewModels.Count,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = paymentViewModels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                response = ResponseHandler.Success(pagedResult, "Lấy danh sách thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<PaymentStudentViewModel>>(ex.Message);
            }
            return response;
        }



    }
}
