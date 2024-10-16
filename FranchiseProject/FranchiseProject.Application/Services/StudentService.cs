using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using System.Data.Common;

namespace FranchiseProject.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateStudentViewModel> _validator;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;

        public StudentService(IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateStudentViewModel> validator, IUserService userService, IMapper mapper, IHubContext<NotificationHub> hubContext, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _userService = userService;
            _mapper = mapper;
            _hubContext = hubContext;
            _emailService = emailService;
        }

       




        public async Task<ApiResponse<bool>> CreateStudentAsync(CreateStudentViewModel createStudentViewModel)
        {
            var response= new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(createStudentViewModel);
                if (!validationResult.IsValid)
                {
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var student=_mapper.Map<Student>(createStudentViewModel);
                student.Status=StudentStatusEnum.pending;
                await _unitOfWork.StudentRepository.AddAsync(student);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public Task<ApiResponse<bool>> DeleteStudentAsync(string studentId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<Pagination<StudentViewModel>>> FilterStudentAsync(FilterStudentViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<StudentViewModel>> GetStudentByIdAsync(string studentId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> StudentEnrollClass()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> StudentEnrollClassAsync(string classId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateStatusStudentAsync(StudentStatusEnum status)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateStudentAsync(CreateStudentViewModel updateStudentViewModel)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResponse<Pagination<ClassForStudentViewModel>>> GetClassForStudentAsync(string studentId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<int>> CountStudenInCourseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
