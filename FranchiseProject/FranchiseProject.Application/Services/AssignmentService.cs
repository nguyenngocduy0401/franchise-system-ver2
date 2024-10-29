using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public AssignmentService( IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateAssignmentViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
         
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
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo slot học thành công!");
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
                if (ass == null) return ResponseHandler.Failure<bool>("Slot học không khả dụng!");

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
                    return ResponseHandler.Failure<Pagination<AssignmentViewModel>>("Không tìm thấy bài tập nào cho lớp này.");
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
    }
}
