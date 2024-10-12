using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.TermViewModel;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace FranchiseProject.Application.Services
{
    public class TermService : ITermService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateTermViewModel> _validator;
  public TermService(IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateTermViewModel> validator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async  Task<ApiResponse<bool>> CreateTermAsync(CreateTermViewModel createTermViewModel)
        {
            var response = new ApiResponse<bool>();
            try 
            { 
            var existingTermByName = await _unitOfWork.TermRepository
            .GetByNameAsync(createTermViewModel.Name);

            if (existingTermByName != null)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = "Tên kỳ học đã tồn tại!";
                return response;
            }


                var hasOverlappingTerms = await _unitOfWork.TermRepository
               .HasOverlappingTermsAsync(createTermViewModel.StartDate.Value, createTermViewModel.EndDate.Value);

                if (hasOverlappingTerms)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Thời gian của kỳ học bị trùng với kỳ học khác!";
                    return response;
                }

                var term = _mapper.Map<Term>(createTermViewModel);
                await _unitOfWork.TermRepository.AddAsync(term);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo  thành công!";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
           }

        public async Task<ApiResponse<bool>> DeleteTermByIdAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classSchedule = await _unitOfWork.TermRepository.GetExistByIdAsync(Guid.Parse(id));
                if (classSchedule == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy học kỳ!";
                    return response;
                }

                switch (classSchedule.IsDeleted)
                {
                    case false:
                        _unitOfWork.TermRepository.SoftRemove(classSchedule);
                        response.Message = "Xoá  học kỳ  thành công!";
                        break;
                    case true:
                        _unitOfWork.TermRepository.RestoreSoftRemove(classSchedule);
                        response.Message = "Phục hồi  học kỳ thành công!";
                        break;
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete fail!");
                response.Data = true;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<TermViewModel>>> FilterTermAsync(FilterTermViewModel filterTermViewModel)
        {
            var response = new ApiResponse<Pagination<TermViewModel>>();
            try
            {
                DateTime? start = null;
                DateTime? end = null;


                if (!string.IsNullOrEmpty(filterTermViewModel.StartDate))
                {
                    start = DateTime.Parse(filterTermViewModel.StartDate);
                }

                if (!string.IsNullOrEmpty(filterTermViewModel.EndDate))
                {
                    end = DateTime.Parse(filterTermViewModel.EndDate);
                }

                Expression<Func<Term, bool>> filter = s =>
                    (!start.HasValue || s.StartDate >= start.Value) &&
                    (!end.HasValue || s.EndDate <= end.Value) &&
                (!filterTermViewModel.isDeleted.HasValue || s.IsDeleted == filterTermViewModel.isDeleted);

                var term = await _unitOfWork.TermRepository.GetFilterAsync(
                    filter: filter,
                    pageIndex: filterTermViewModel.PageIndex,
                    pageSize: filterTermViewModel.PageSize
                );

                var termViewModels = _mapper.Map<Pagination<TermViewModel>>(term);


                response.Data = termViewModels;
                response.isSuccess = true;
                response.Message = "Lấy danh sách học kỳ thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Pagination<TermViewModel>>> GetAllTermAsync(int pageSize, int pageIndex)
        {

            var response = new ApiResponse<Pagination<TermViewModel>>();
            try
            {
                var term = await _unitOfWork.TermRepository.GetAllAsync();
                var termViewModel = _mapper.Map<Pagination<TermViewModel>>(term);
                response.Data = termViewModel;
                response.isSuccess = true;
                response.Message = "truy xuất học kỳ thành công!";

            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<TermViewModel>> GetTermByIdAsync(string id)
        {
            var response = new ApiResponse<TermViewModel>();
            try
            {
                var classSchedule = await _unitOfWork.TermRepository.GetExistByIdAsync(Guid.Parse(id));
                if (classSchedule == null)
                {
                    response.Data = null;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy học kỳ!";
                    return response;
                }
      
                var termViewModel = _mapper.Map<TermViewModel>(classSchedule);
         

                response.Data = termViewModel;
                response.isSuccess = true;
                response.Message = "tìm kỳ  học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateTermAsync(CreateTermViewModel createTermViewModel, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(createTermViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var term = await _unitOfWork.TermRepository.GetExistByIdAsync(Guid.Parse(id));
                _mapper.Map(createTermViewModel, term);
                _unitOfWork.TermRepository.Update(term);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Cập nhật thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
