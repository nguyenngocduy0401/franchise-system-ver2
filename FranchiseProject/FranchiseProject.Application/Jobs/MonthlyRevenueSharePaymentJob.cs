using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Domain.Enums;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Jobs
{
    public class MonthlyRevenueSharePaymentJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public MonthlyRevenueSharePaymentJob(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var activeAgencies = await _unitOfWork.AgencyRepository.GetAllAsync1(a => a.Status == AgencyStatusEnum.Active);

            foreach (var agency in activeAgencies)
            {
                var revenue = await _unitOfWork.PaymentRepository.CalculateAgencyRevenue(agency.Id, startDate, endDate);//hàm tính tiền 

                var model = new CreatePaymentMontlyViewModel
                {
                    AgencyId = agency.Id,
                    Amount = revenue,
                    Title = $"Thanh toán phí chia sẻ doanh thu tháng {startDate:MM/yyyy}",
                    Description = $"Thanh toán phí chia sẻ doanh thu từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}"
                };

                await _paymentService.CreateMonthlyRevenueSharePayment(model);
            }
        }
    }
}
