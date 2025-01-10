using AutoMapper;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels
{
    public class AgencyFinancialReport
    {
        public string? FiscalPeriod { get; set; } //Kỳ tài chính
        public double? Revenue { get; set; } // Doanh thu

        public double? ProfitsReceived { get; set; }//lợi nhuận đã thu
        public double? Refunds { get; set; }//Hoàn tiền
        public double? ActualProfits { get; set; }//Lợi nhuận thực tế 
        public string? OffsettingPeriod { get; set; }//Kỳ bù trừ
        public  List<CourseRevenueViewModel>? CourseRevenueViews { get; set; }
    }
}
