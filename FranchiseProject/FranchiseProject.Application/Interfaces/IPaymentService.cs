using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentStudentViewModel = FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentStudentViewModel;

namespace FranchiseProject.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<bool>> CreatePaymentStudent(CreateStudentPaymentViewModel create, StudentPaymentStatusEnum status);
        Task<ApiResponse<Pagination<PaymentStudentViewModel>>> FilterPaymentAsync(FilterStudentPaymentViewModel filterModel);
        Task<ApiResponse<PaymentStudentViewModel>> GetPaymentByIdAsync(string paymentId);
        Task<ApiResponse<Pagination<PaymentStudentViewModel>>> GetPaymentByLoginAsync(int pageIndex = 1, int pageSize = 10);
        Task<ApiResponse<bool>> UpdateStudentPaymentStatusAsync(Guid registerCourseId, StudentPaymentStatusEnum newStatus);
        Task<ApiResponse<bool>> CreatePaymentContractDirect(CreateContractDirect create);

    }
}
