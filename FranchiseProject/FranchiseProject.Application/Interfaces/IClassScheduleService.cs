﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AttendanceViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IClassScheduleService
    {
        Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel);
        Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync(CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel);
        Task<ApiResponse<bool>> DeleteClassScheduleByIdAsync(string id);
        Task<ApiResponse<List<ClassScheduleViewModel>>> FilterClassScheduleAsync(FilterClassScheduleViewModel filterClassScheduleViewModel);
        Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleByIdAsync(string id);
        Task<ApiResponse<bool>> UpdateClassScheduleAsync(CreateClassScheduleViewModel updateClassScheduleViewModel, string id);
        Task<ApiResponse<bool>> DeleteClassSheduleAllByClassIdAsync(string classId);
        Task<ApiResponse<ClassScheduleDetailViewModel>> GetClassScheduleDetailAsync(Guid id);
        Task<ApiResponse<ClassScheduleByLoginViewModel>> GetClassScheduleByLoginAsync(Guid classId);


    }
}
