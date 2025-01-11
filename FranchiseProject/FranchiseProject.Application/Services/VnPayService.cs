using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FranchiseProject.Application.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _vnPayConfig;
        private readonly ILogger<VnPayService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IServiceProvider _serviceProvider;
        public VnPayService(
        IOptions<VnPayConfig> vnPayConfig,
        ILogger<VnPayService> logger,
        IUnitOfWork unitOfWork,
        ICurrentTime currentTime,
         IServiceProvider serviceProvider)
        {
            _vnPayConfig = vnPayConfig.Value ?? throw new ArgumentNullException(nameof(vnPayConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            _logger.LogInformation($"VnPayConfig: {System.Text.Json.JsonSerializer.Serialize(_vnPayConfig)}");
            _currentTime = currentTime;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> CreatePaymentUrlFromContractPayment(PaymentContractViewModel paymentContract)
        {
            var contract =await _unitOfWork.ContractRepository.GetExistByIdAsync(paymentContract.ContractId.Value);
            var amount = contract.Total *( contract.DepositPercentage/100);
            var paidAmount = contract.Total - amount;
            var paymentId = Guid.NewGuid();
            var vnpayTxnRef = paymentId.ToString();
            var vnpayOrderInfo = $"Thanh toan hop dong {paymentContract.ContractId}";
            var vnpayAmount = Convert.ToInt64(amount * 100);
            var vnpayLocale = "vn";
            var vnpayCreateDate = _currentTime.GetCurrentTime().ToString("yyyyMMddHHmmss");
            var vnpayExpireDate = _currentTime.GetCurrentTime().AddMinutes(15).ToString("yyyyMMddHHmmss");

            var vnpayData = new Dictionary<string, string>
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", _vnPayConfig.TmnCode},
                {"vnp_Amount", vnpayAmount.ToString()},
                {"vnp_CreateDate", vnpayCreateDate},
                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", "127.0.0.1"},
                {"vnp_Locale", vnpayLocale},
                {"vnp_OrderInfo", vnpayOrderInfo},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", _vnPayConfig.ReturnUrl},
                {"vnp_TxnRef", vnpayTxnRef},
                {"vnp_ExpireDate", vnpayExpireDate}
            };

            var orderedData = new SortedList<string, string>(vnpayData);
            var hashData = string.Join("&", orderedData.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var secureHash = ComputeHmacSha512(_vnPayConfig.HashSecret, hashData);
            vnpayData.Add("vnp_SecureHash", secureHash);

            var paymentUrl = _vnPayConfig.PaymentUrl + "?" + string.Join("&", vnpayData.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

            // Save payment information to database
            var payment = new Payment
            {
                Id = paymentId,
                Title="Thanh toán "+ contract.Title,
                Description= "Thanh toán " + contract.Title + DateTime.Now,
                Type=PaymentTypeEnum.Contract,
                Method=PaymentMethodEnum.BankTransfer,
                Amount = amount,
                ContractId = paymentContract.ContractId,
                Status = PaymentStatus.NotCompleted,
                CreationDate = DateTime.UtcNow
            };
           
            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangeAsync();

            return paymentUrl;
        }
        public async Task<string> CreatePaymentUrlFromContractSeacondPayment(PaymentContractViewModel paymentContract)
        {

            var contract = await _unitOfWork.ContractRepository.GetExistByIdAsync(paymentContract.ContractId.Value);
            var amount = contract.Total -contract.PaidAmount;
            var paidAmount = contract.PaidAmount+ amount;
            var paymentId = Guid.NewGuid();
            var vnpayTxnRef = paymentId.ToString();
            var vnpayOrderInfo = $"Thanh toán hợp đồng lần 2 {paymentContract.ContractId}";
            var vnpayAmount = Convert.ToInt64(amount * 100);
            var vnpayLocale = "vn";
            var vnpayCreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnpayExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");
         //   var returnUrl = _vnPayConfig.ReturnUrl + "/course-payment-callback";
            var vnpayData = new Dictionary<string, string>
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", _vnPayConfig.TmnCode},
                {"vnp_Amount", vnpayAmount.ToString()},
                {"vnp_CreateDate", vnpayCreateDate},
                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", "127.0.0.1"},
                {"vnp_Locale", vnpayLocale},
                {"vnp_OrderInfo", vnpayOrderInfo},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", _vnPayConfig.ReturnUrl},
                {"vnp_TxnRef", vnpayTxnRef},
                {"vnp_ExpireDate", vnpayExpireDate}
            };

            var orderedData = new SortedList<string, string>(vnpayData);
            var hashData = string.Join("&", orderedData.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var secureHash = ComputeHmacSha512(_vnPayConfig.HashSecret, hashData);
            vnpayData.Add("vnp_SecureHash", secureHash);

            var paymentUrl = _vnPayConfig.PaymentUrl + "?" + string.Join("&", vnpayData.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

            // Save payment information to database
            var payment = new Payment
            {
                Id = paymentId,
                Title = "Thanh toán " + contract.Title,
                Description = "Thanh toán " + contract.Title + DateTime.Now,
                Type = PaymentTypeEnum.Contract,
                Method = PaymentMethodEnum.BankTransfer,
                Amount = paidAmount,
                ContractId = paymentContract.ContractId,
                Status = PaymentStatus.NotCompleted,
                CreationDate = DateTime.UtcNow
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangeAsync();

            return paymentUrl;
        }

        public async Task<PaymentResult> ProcessPaymentCallback(VnPayCallbackViewModel callbackData)
        {
            var vnpayData = new Dictionary<string, string>();
            foreach (var property in callbackData.GetType().GetProperties())
            {
                var value = property.GetValue(callbackData)?.ToString() ?? "";
                vnpayData[property.Name] = value;
            }

            _logger.LogInformation($"Received callback data: {System.Text.Json.JsonSerializer.Serialize(vnpayData)}");

            if (!vnpayData.ContainsKey("vnp_SecureHash"))
            {
                _logger.LogError("vnp_SecureHash is missing from the callback data");
                return new PaymentResult { IsSuccess = false, Message = "Missing secure hash" };
            }

            // Uncomment this when you're ready to validate the signature
            // if (!ValidateSignature(vnpayData))
            // {
            //     _logger.LogWarning("Signature validation failed");
            //     return new PaymentResult { IsSuccess = false, Message = "Invalid signature" };
            // }

            if (callbackData.vnp_ResponseCode == "00" && callbackData.vnp_TransactionStatus == "00")
            {
                var orderId = callbackData.vnp_TxnRef;
                var payment = await _unitOfWork.PaymentRepository.GetFirstOrDefaultAsync(
                    filter: p => p.Id.ToString() == orderId);

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    _unitOfWork.PaymentRepository.Update(payment);

                    if (payment.ContractId.HasValue)
                    {
                        // Xử lý thanh toán hợp đồng
                        var contract = await _unitOfWork.ContractRepository.GetExistByIdAsync(payment.ContractId.Value);
                        contract.PaidAmount = payment.Amount;
                        _unitOfWork.ContractRepository.Update(contract);
                    }
                    else if (payment.RegisterCourseId.HasValue && payment.UserId != null)
                    {
                        // Xử lý thanh toán khóa học
                        var registerCourseService = _serviceProvider.GetRequiredService<IRegisterCourseService>();

                        // Check if all required values are not null
                        if (payment.UserId == null)
                        {
                            _logger.LogError("UserId is null");
                            return new PaymentResult { IsSuccess = false, OrderId = orderId, Message = "Invalid user information" };
                        }



                        if (!payment.RegisterCourseId.HasValue)
                        {
                            _logger.LogError("RegisterCourseId is null");
                            return new PaymentResult { IsSuccess = false, OrderId = orderId, Message = "Invalid registration information" };
                        }

                        var result = await registerCourseService.CompleteRegistrationAfterPayment(
                            payment.UserId,
                            payment.RegisterCourseId.Value,
                            payment.Id);

                        if (!result.isSuccess)
                        {
                            _logger.LogError($"Failed to complete registration: {result.Message}");
                            return new PaymentResult { IsSuccess = false, OrderId = orderId, Message = "Failed to complete registration" };
                        }
                    }
                    else if (payment.Type == PaymentTypeEnum.MonthlyDue)
                    {
                        // Xử lý thanh toán MontlyDue
                        var montlyDuePayment = await _unitOfWork.PaymentRepository.GetFirstOrDefaultAsync(
                            filter: mdp => mdp.Id == payment.Id);

                        if (montlyDuePayment != null)
                        {
                            montlyDuePayment.Status = PaymentStatus.Completed;
                            montlyDuePayment.PaidDate = DateTime.Now;
                            _unitOfWork.PaymentRepository.Update(montlyDuePayment);

                        }
                    }
                            await _unitOfWork.SaveChangeAsync();
                    return new PaymentResult { IsSuccess = true, OrderId = orderId, Message = "Payment successful" };
                }
                else
                {
                    return new PaymentResult { IsSuccess = false, OrderId = orderId, Message = "Payment not found" };
                }
            }
            else
            {
                var orderId = callbackData.vnp_TxnRef;
                var payment = await _unitOfWork.PaymentRepository.GetFirstOrDefaultAsync(
                    filter: p => p.Id.ToString() == orderId);

                if (payment != null && payment.Type == PaymentTypeEnum.MonthlyDue)
                {
                    // Thanh toán thất bại, tạo URL thanh toán mới
                    var paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
                    var newPaymentUrl = await paymentService.CreateOrRefreshPaymentUrl(payment);

                    if (!string.IsNullOrEmpty(newPaymentUrl))
                    {
                        payment.Status = PaymentStatus.NotCompleted;
                        _unitOfWork.PaymentRepository.Update(payment);
                        await _unitOfWork.SaveChangeAsync();

                        return new PaymentResult
                        {
                            IsSuccess = false,
                            OrderId = orderId,
                            Message = "Payment failed. A new payment URL has been created.",
                        };
                    }
                    else
                    {
                        return new PaymentResult
                        {
                            IsSuccess = false,
                            OrderId = orderId,
                            Message = "Payment failed and unable to create a new payment URL."
                        };
                    }
                }
                else
                {
                    return new PaymentResult
                    {
                        IsSuccess = false,
                        OrderId = callbackData.vnp_TxnRef,
                        Message = "Payment failed"
                    };
                }
            }
        }
        private bool ValidateSignature(Dictionary<string, string> vnpayData)
        {
            var vnpSecureHash = vnpayData["vnp_SecureHash"];
            vnpayData.Remove("vnp_SecureHash");
            vnpayData.Remove("vnp_SecureHashType");

            var orderedData = new SortedList<string, string>(vnpayData);
            var hashData = string.Join("&", orderedData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            _logger.LogInformation($"Hash data: {hashData}");

            var secureHash = ComputeHmacSha512(_vnPayConfig.HashSecret, hashData);

            _logger.LogInformation($"Computed hash: {secureHash}");
            _logger.LogInformation($"Received hash: {vnpSecureHash}");

            return vnpSecureHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeHmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public async Task<string> CreatePaymentUrlForAgencyCourse(AgencyCoursePaymentViewModel model)
        {
            var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(model.AgencyId);
            var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(model.CourseId);
            var agencyVnPayInfo = await _unitOfWork.AgencyVnPayInfoRepository.GetByAgencyIdAsync(model.AgencyId);

            if (agency == null || course == null || agencyVnPayInfo == null)
            {
                throw new ArgumentException("Agency, Course, or Agency VNPay info not found");
            }

            var amount = course.Price;
            var paymentId = Guid.NewGuid();
            var vnpayTxnRef = paymentId.ToString();
            var vnpayOrderInfo = $"Thanh toan khoa hoc {course.Name} cho dai ly {agency.Name}";
            var vnpayAmount = Convert.ToInt64(amount * 100);
            var vnpayLocale = "vn";
            var vnpayCreateDate = _currentTime.GetCurrentTime().ToString("yyyyMMddHHmmss");
            var vnpayExpireDate = _currentTime.GetCurrentTime().AddMinutes(15).ToString("yyyyMMddHHmmss");
            //var returnUrl = _vnPayConfig.ReturnUrl + "/course-payment-callback?paymentType=AgencyCourse";

            var vnpayData = new Dictionary<string, string>
    {
        {"vnp_Version", "2.1.0"},
        {"vnp_Command", "pay"},
        {"vnp_TmnCode",  agencyVnPayInfo.TmnCode}, // Use agency-specific TmnCode
        {"vnp_Amount", vnpayAmount.ToString()},
        {"vnp_CreateDate", vnpayCreateDate},
        {"vnp_CurrCode", "VND"},
        {"vnp_IpAddr", "127.0.0.1"},
        {"vnp_Locale", vnpayLocale},
        {"vnp_OrderInfo", vnpayOrderInfo},
        {"vnp_OrderType", "other"},
        {"vnp_ReturnUrl",_vnPayConfig.ReturnUrl},
        {"vnp_TxnRef", vnpayTxnRef},
        {"vnp_ExpireDate", vnpayExpireDate}
    };

            var orderedData = new SortedList<string, string>(vnpayData);
            var hashData = string.Join("&", orderedData.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var secureHash = ComputeHmacSha512(agencyVnPayInfo.HashSecret, hashData); // Use agency-specific HashSecret
            vnpayData.Add("vnp_SecureHash", secureHash);

            var paymentUrl = _vnPayConfig.PaymentUrl + "?" + string.Join("&", vnpayData.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

            var payment = new Payment
            {
                Id = paymentId,
                Title = $"Thanh toán khóa học {course.Name}",
                Description = $"Thanh toán khóa học {course.Name} cho đại lý {agency.Name} - {DateTime.Now}",
                Type = PaymentTypeEnum.Course,
                Method = PaymentMethodEnum.BankTransfer,
                Amount = amount,
                Status = PaymentStatus.NotCompleted,
                CreationDate = DateTime.UtcNow,
                AgencyId=model.AgencyId,
                RegisterCourseId=model.RegisterCourseId,
               UserId= model.UserId
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangeAsync();

            return paymentUrl;
        }
        //AgencyId - Amount-
        public async Task<string> CreatePaymentUrlForCourse(Guid agencyId,double amount,Guid paymnentId)
        {
            var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(agencyId);
           //var agencyVnPayInfo = await _unitOfWork.AgencyVnPayInfoRepository.GetByAgencyIdAsync(agencyId);

           // if (agency == null  || agencyVnPayInfo == null)
           // {
           //     throw new ArgumentException("Agency, Course, or Agency VNPay info not found");
           // }

       
           
            var vnpayTxnRef = paymnentId.ToString();
            var vnpayOrderInfo = $"Thanh toán phí chia sẻ doanh thu ";
            var vnpayAmount = Convert.ToInt64(amount * 100);
            var vnpayLocale = "vn";
            var vnpayCreateDate = _currentTime.GetCurrentTime().ToString("yyyyMMddHHmmss");
            var vnpayExpireDate = _currentTime.GetCurrentTime().AddDays(15).ToString("yyyyMMddHHmmss");

            var vnpayData = new Dictionary<string, string>
    {
        {"vnp_Version", "2.1.0"},
        {"vnp_Command", "pay"},
        {"vnp_TmnCode", _vnPayConfig.TmnCode}, // Use agency-specific TmnCode
        {"vnp_Amount", vnpayAmount.ToString()},
        {"vnp_CreateDate", vnpayCreateDate},
        {"vnp_CurrCode", "VND"},
        {"vnp_IpAddr", "127.0.0.1"},
        {"vnp_Locale", vnpayLocale},
        {"vnp_OrderInfo", vnpayOrderInfo},
        {"vnp_OrderType", "other"},
        {"vnp_ReturnUrl", _vnPayConfig.ReturnUrl},
        {"vnp_TxnRef", vnpayTxnRef},
        {"vnp_ExpireDate", vnpayExpireDate}
    };

            var orderedData = new SortedList<string, string>(vnpayData);
            var hashData = string.Join("&", orderedData.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var secureHash = ComputeHmacSha512(_vnPayConfig.HashSecret, hashData); // Use agency-specific HashSecret
            vnpayData.Add("vnp_SecureHash", secureHash);

            var paymentUrl = _vnPayConfig.PaymentUrl + "?" + string.Join("&", vnpayData.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

      

            return paymentUrl;
        }

    }
}