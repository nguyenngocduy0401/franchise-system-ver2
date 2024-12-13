using FranchiseProject.Application;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Domain.Enums;
using FranchiseProject.Infrastructures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
  
    public class PaymentController
    {
        private readonly IVnPayService _vnPayService;
        private IUnitOfWork _unitOfwork;
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService,IVnPayService vnPayService,IUnitOfWork unitOfWork) { _paymentService = paymentService;_vnPayService = vnPayService; _unitOfwork = unitOfWork; }
        [SwaggerOperation(Summary = "tạo lịch sử thanh toán (dùng cho Agency) {Authorize = AgencyManager,AgencyStaff}  ")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> CreatePaymentStudent([FromBody]CreateStudentPaymentViewModel create, StudentPaymentStatusEnum status) => await _paymentService.CreatePaymentStudent(create, status);
        [SwaggerOperation(Summary = "Truy xuất thông tin thanh toán của student  (dùng cho Agency) {Authorize = AgencyManager,AgencyStaff}  ")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("filter")]
         public async   Task<ApiResponse<Pagination<PaymentStudentViewModel>>> FilterPaymentAsync([FromQuery]FilterStudentPaymentViewModel filterModel)=> await _paymentService.FilterPaymentAsync(filterModel);
        [SwaggerOperation(Summary = "Truy xuất thông tin thanh toán của student By Id (dùng cho Agency)   ")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ApiResponse<PaymentStudentViewModel>> GetPaymentByIdAsync(string id) => await _paymentService.GetPaymentByIdAsync(id);
        [SwaggerOperation(Summary = "Truy xuất thông tin thanh toán của student By Login (dùng cho Agency)   ")]
        [Authorize(Roles = AppRole.Student )]
        [HttpGet("mine")]
        public async Task<ApiResponse<Pagination<PaymentStudentViewModel>>> GetPaymentByLoginAsync(int pageIndex = 1, int pageSize = 10)=>await _paymentService.GetPaymentByLoginAsync(pageIndex, pageSize);
        [SwaggerOperation(Summary = "cập nhật trạng thái thanh toán (dùng cho Agency)   ")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateStudentPaymentStatusAsync(Guid id, StudentPaymentStatusEnum newStatus)=> await _paymentService.UpdateStudentPaymentStatusAsync(id, newStatus);

        [SwaggerOperation(Summary = "tạo thanh toán hợp đồng trực tiếp {Authorize = Manager}  ")]
        [Authorize(Roles = AppRole.Manager)]
        [HttpPost("/direct")]
        public async Task<ApiResponse<bool>> CreatePaymentContractDirectStudent(CreateContractDirect create) => await _paymentService.CreatePaymentContractDirect(create);
        //---------------------VNPay----------------------------------

        [HttpPost("create-vnpay-url")]
        [SwaggerOperation(Summary = "Tạo URL thanh toán hợp đồng lần 1 VnPay")]
        public async Task<ApiResponse<string>> CreateVnPayUrl([FromBody] PaymentContractViewModel paymentContract)
        {

            try
            {
                var isPay = await _unitOfwork.ContractRepository.IsDepositPaidCorrectlyAsync(paymentContract.ContractId.Value);
                if (isPay)
                {

                    return ResponseHandler.Success<string>(null, "Hợp đồng đã được thanh toán lần 1!");

                }
                var paymentUrl = await _vnPayService.CreatePaymentUrlFromContractPayment(paymentContract);
                return ResponseHandler.Success(paymentUrl, "URL thanh toán VnPay đã được tạo thành công.");
            }
            catch (ArgumentException ex)
            {
                return ResponseHandler.Failure<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>("Đã xảy ra lỗi khi xử lý yêu cầu của bạn.");
            }
        }
        [HttpPost("create-vnpay-url-2")]
        [SwaggerOperation(Summary = "Tạo URL thanh toán hợp đồng lần 2 VnPay")]
        public async Task<ApiResponse<string>> CreateContractSecondVnPayUrl([FromBody] PaymentContractViewModel paymentContract)
        {

            try
            {
                var isPay = await _unitOfwork.ContractRepository.IsCompletedPaidCorrectlyAsync(paymentContract.ContractId.Value);
                if (isPay)
                {

                    return ResponseHandler.Success<string>(null, "Hợp đồng đã được thanh toán lần 2!");

                }
                var paymentUrl = await _vnPayService.CreatePaymentUrlFromContractSeacondPayment(paymentContract);
                return ResponseHandler.Success(paymentUrl, "URL thanh toán VnPay đã được tạo thành công.");
            }
            catch (ArgumentException ex)
            {
                return ResponseHandler.Failure<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>("Đã xảy ra lỗi khi xử lý yêu cầu của bạn.");
            }
        }
        //   [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Lọc thanh toán hợp đồng")]
        [HttpGet("contract/filter")]
        public async Task<ApiResponse<Pagination<PaymentContractAgencyViewModel>>> FilterPaymentContractAsync([FromQuery] FilterContractPaymentViewModel filterModel)
        {
            return await _paymentService.FilterPaymentContractAsync(filterModel);
        }
    }
}

