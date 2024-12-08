using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FranchiseProject.Application.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _vnPayConfig;
        private readonly ILogger<VnPayService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(
        IOptions<VnPayConfig> vnPayConfig,
        ILogger<VnPayService> logger,
        IUnitOfWork unitOfWork)
        {
            _vnPayConfig = vnPayConfig.Value ?? throw new ArgumentNullException(nameof(vnPayConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            _logger.LogInformation($"VnPayConfig: {System.Text.Json.JsonSerializer.Serialize(_vnPayConfig)}");
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
            var vnpayCreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnpayExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

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
                Amount = amount,
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

            //if (!ValidateSignature(vnpayData))
            //{
            //    _logger.LogWarning("Signature validation failed");
            //    return new PaymentResult { IsSuccess = false, Message = "Invalid signature" };
            //}

            if (callbackData.vnp_ResponseCode == "00" && callbackData.vnp_TransactionStatus == "00")
            {
                var orderId = callbackData.vnp_TxnRef;
                var payment = await _unitOfWork.PaymentRepository.GetFirstOrDefaultAsync(
                    filter: p => p.Id.ToString() == orderId);

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                 var  contract= await _unitOfWork.ContractRepository.GetExistByIdAsync(payment.ContractId.Value);
                    contract.PaidAmount = payment.Amount;
                    _unitOfWork.ContractRepository.Update(contract);
                    _unitOfWork.PaymentRepository.Update(payment);
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
                return new PaymentResult { IsSuccess = false, OrderId = callbackData.vnp_TxnRef, Message = "Payment failed" };
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
    }
}