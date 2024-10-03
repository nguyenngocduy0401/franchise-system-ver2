using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.TermViewModel;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
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

            
            var overlappingTerms = await _unitOfWork.TermRepository
                .GetOverlappingTermsAsync(createTermViewModel.StartDate.Value, createTermViewModel.EndDate.Value);

            if (overlappingTerms != null)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = "Thời gian của kỳ học bị trung với kỳ học khác!";
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

        public Task<ApiResponse<bool>> DeleteTermByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<Pagination<ClassScheduleViewModel>>> FilterTermAsync(FilterTermViewModel filterTermViewModel)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ClassScheduleViewModel>> GetTermByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateTermAsync(CreateTermViewModel createTermViewModel, string id)
        {
            throw new NotImplementedException();
        }
    }
}
