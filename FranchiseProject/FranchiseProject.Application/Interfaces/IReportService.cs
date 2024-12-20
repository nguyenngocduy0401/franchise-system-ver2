using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Application.ViewModels.ReportViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IReportService
    {
        Task<ApiResponse<bool>> CreateCourseReport(CreateReportCourseViewModel model);
        Task<ApiResponse<bool>> UpdateCourseReport(Guid reportId, UpdateReportCourseViewModel model);
        Task<ApiResponse<bool>> UpdateEquipmentReport(Guid reportId, UpdateReportEquipmentViewModel model);
        Task<ApiResponse<Pagination<ReportViewModel>>> FilterReportAsync(FilterReportModel filterReportModel);
        Task<ApiResponse<bool>> UpdateReportStatusAsync(Guid reportId, ReportStatusEnum newStatus);
        Task<ApiResponse<bool>> RespondToReportAsync(Guid reportId, string response);
        Task<ApiResponse<bool>> DeleteReportAsync(Guid reportId);
    }
}
