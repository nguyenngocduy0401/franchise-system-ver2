using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.PackageViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePackageModel> _createPackageValidator;
        private readonly IValidator<UpdatePackageModel> _updatePackageValidator;
        public PackageService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CreatePackageModel>  createPackageValidator, IValidator<UpdatePackageModel> updatePackageValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createPackageValidator = createPackageValidator;
            _updatePackageValidator = updatePackageValidator;
        }
        public async Task<ApiResponse<List<PackageViewModel>>> GetAllStandardPackageByAsync()
        {
            var response = new ApiResponse<List<PackageViewModel>>();
            try
            {
                Func<IQueryable<Package>, IOrderedQueryable<Package>> ? orderBy = e => e.OrderBy(p => p.Price);
                var package = await _unitOfWork.PackageRepository.GetAllAsync(orderBy);

                var packageViews = _mapper.Map<List<PackageViewModel>>(package);
                response = ResponseHandler.Success(packageViews);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<PackageViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreatePackageAsync(CreatePackageModel createPackageModel) 
        {
            var response = new ApiResponse<bool>(); 
            try 
            {
                ValidationResult validationResult =await _createPackageValidator.ValidateAsync(createPackageModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var package = _mapper.Map<Package>(createPackageModel);
                await _unitOfWork.PackageRepository.AddAsync(package);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (isSuccess) ResponseHandler.Failure<bool>("Failed to create package");
                response = ResponseHandler.Success(true);

            }catch(Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdatePackageAsync(Guid id, UpdatePackageModel updatePackageModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updatePackageValidator.ValidateAsync(updatePackageModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var package = await _unitOfWork.PackageRepository.GetExistByIdAsync(id);
                if(package == null) return ResponseHandler.Success(false, "Gói nhượng quyền không khả dụng không khả dụng!");

                package = _mapper.Map(updatePackageModel, package);
                await _unitOfWork.PackageRepository.AddAsync(package);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (isSuccess) ResponseHandler.Failure<bool>("Failed to create package");
                response = ResponseHandler.Success(true);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<PackageViewModel>> GetPackageByIdAsync(Guid id)
        {
            var response = new ApiResponse<PackageViewModel>();
            try
            {
                var package = await _unitOfWork.PackageRepository.GetByIdAsync(id);
                if (package == null) return ResponseHandler.Failure<PackageViewModel>("Package not found");

                var packageView = _mapper.Map<PackageViewModel>(package);
                response = ResponseHandler.Success(packageView);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<PackageViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeletePackageByIdAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var package = await _unitOfWork.PackageRepository.GetExistByIdAsync(id);
                if (package == null) return ResponseHandler.Success(false, "Gói nhượng quyền không khả dụng không khả dụng!");

                _unitOfWork.PackageRepository.SoftRemove(package);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá gói nhượng quyền học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}
