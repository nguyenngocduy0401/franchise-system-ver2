using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<bool>> CreatePaymentStudent(CreateStudentPaymentViewModel create,string userId);

    }
}
