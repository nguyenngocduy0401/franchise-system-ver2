using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class AgencyDashboardService : IAgencyDashboardService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateClassViewModel> _validator;
        private readonly IValidator<UpdateClassViewModel> _validatorUpdate;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFirebaseRepository _firebaseService;
        public AgencyDashboardService(IFirebaseRepository firebaseService,IValidator<UpdateClassViewModel> validatorUpdate, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateClassViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
            _validatorUpdate = validatorUpdate;
            _firebaseService = firebaseService;
        }
        #endregion
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<List<CourseRevenueViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId);
                var agencyId = userCurrent.AgencyId;
                var courseRevenueList = new List<CourseRevenueViewModel>();
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(agencyId.Value);
                var courseList = await _unitOfWork.CourseRepository.GetAllAsync();
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId.Value);
                if (contract == null)
                {
                    return ResponseHandler.Success<List<CourseRevenueViewModel>>(null, "Không tìm thấy hợp đồng !");
                }

                var courseGroups = courseList.GroupBy(c => c.Code);

                foreach (var courseGroup in courseGroups)
                {
                    var totalRevenue = 0.0;
                    var studentCount = 0;
                    var totalRefunds = 0.0;

                    foreach (var course in courseGroup)
                    {
                        var filteredRegisterCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCourseByCourseIdAsync(course.Id, contract.AgencyId.Value);
                        var validRegistrations = filteredRegisterCourses
                            .Where(r => r.CreationDate.Date <= endDate.Date)
                            .ToList();

                        foreach (var registration in validRegistrations)
                        {
                            studentCount++;
                            var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                            foreach (var payment in payments)
                            {
                                if (payment.ToDate.HasValue && payment.ToDate.Value >= DateOnly.FromDateTime(startDate) && payment.ToDate.HasValue && payment.ToDate.Value <= DateOnly.FromDateTime(endDate))
                                {
                                    if (payment.Type == PaymentTypeEnum.Refund)
                                    {
                                        totalRefunds += payment.Amount ?? 0;
                                    }
                                    else
                                    {
                                        totalRevenue += payment.Amount ?? 0;
                                    }
                                }
                            }
                        }
                    }

                    var monthlyFee = totalRevenue * (contract.RevenueSharePercentage/100);
                    var actualProfits = totalRevenue - totalRefunds - monthlyFee;

                    // Sử dụng thông tin từ phiên bản mới nhất của khóa học
                    var latestCourse = courseGroup.OrderByDescending(c => c.CreationDate).First();

                    courseRevenueList.Add(new CourseRevenueViewModel
                    {
                        CourseId = latestCourse.Id,
                        StudentCount = studentCount,
                        CourseCode = latestCourse.Code,
                        CourseName = latestCourse.Name,
                        TotalRevenue = Math.Round(totalRevenue, 2),
                        MonthlyFee = monthlyFee.HasValue ? (double?)Math.Round((decimal)monthlyFee.Value, 2) : null,
                        Refunds = Math.Round(totalRefunds, 2),
                        ActualProfits = actualProfits.HasValue ? (double?)Math.Round((decimal)actualProfits.Value, 2) : null,


                    });
                }

                response = ResponseHandler.Success(courseRevenueList, "Tính doanh thu khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<CourseRevenueViewModel>>($"Lỗi khi tính doanh thu khóa học: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAgencyIdAsync(Guid AgencyId, DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<List<CourseRevenueViewModel>>();
            try
            {
               
                var agencyId = AgencyId;
                var courseRevenueList = new List<CourseRevenueViewModel>();
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(agencyId);
                var courseList = await _unitOfWork.CourseRepository.GetAllAsync();
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                if (contract == null)
                {
                    return ResponseHandler.Success<List<CourseRevenueViewModel>>(null, "Không tìm thấy hợp đồng !");
                }

                var courseGroups = courseList.GroupBy(c => c.Code);

                foreach (var courseGroup in courseGroups)
                {
                    var totalRevenue = 0.0;
                    var studentCount = 0;
                    var totalRefunds = 0.0;

                    foreach (var course in courseGroup)
                    {
                        var filteredRegisterCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCourseByCourseIdAsync(course.Id, contract.AgencyId.Value);
                        var validRegistrations = filteredRegisterCourses
                            .Where(r => r.CreationDate.Date <= endDate.Date)
                            .ToList();

                        foreach (var registration in validRegistrations)
                        {
                            studentCount++;
                            var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                            foreach (var payment in payments)
                            {
                                if (payment.ToDate.HasValue && payment.ToDate.Value >= DateOnly.FromDateTime(startDate) && payment.ToDate.HasValue && payment.ToDate.Value <= DateOnly.FromDateTime(endDate))
                                {
                                    if (payment.Type == PaymentTypeEnum.Refund)
                                    {
                                        totalRefunds += payment.Amount ?? 0;
                                    }
                                    else
                                    {
                                        totalRevenue += payment.Amount ?? 0;
                                    }
                                }
                            }
                        }
                    }

                    var monthlyFee = totalRevenue * (contract.RevenueSharePercentage / 100);
                    var actualProfits = totalRevenue - totalRefunds - monthlyFee;

                    // Sử dụng thông tin từ phiên bản mới nhất của khóa học
                    var latestCourse = courseGroup.OrderByDescending(c => c.CreationDate).First();

                    courseRevenueList.Add(new CourseRevenueViewModel
                    {
                        CourseId = latestCourse.Id,
                        StudentCount = studentCount,
                        CourseCode = latestCourse.Code,
                        CourseName = latestCourse.Name,
                        TotalRevenue = Math.Round(totalRevenue, 2),
                        MonthlyFee = monthlyFee.HasValue ? (double?)Math.Round((decimal)monthlyFee.Value, 2) : null,
                        Refunds = Math.Round(totalRefunds, 2),
                        ActualProfits = actualProfits.HasValue ? (double?)Math.Round((decimal)actualProfits.Value, 2) : null,


                    });
                }

                response = ResponseHandler.Success(courseRevenueList, "Tính doanh thu khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<CourseRevenueViewModel>>($"Lỗi khi tính doanh thu khóa học: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<double>> GetTotalRevenueFromRegisterCourseAsync(DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<double>();

            try
            {

                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(userCurrent.AgencyId.Value);
                var totalMoney = (double)0;
                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<double>(0, "User hoặc Agency không khả dụng!");
                }
                foreach (var registration in registerCourses)
                {

                    var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                    foreach (var payment in payments)
                    {
                        if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                        {
                            totalMoney += payment.Amount ?? 0;
                        }
                    }
                }
                response = ResponseHandler.Success(totalMoney, "Tính tổng doanh thu từ Payment thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<double>($"Lỗi khi tính tổng doanh thu: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<double>> GetTotalRevenueFromRegisterCourseByAgencyIdAsync(Guid AgencyId, DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<double>();

            try
            {

                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(AgencyId);
                var totalMoney = (double)0;
                foreach (var registration in registerCourses)
                {

                    var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                    foreach (var payment in payments)
                    {
                        if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                        {
                            totalMoney += payment.Amount ?? 0;
                        }
                    }
                }
                response = ResponseHandler.Success(totalMoney, "Tính tổng doanh thu từ Payment thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<double>($"Lỗi khi tính tổng doanh thu: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<double>> GetAmountAgencyAmountPayAsync(Guid AgencyId, DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<double>();

            try
            {
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(AgencyId);
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(AgencyId);
                var totalMoney = (double)0;
                foreach (var registration in registerCourses)
                {

                    var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                    foreach (var payment in payments)
                    {
                        if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                        {
                            totalMoney += payment.Amount ?? 0;
                        }
                    }
                }
                totalMoney = totalMoney * (contract.RevenueSharePercentage.Value / 100);

                response = ResponseHandler.Success(totalMoney, "Tính tổng doanh thu từ Payment thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<double>($"Lỗi khi tính tổng doanh thu: {ex.Message}");
            }

            return response;
        }
        //
        public async Task<ApiResponse<List<AgencyFinancialReport>>> GetAgencyFinancialReportAsync(Guid agencyId, int year)
        {
            var response = new ApiResponse<List<AgencyFinancialReport>>();
            try
            {
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if (agency == null)
                {
                    return ResponseHandler.Failure<List<AgencyFinancialReport>>("Agency not found");
                }

                var reports = new List<AgencyFinancialReport>();

                for (int month = 1; month <= 12; month++)
                {
                    var startDate = new DateTime(year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var revenue = await _unitOfWork.PaymentRepository.GetTotalRevenueForAgencyInPeriod(agencyId, startDate, endDate);
                    var refunds = await _unitOfWork.PaymentRepository.GetTotalRefundsForAgencyInPeriod(agencyId, startDate, endDate);

                    var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                    var revenueSharePercentage = contract?.RevenueSharePercentage ?? 0;

                    var profitsReceived = revenue * (revenueSharePercentage / 100);
                    var actualProfits = profitsReceived - (refunds * (revenueSharePercentage / 100));

                    var report = new AgencyFinancialReport
                    {
                        FiscalPeriod = "Tháng" +" "+ month,
                        Revenue = Math.Round(revenue, 2),
                        ProfitsReceived = Math.Round(profitsReceived, 2),
                        Refunds = Math.Round(refunds, 2),
                        ActualProfits = Math.Round(actualProfits, 2),
                        OffsettingPeriod = "Tháng" +" "+ (month + 1 > 12 ? 1 : month + 1).ToString()
                    };

                    reports.Add(report);
                }

                response.Data = reports;
                response.isSuccess = true;
                response.Message = "Truy xuất thành công";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = $"Error retrieving financial reports: {ex.Message}";
            }

            return response;
        }
        //Xuất file Excel cái trên 
        public async Task<ApiResponse<string>> GetFileExcelAgencyFinancialReportAsync(Guid agencyId, int year)
        {
            try
            {
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if (agency == null)
                {
                    return ResponseHandler.Failure<string>("Agency not found");
                }

                var reports = new List<AgencyFinancialReport>();

                for (int month = 1; month <= 12; month++)
                {
                    var startDate = new DateTime(year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var revenue = await _unitOfWork.PaymentRepository.GetTotalRevenueForAgencyInPeriod(agencyId, startDate, endDate);
                    var refunds = await _unitOfWork.PaymentRepository.GetTotalRefundsForAgencyInPeriod(agencyId, startDate, endDate);

                    var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                    var revenueSharePercentage = contract?.RevenueSharePercentage ?? 0;

                    var profitsReceived = revenue * (revenueSharePercentage / 100);
                    var actualProfits = profitsReceived - (refunds * (revenueSharePercentage / 100));

                    var report = new AgencyFinancialReport
                    {
                        FiscalPeriod = $"Tháng {month}",
                        Revenue = Math.Round(revenue, 2),
                        ProfitsReceived = Math.Round(profitsReceived, 2),
                        Refunds = Math.Round(refunds, 2),
                        ActualProfits = Math.Round(actualProfits, 2),
                        OffsettingPeriod = $"Tháng {(month + 1 > 12 ? 1 : month + 1)}"
                    };

                    reports.Add(report);
                }

                // Generate Excel file
                var excelData = GenerateExcelFile(reports, agency.Name, year);

                // Create file name
                string fileName = $"Báo cáo doanh thu _{agency.Name}_{year}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                // Upload file to Firebase and get URL
                string firebaseUrl = await _firebaseService.UploadFileAsync(new MemoryStream(excelData), fileName);

                return ResponseHandler.Success(firebaseUrl, "Thành công.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"Error generating financial report: {ex.Message}");
            }
        }

        private byte[] GenerateExcelFile(List<AgencyFinancialReport> data, string agencyName, int year)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"{agencyName} Financial Report {year}");

                // Thêm tiêu đề báo cáo
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1"].Value = $"Báo Cáo Tài Chính Cho {agencyName} - Năm {year}";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.White);

                // Thêm tiêu đề cột
                worksheet.Cells["A2"].Value = "Kỳ Tài Chính";
                worksheet.Cells["B2"].Value = "Doanh Thu (VND)";
                worksheet.Cells["C2"].Value = "Lợi Nhuận Nhận Được(VND)";
                worksheet.Cells["D2"].Value = "Hoàn Tiền(VND)";
                worksheet.Cells["E2"].Value = "Lợi Nhuận Thực Tế(VND)" +
                    "";
                worksheet.Cells["F2"].Value = "Kỳ Bù Trừ";

                // Định dạng tiêu đề cột
                worksheet.Cells["A2:F2"].Style.Font.Bold = true;
                worksheet.Cells["A2:F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A2:F2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:F2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:F2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:F2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // Thêm dữ liệu
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = data[i].FiscalPeriod;
                    worksheet.Cells[i + 3, 2].Value = data[i].Revenue;
                    worksheet.Cells[i + 3, 3].Value = data[i].ProfitsReceived;
                    worksheet.Cells[i + 3, 4].Value = data[i].Refunds;
                    worksheet.Cells[i + 3, 5].Value = data[i].ActualProfits;
                    worksheet.Cells[i + 3, 6].Value = data[i].OffsettingPeriod;

                    // Căn lề giữa cho từng hàng
                    for (int col = 1; col <= 6; col++)
                    {
                        worksheet.Cells[i + 3, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 3, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[i + 3, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[i + 3, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[i + 3, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[i + 3, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    // Định dạng số liệu: Kiểu tiền tệ cho cột "Doanh Thu", "Lợi Nhuận Nhận Được", "Lợi Nhuận Thực Tế", "Hoàn Tiền"
                    worksheet.Cells[i + 3, 2].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[i + 3, 3].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[i + 3, 5].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[i + 3, 4].Style.Numberformat.Format = "#,##0";
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Đặt độ rộng tối thiểu và tối đa cho các cột
                for (int col = 1; col <= 6; col++)
                {
                    worksheet.Column(col).Width = Math.Max(worksheet.Column(col).Width, 15); // Độ rộng tối thiểu
                    worksheet.Column(col).Width = Math.Min(worksheet.Column(col).Width, 30); // Độ rộng tối đa
                }

                return package.GetAsByteArray();
            }
        }


        public async Task<ApiResponse<string>> GetFileExcelAgencyMonthlyFinancialReportAsync(Guid agencyId, int month, int year)
        {
            try
            {
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if (agency == null)
                {
                    return ResponseHandler.Failure<string>("Agency not found");
                }

                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var revenue = await _unitOfWork.PaymentRepository.GetTotalRevenueForAgencyInPeriod(agencyId, startDate, endDate);
                var refunds = await _unitOfWork.PaymentRepository.GetTotalRefundsForAgencyInPeriod(agencyId, startDate, endDate);
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                var revenueSharePercentage = contract?.RevenueSharePercentage ?? 0;

                var profitsReceived = revenue * (revenueSharePercentage / 100);
                var actualProfits = profitsReceived - (refunds * (revenueSharePercentage / 100));

                var nextMonth = month == 12 ? 1 : month + 1;
                var nextYear = month == 12 ? year + 1 : year;

                var report = new AgencyFinancialReport
                {
                    FiscalPeriod = $"Tháng {month}/{year}",
                    Revenue = Math.Round(revenue, 2),
                    ProfitsReceived = Math.Round(profitsReceived, 2),
                    Refunds = Math.Round(refunds, 2),
                    ActualProfits = Math.Round(actualProfits, 2),
                    OffsettingPeriod = $"Tháng {nextMonth}/{nextYear}"
                };

                // Generate Excel file
                var excelData = GenerateMonthlyExcelFile(report, agency.Name, month, year);

                // Create file name
                string fileName = $"Báo cáo doanh thu_{agency.Name}_{month}_{year}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                // Upload file to Firebase and get URL
                string firebaseUrl = await _firebaseService.UploadFileAsync(new MemoryStream(excelData), fileName);

                return ResponseHandler.Success(firebaseUrl, "Thành công.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"Error generating monthly financial report: {ex.Message}");
            }
        }

        private byte[] GenerateMonthlyExcelFile(AgencyFinancialReport data, string agencyName, int month, int year)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"{agencyName} Financial Report {month}/{year}");

                // Add report title
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1"].Value = $"Báo Cáo Tài Chính Cho {agencyName} - Tháng {month}/{year}";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.White);

                // Add column headers
                worksheet.Cells["A2"].Value = "Kỳ Tài Chính";
                worksheet.Cells["B2"].Value = "Doanh Thu (VND)";
                worksheet.Cells["C2"].Value = "Lợi Nhuận Nhận Được (VND)";
                worksheet.Cells["D2"].Value = "Hoàn Tiền (VND)";
                worksheet.Cells["E2"].Value = "Lợi Nhuận Thực Tế (VND)";
                worksheet.Cells["F2"].Value = "Kỳ Bù Trừ";

                // Format column headers
                worksheet.Cells["A2:F2"].Style.Font.Bold = true;
                worksheet.Cells["A2:F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A2:F2"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:F2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:F2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:F2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // Add data
                worksheet.Cells["A3"].Value = data.FiscalPeriod;
                worksheet.Cells["B3"].Value = data.Revenue;
                worksheet.Cells["C3"].Value = data.ProfitsReceived;
                worksheet.Cells["D3"].Value = data.Refunds;
                worksheet.Cells["E3"].Value = data.ActualProfits;
                worksheet.Cells["F3"].Value = data.OffsettingPeriod;

                // Format data cells
                worksheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B3:E3"].Style.Numberformat.Format = "#,##0.00";

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}
