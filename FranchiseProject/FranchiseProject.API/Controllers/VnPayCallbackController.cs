using FranchiseProject.Application;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FranchiseProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnPayCallbackController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayCallbackController(IVnPayService vnPayService,IUnitOfWork unitOfWork)
        {
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
        }

        //[HttpGet("payment-callback")]
        //public async Task<IActionResult> PaymentCallback([FromQuery] VnPayCallbackViewModel callbackData)
        //{
        //    var result = await _vnPayService.ProcessPaymentCallback(callbackData);

        //    if (result.IsSuccess)
        //    {
        //        // Redirect to a success page
        //        return Redirect($"http://localhost:5173/payment-success");
        //    }
        //    else
        //    {
        //        // Redirect to a failure page
        //        return Redirect($"https://localhost:7116//agency-manager/payment-failure?orderId={result.OrderId}&message={result.Message}");
        //    }
        //}
        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback([FromQuery] VnPayCallbackViewModel callbackData)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(Guid.Parse(callbackData.vnp_TxnRef));
            if (payment == null)
            {
                return BadRequest("Payment not found");
            }

            var result = await _vnPayService.ProcessPaymentCallback(callbackData);

            if (result.IsSuccess)
            {
                return payment.Type switch
                {
                    PaymentTypeEnum.Course => Redirect($"http://localhost:5173/course-payment-success"),
                    PaymentTypeEnum.Contract => Redirect($"http://localhost:5173/contract-payment-success"),
                    PaymentTypeEnum.MonthlyDue => Redirect($"http://localhost:5173/monthly-due-payment-success"),
                    _ => Redirect($"http://localhost:5173/payment-success")
                };
            }
            else
            {
                return payment.Type switch
                {
                    PaymentTypeEnum.Course => Redirect($"http://localhost:5173/course-payment-failure?orderId={result.OrderId}&message={result.Message}"),
                    PaymentTypeEnum.Contract => Redirect($"http://localhost:5173/contract-payment-failure?orderId={result.OrderId}&message={result.Message}"),
                    PaymentTypeEnum.MonthlyDue => Redirect($"http://localhost:5173/monthly-due-payment-failure?orderId={result.OrderId}&message={result.Message}"),
                    _ => Redirect($"http://localhost:5173/payment-failure?orderId={result.OrderId}&message={result.Message}")
                };
            }
        }

    }
}