using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrlFromContractPayment(PaymentContractViewModel paymentContract);
        Task<PaymentResult> ProcessPaymentCallback(VnPayCallbackViewModel callbackData);
        Task<string> CreatePaymentUrlFromContractSeacondPayment(PaymentContractViewModel paymentContract);
        Task<string> CreatePaymentUrlForAgencyCourse(AgencyCoursePaymentViewModel model);
        Task<string> CreatePaymentUrlForCourse(Guid agencyId, double amount);
    }
}
