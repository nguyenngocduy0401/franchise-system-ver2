using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateMaterialModel> _createMaterialValidator;
        private readonly IValidator<UpdateMaterialModel> _updateMaterialValidator;
        public MaterialService(IUnitOfWork unitOfWork, IMapper mapper, 
            IValidator<CreateMaterialModel> createMaterialModel, IValidator<UpdateMaterialModel> updateMaterialModel)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createMaterialValidator = createMaterialModel;
            _updateMaterialValidator = updateMaterialModel;
        }
        public async Task<ApiResponse<bool>> CreateMaterialAsync(CreateMaterialModel createMaterialModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createMaterialValidator.ValidateAsync(createMaterialModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync((Guid)createMaterialModel.CourseId);
                if (course == null) throw new Exception("Course does not exist!");

                var material = _mapper.Map<Material>(createMaterialModel);
                _unitOfWork.MaterialRepository.Update(material);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo tài nguyên học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteMaterialByIdAsync(Guid materialId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var material = await _unitOfWork.MaterialRepository.GetByIdAsync(materialId);
                if (material == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy tài nguyên!";
                    return response;
                }
                switch (material.IsDeleted)
                {
                    case false:
                        _unitOfWork.MaterialRepository.SoftRemove(material);
                        response.Message = "Xoá tài nguyên học thành công!";
                        break;
                    case true:
                        _unitOfWork.MaterialRepository.RestoreSoftRemove(material);
                        response.Message = "Phục hồi tài nguyên học thành công!";
                        break;
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
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

        public Task<ApiResponse<List<MaterialViewModel>>> GetAllMaterialAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<MaterialViewModel>> GetMaterialByIdAsync(Guid materialId)
        {
            var response = new ApiResponse<MaterialViewModel>();
            try 
            {
                var material = await _unitOfWork.MaterialRepository.GetByIdAsync(materialId);
                var matertialModel = _mapper.Map<MaterialViewModel>(material);
                response.Data = matertialModel;
                response.isSuccess = true;
                response.Message = "Successful!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateMaterialAsync(Guid materialId, UpdateMaterialModel updateMaterialModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateMaterialValidator.ValidateAsync(updateMaterialModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var material = await _unitOfWork.MaterialRepository.GetExistByIdAsync(materialId);
                material = _mapper.Map(updateMaterialModel, material);
                _unitOfWork.MaterialRepository.Update(material);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật tài nguyên học thành công!";

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
