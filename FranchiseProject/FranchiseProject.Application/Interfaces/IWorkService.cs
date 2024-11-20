﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IWorkService
    {
        Task<ApiResponse<bool>> CreateWorkAsync(CreateWorkModel createWorkModel);
        Task<ApiResponse<bool>> DeleteWorkByIdAsync(Guid workId);
        Task<ApiResponse<IEnumerable<WorkViewModel>>> GetAllWorkByAgencyId(Guid agencyId);
        Task<ApiResponse<bool>> UpdateWorkAsync(Guid workId, UpdateWorkModel updateWorkModel);
        Task<ApiResponse<WorkDetailViewModel>> GetWorkDetailByIdAsync(Guid id);
        Task<ApiResponse<Pagination<WorkViewModel>>> FilterWorksByLogin(FilterWorkByLoginModel filterWorkByLoginModel);
        Task<ApiResponse<bool>> UpdateStatusWorkByIdAsync(Guid workId, WorkStatusEnum status);
    }
}
