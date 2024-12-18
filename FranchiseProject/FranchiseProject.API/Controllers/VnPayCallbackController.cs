using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FranchiseProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnPayCallbackController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public VnPayCallbackController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback([FromQuery] VnPayCallbackViewModel callbackData)
        {
            var result = await _vnPayService.ProcessPaymentCallback(callbackData);

            if (result.IsSuccess)
            {
                // Redirect to a success page
                return Redirect($"https://future-tech-franchise.vercel.app/agency-manager/payment-success?orderId={result.OrderId}");
            }
            else
            {
                // Redirect to a failure page
                return Redirect($"https://future-tech-franchise.vercel.app/agency-manager/payment-failure?orderId={result.OrderId}&message={result.Message}");
            }
        }
    }
}