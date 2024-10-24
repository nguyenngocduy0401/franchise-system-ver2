using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ClassScheduleService : IClassScheduleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateClassScheduleViewModel> _validator1;
        private readonly IValidator<CreateClassScheduleDateRangeViewModel> _validator2;

        public ClassScheduleService(IValidator<CreateClassScheduleDateRangeViewModel> validator2, IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateClassScheduleViewModel> validator1)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator1 = validator1;
            _validator2 = validator2;
        }

        public async Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator1.ValidateAsync(createClassScheduleViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                DateTime? date = DateTime.Parse(createClassScheduleViewModel.Date);
                var existingSchedule = await _unitOfWork.ClassScheduleRepository
                        .GetExistingScheduleAsync((DateTime)date, createClassScheduleViewModel.Room, Guid.Parse(createClassScheduleViewModel.SlotId));

                if (existingSchedule != null)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = $"Đã tồn tại lớp học .";
                    return response;
                }
                var classSchedule = _mapper.Map<ClassSchedule>(createClassScheduleViewModel);
                await _unitOfWork.ClassScheduleRepository.AddAsync(classSchedule);
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

        public async Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync(CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator2.ValidateAsync(createClassScheduleDateRangeViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
               
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Tạo lịch học thất bại!");

                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo lịch học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteClassScheduleByIdAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetExistByIdAsync(Guid.Parse(id));
                if (classSchedule == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy slot!";
                    return response;
                }
               
               
                _unitOfWork.ClassScheduleRepository.SoftRemove(classSchedule);
                response.Message = "Xoá class schedule học thành công!";
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

        public async Task<ApiResponse<Pagination<ClassScheduleViewModel>>> FilterClassScheduleAsync(FilterClassScheduleViewModel filterClassScheduleViewModel)
        {
            var response = new ApiResponse<Pagination<ClassScheduleViewModel>>();
            try
            {
                DateTime? start = null;
                DateTime? end = null;


                if (!string.IsNullOrEmpty(filterClassScheduleViewModel.StartDate))
                {
                    start = DateTime.Parse(filterClassScheduleViewModel.StartDate);
                }

                if (!string.IsNullOrEmpty(filterClassScheduleViewModel.EndDate))
                {
                    end = DateTime.Parse(filterClassScheduleViewModel.EndDate);
                }

                Expression<Func<ClassSchedule, bool>> filter = s =>
                    (!start.HasValue || s.Date >= start.Value) &&
                    (!end.HasValue || s.Date <= end.Value);


                var schedules = await _unitOfWork.ClassScheduleRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties: "Class,Slot"
               
                );

                var scheduleViewModels = _mapper.Map<Pagination<ClassScheduleViewModel>>(schedules);
              /*  foreach (var schedule in scheduleViewModels.Items)
                {
                    schedule.Date = DateTime.Parse(schedule.Date).ToString("d/M/yyyy"); // Định dạng lại ngày
                }*/

                response.Data = scheduleViewModels;
                response.isSuccess = true;
                response.Message = "Lấy danh sách ClassSchedule thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleByIdAsync(string id)
        {
            var response = new ApiResponse<ClassScheduleViewModel>();
            try
            {
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetByIdAsync(Guid.Parse(id));
                if (classSchedule == null)
                {
                    response.Data = null;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy class schedule!";
                    return response;
                }
                var slot =await _unitOfWork.SlotRepository.GetByIdAsync(classSchedule.SlotId.Value);
                var class1 =await _unitOfWork.ClassRepository.GetByIdAsync(classSchedule.ClassId.Value);
                var clasScheduleViewModel =  _mapper.Map<ClassScheduleViewModel>(classSchedule);
         /*       clasScheduleViewModel.SlotName=slot.Name;
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(classSchedule.SlotId.Value);
                var class1 = await _unitOfWork.ClassRepository.GetByIdAsync(classSchedule.ClassId.Value);
                var clasScheduleViewModel = _mapper.Map<ClassScheduleViewModel>(classSchedule);
                clasScheduleViewModel.SlotName = slot.Name;
                clasScheduleViewModel.ClassName = class1.Name;
                clasScheduleViewModel.Date = classSchedule.Date.Value.ToString("d/M/yyyy");*/

                response.Data = clasScheduleViewModel;
                response.isSuccess = true;
                response.Message = "tìm class schedule học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateClassScheduleAsync(CreateClassScheduleViewModel updateClassScheduleViewModel, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator1.ValidateAsync(updateClassScheduleViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetByIdAsync(Guid.Parse(id));
                _mapper.Map(updateClassScheduleViewModel, classSchedule);
                _unitOfWork.ClassScheduleRepository.Update(classSchedule);
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
