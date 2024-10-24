using AutoMapper;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Handler.EmailTemplateHandler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ApiResponse<bool>> CreatePaymentStudent(CreateStudentPaymentViewModel create,string userId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var student = await _unitOfWork.UserRepository.GetStudentByIdAsync(userId);
                if (student == null) throw new Exception("Student does not exist!");

                //cần check Student thuộc agency 
                var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(userCurrent.AgencyId.Value);
                if (student.AgencyId != userCurrent.AgencyId)
                {
                    throw new Exception("Student does not belong to your agency!");
                }
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(create);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                if (create.Status == StudentPaymentStatusEnum.Completed)
                {
                    student.StudentStatus=StudentStatusEnum.Waitlisted;
                    var courseNames = await _unitOfWork.RegisterCourseRepository
                  .GetCourseNamesByUserIdAsync(userId);
                    var emailMessage = EmailTemplateHandler.StudentPaymentSuccsess(student.Email, student.FullName, create.Amount, agency.Name);
                    bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                    if (!emailSent)
                    {
                        response = ResponseHandler.Failure<bool>("Lỗi khi gửi mail");
                    }
                }
                var payment = _mapper.Map<Payment>(create);
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
    }
}
