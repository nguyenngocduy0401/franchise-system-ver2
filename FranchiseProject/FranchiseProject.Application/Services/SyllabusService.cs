using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class SyllabusService : ISyllabusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateSyllabusModel> _updateSyllabusValidator;
        private readonly IValidator<CreateSyllabusModel> _createSyllabusValidator;
        public SyllabusService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateSyllabusModel> updateSyllabusValidator,
            IValidator<CreateSyllabusModel> createSyllabusValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateSyllabusValidator = updateSyllabusValidator;
            _createSyllabusValidator = createSyllabusValidator;
        }
        public async Task<ApiResponse<bool>> CreateSyllabusAsync(CreateSyllabusModel createSyllabusModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createSyllabusValidator.ValidateAsync(createSyllabusModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var course = await _unitOfWork.CourseRepository.GetByIdAsync((Guid)createSyllabusModel.CourseId);
                if (course == null) throw new Exception("Course does not exist!");

                var chapter = _mapper.Map<Chapter>(createSyllabusModel);
                await _unitOfWork.ChapterRepository.AddAsync(chapter);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo thấy giáo trình của khóa học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteSyllabusByIdAsync(Guid syllabusId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
                if (syllabus == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy giáo trình của khóa học!";
                    return response;
                }
                _unitOfWork.SyllabusRepository.SoftRemove(syllabus);
                response.Message = "Xoá giáo trình của khóa học thành công!";
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

        public async Task<ApiResponse<SyllabusViewModel>> GetSyllabusIdAsync(Guid syllabusId)
        {
            var response = new ApiResponse<SyllabusViewModel>();
            try
            {
                var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
                var syllabusModel = _mapper.Map<SyllabusViewModel>(syllabus);
                response.Data = syllabusModel;
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

        public async Task<ApiResponse<bool>> UpdateSyllabusAsync(Guid syllabusId, UpdateSyllabusModel updateSyllabusModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateSyllabusValidator.ValidateAsync(updateSyllabusModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
                syllabus = _mapper.Map(updateSyllabusModel, syllabus);

                _unitOfWork.SyllabusRepository.Update(syllabus);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật giáo trình của khóa học thành công!";

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
