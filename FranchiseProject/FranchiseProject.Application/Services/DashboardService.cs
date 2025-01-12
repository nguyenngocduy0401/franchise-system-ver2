using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.DashBoard;
using FranchiseProject.Application.ViewModels.DashBoardViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseRepository _firebaseService;

        public DashboardService(IUnitOfWork unitOfWork    ,    IFirebaseRepository firebaseService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
        }
        public async Task<ApiResponse<AdminDashboardModel>> AdminCourseRevenueDashboardAsync(DateOnly from, DateOnly to, bool year, int topCourse)
        {
            var response = new ApiResponse<AdminDashboardModel>();
            try
            {
                var fromDate = from.ToDateTime(new TimeOnly(0, 0));  // Chuyển từ DateOnly sang DateTime
                var toDate = to.ToDateTime(new TimeOnly(23, 59));
                // Lọc dữ liệu thanh toán đã hoàn thành và loại là MonthlyDue, đồng thời lọc theo ngày từ "from" đến "to"
                var queryCourse = _unitOfWork.PaymentRepository.GetTableAsTracking()
                    .AsNoTracking()
                    .Where(x => x.Status == PaymentStatus.Completed && x.Type == PaymentTypeEnum.MonthlyDue
                    && x.PaidDate >= fromDate && x.PaidDate <= toDate);  // So sánh DateOnly với DateOnly
                var adminDashboardModel = new AdminDashboardModel();
                List<AdminCourseRevenueDashboard> revenueStatistics;

                // Nếu year == true, nhóm theo năm
                if (year)
                {
                    revenueStatistics = await queryCourse
                        .GroupBy(x => x.PaidDate.Value.Year)
                        .Select(g => new AdminCourseRevenueDashboard
                        {
                            Year = g.Key,
                            Month = 0,  // Không cần tháng, chỉ cần năm
                            Revenue = (double)g.Sum(x => x.Amount)  // Tổng doanh thu theo năm
                        })
                        .ToListAsync();
                    adminDashboardModel.totalCourseRevenue = revenueStatistics.Sum(r => r.Revenue);
                    adminDashboardModel.adminCourseRevenueDashboards = revenueStatistics;
                }
                else
                {

                    revenueStatistics = await queryCourse
                        .GroupBy(x => new { x.PaidDate.Value.Year, x.PaidDate.Value.Month })
                        .Select(g => new AdminCourseRevenueDashboard
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                    adminDashboardModel.totalCourseRevenue = revenueStatistics.Sum(r => r.Revenue);
                    adminDashboardModel.adminCourseRevenueDashboards = revenueStatistics;
                }

                var queryContract = _unitOfWork.PaymentRepository.GetTableAsTracking()
                    .AsNoTracking()
                    .Where(x => x.Status == PaymentStatus.Completed && x.Type == PaymentTypeEnum.Contract
                    && x.PaidDate >= fromDate && x.PaidDate <= toDate);
                List<AdminContractRevenueDashboard> revenueContractStatistics;


                if (year)
                {
                    revenueContractStatistics = await queryContract
                        .GroupBy(x => x.PaidDate.Value.Year)
                        .Select(g => new AdminContractRevenueDashboard
                        {
                            Year = g.Key,
                            Month = 0,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                    adminDashboardModel.totalContractRevenue = revenueContractStatistics.Sum(r => r.Revenue);
                    adminDashboardModel.adminContractRevenueDashboard = revenueContractStatistics;
                }
                else
                {
                    revenueContractStatistics = await queryContract
                        .GroupBy(x => new { x.PaidDate.Value.Year, x.PaidDate.Value.Month })
                        .Select(g => new AdminContractRevenueDashboard
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                    adminDashboardModel.totalContractRevenue = revenueContractStatistics.Sum(r => r.Revenue);
                    adminDashboardModel.adminContractRevenueDashboard = revenueContractStatistics;
                }

                var topCourses = await _unitOfWork.RegisterCourseRepository.GetTableAsTracking()
                .AsNoTracking()
                .Where(rc => !rc.Course.IsDeleted && rc.CreationDate >= fromDate && rc.CreationDate <= toDate) // Loại bỏ các khóa học đã bị xóa
                .GroupBy(rc => new { rc.Course.Code })
                .Select(g => new CourseDashboardModel
                {
                    Code = g.Key.Code,
                    Name = g.OrderByDescending(x => x.Course.Version).FirstOrDefault().Course.Name, // Lấy tên từ phiên bản mới nhất,
                    NumberOfRegisters = g.Count() // Đếm số lượng đăng ký
                })
                .OrderByDescending(c => c.NumberOfRegisters) // Sắp xếp giảm dần theo số lượng đăng ký
                .Take(topCourse) // Lấy top 5 khóa học
                .ToListAsync();
                adminDashboardModel.courseDashboardModels = topCourses;
                return ResponseHandler.Success(adminDashboardModel, "Revenue statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<AdminDashboardModel>($"Error retrieving revenue statistics: {ex.Message}");
            }
        }

        /*public async Task<ApiResponse<AdminDashboardModel>> AdminCourseRevenueDashboardAsync(DateOnly from, DateOnly to, bool year, int topCourse)
        {
            var response = new ApiResponse<AdminDashboardModel>();
            try
            {
                var fromDate = from.ToDateTime(new TimeOnly(0, 0));
                var toDate = to.ToDateTime(new TimeOnly(23, 59));

                var adminDashboardModel = new AdminDashboardModel();

                // Query for Course payments (Type == PaymentTypeEnum.Course)
                var queryCourse = _unitOfWork.PaymentRepository.GetTableAsTracking()
                    .AsNoTracking()
                    .Where(x => x.Status == PaymentStatus.Completed && x.Type == PaymentTypeEnum.Course)
                    .Where(x => x.PaidDate >= fromDate && x.PaidDate <= toDate);

                List<AdminCourseRevenueDashboard> courseRevenueStatistics;

                if (year)
                {
                    courseRevenueStatistics = await queryCourse
                        .GroupBy(x => x.PaidDate.Value.Year)
                        .Select(g => new AdminCourseRevenueDashboard
                        {
                            Year = g.Key,
                            Month = 0,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                }
                else
                {
                    courseRevenueStatistics = await queryCourse
                        .GroupBy(x => new { x.PaidDate.Value.Year, x.PaidDate.Value.Month })
                        .Select(g => new AdminCourseRevenueDashboard
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                }

                adminDashboardModel.adminCourseRevenueDashboards = courseRevenueStatistics;
                adminDashboardModel.totalCourseRevenue = courseRevenueStatistics.Sum(r => r.Revenue);

                // Query for MonthlyDue payments (Type == PaymentTypeEnum.MonthlyDue)
                var queryMonthlyDue = _unitOfWork.PaymentRepository.GetTableAsTracking()
                    .AsNoTracking()
                    .Where(x => x.Status == PaymentStatus.Completed && x.Type == PaymentTypeEnum.MonthlyDue)
                    .Where(x => x.PaidDate >= fromDate && x.PaidDate <= toDate);

                List<AdminContractRevenueDashboard> monthlyDueRevenueStatistics;

                if (year)
                {
                    monthlyDueRevenueStatistics = await queryMonthlyDue
                        .GroupBy(x => x.PaidDate.Value.Year)
                        .Select(g => new AdminContractRevenueDashboard
                        {
                            Year = g.Key,
                            Month = 0,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                }
                else
                {
                    monthlyDueRevenueStatistics = await queryMonthlyDue
                        .GroupBy(x => new { x.PaidDate.Value.Year, x.PaidDate.Value.Month })
                        .Select(g => new AdminContractRevenueDashboard
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Revenue = (double)g.Sum(x => x.Amount)
                        })
                        .ToListAsync();
                }

                adminDashboardModel.adminContractRevenueDashboard = monthlyDueRevenueStatistics;
                adminDashboardModel.totalContractRevenue = monthlyDueRevenueStatistics.Sum(r => r.Revenue);

                // Query for top courses
                var topCourses = await _unitOfWork.RegisterCourseRepository.GetTableAsTracking()
                    .AsNoTracking()
                    .Where(rc => !rc.Course.IsDeleted && rc.CreationDate >= fromDate && rc.CreationDate <= toDate)
                    .GroupBy(rc => new { rc.Course.Code })
                    .Select(g => new CourseDashboardModel
                    {
                        Code = g.Key.Code,
                        Name = g.OrderByDescending(x => x.Course.Version).FirstOrDefault().Course.Name,
                        NumberOfRegisters = g.Count()
                    })
                    .OrderByDescending(c => c.NumberOfRegisters)
                    .Take(topCourse)
                    .ToListAsync();

                adminDashboardModel.courseDashboardModels = topCourses;

                return ResponseHandler.Success(adminDashboardModel, "Revenue statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<AdminDashboardModel>($"Error retrieving revenue statistics: {ex.Message}");
            }
        }*/
        public async Task<ApiResponse<RevenueStatisticsViewModel>> GetRevenueStatistics()
        {
            try
            {
                var contracts = await _unitOfWork.ContractRepository.GetAllAsync();
                if (contracts == null)
                {
                    return ResponseHandler.Success<RevenueStatisticsViewModel>(null, "không có dữ liệu truy xuất !");
                }
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync();

                double? totalRevenue = 0;
                double? collectedRevenue = 0;
                double? unpaidRevenue = 0;

                foreach (var contract in contracts)
                {
                    totalRevenue += contract.Total;
                    collectedRevenue += contract.PaidAmount;
                }

                unpaidRevenue = totalRevenue - collectedRevenue;

                var revenueStatistics = new RevenueStatisticsViewModel
                {
                    TotalRevenue = (double)totalRevenue,
                    CollectedRevenue = (double)collectedRevenue,
                    UnpaidRevenue = (double)unpaidRevenue
                };

                return ResponseHandler.Success(revenueStatistics, "Revenue statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<RevenueStatisticsViewModel>($"Error retrieving revenue statistics: {ex.Message}");
            }
        }
        public async Task<ApiResponse<Pagination<AgencyStatisticsViewModel>>> GetAgencyStatistics(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            try
            {
                var agencies = await _unitOfWork.AgencyRepository.GetAllAsync();
                var agencyStatistics = new List<AgencyStatisticsViewModel>();

                foreach (var agency in agencies)
                {
                    var latestContract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agency.Id);

                    if (latestContract == null)
                    {
                        continue;
                    }

                    var registerCourses = await _unitOfWork.RegisterCourseRepository.GetRegisterCoursesByAgencyIdAndDateRange(agency.Id, startDate, endDate);
                    var payments = await _unitOfWork.PaymentRepository.GetPaymentsByAgencyIdAndDateRange(agency.Id, startDate, endDate);

                    int totalStudents = registerCourses.Count;
                    double totalRevenue = payments.Where(p => p.Type == PaymentTypeEnum.Course).Sum(p => p.Amount ?? 0);
                    double revenueToHeadquarters = (double)(totalRevenue * (latestContract.RevenueSharePercentage / 100));

                    var monthlyPayments = payments.Where(p => p.Type == PaymentTypeEnum.MonthlyDue).ToList();
                    bool allMonthlyPaymentsPaid = monthlyPayments.All(p => p.Status == PaymentStatus.Completed);

                    agencyStatistics.Add(new AgencyStatisticsViewModel
                    {
                        AgencyId = agency.Id,
                        AgencyName = agency.Name,
                        TotalStudents = totalStudents,
                        TotalRevenue = totalRevenue,
                        RevenueToHeadquarters = revenueToHeadquarters,
                        MonthlyPaymentStatus = allMonthlyPaymentsPaid ? "Paid" : "Pending"
                    });
                }
                var paginatedResult = new Pagination<AgencyStatisticsViewModel>
                {
                    TotalItemsCount = agencyStatistics.Count,
                    PageSize = pageSize,
                    PageIndex = pageNumber,
                    Items = agencyStatistics.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                };

                return ResponseHandler.Success(paginatedResult, "Agency statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<Pagination<AgencyStatisticsViewModel>>($"Error retrieving agency statistics: {ex.Message}");
            }
        }
        public async Task<ApiResponse<List<MonthlyRevenueViewModel>>> AnalyzeMonthlyRevenueAsync(int year)
        {
            var response = new ApiResponse<List<MonthlyRevenueViewModel>>();
            try
            {
                var revenueData = await _unitOfWork.RevenueRepository.GetRevenueByMonthAsync(year);

                var monthlyRevenue = revenueData
                    .GroupBy(r => new { r.Date.Year, r.Date.Month })
                    .Select(g => new MonthlyRevenueViewModel
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalRevenue = g.Sum(r => r.Amount)
                    })
                    .ToList();

                response = ResponseHandler.Success(monthlyRevenue, "Phân tích doanh thu theo tháng thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<MonthlyRevenueViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<PartnerRevenueShare>>> CalculateRevenueSharePercentageAsync()
        {
            var response = new ApiResponse<List<PartnerRevenueShare>>();
            try
            {
                var partnerRevenues = await _unitOfWork.RevenueRepository.GetMonthlyRevenueShareAsync();
                var totalRevenue = partnerRevenues.Sum(pr => pr.TotalAmount);

                foreach (var partnerRevenue in partnerRevenues)
                {
                    partnerRevenue.RevenuePercentage = totalRevenue > 0 ? (partnerRevenue.TotalAmount / totalRevenue) * 100 : 0;
                }

                response = ResponseHandler.Success(partnerRevenues, "Calculated revenue share percentages successfully.");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<PartnerRevenueShare>>($"Error calculating revenue share percentages: {ex.Message}");
            }
            return response;
        }
        public async Task<ApiResponse<string>> GenerateAgencyPaymentReportAsync(int month, int year)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var agencies = await _unitOfWork.AgencyRepository.GetAllAsync();
                var reportData = new List<AgencyPaymentReportViewModel>();

                int rowNumber = 1;
                foreach (var agency in agencies)
                {
                    var payments = await _unitOfWork.PaymentRepository.GetPaymentsByAgencyIdAndDateRange(agency.Id, startDate, endDate);

                    var monthlyRevenue = payments.Sum(p => p.Amount ?? 0);

                    var activeContract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agency.Id);

                    if (activeContract == null)
                    {
                        continue;
                    }

                    var franchiseFee = activeContract.FrachiseFee ?? 0;
                    var revenuePercentage = activeContract.RevenueSharePercentage ?? 0;
                    var sharedAmount = monthlyRevenue * (revenuePercentage / 100);

                    reportData.Add(new AgencyPaymentReportViewModel
                    {
                        RowNumber = rowNumber++,
                        AgencyName = agency.Name,
                        MonthYear = $"{month}/{year}",
                        FranchiseFee = (decimal)franchiseFee,
                        MonthlyRevenue = (decimal)monthlyRevenue,
                        RevenuePercentage = (decimal)revenuePercentage,
                        SharedAmount = (decimal)sharedAmount
                    });
                }

                var excelData = GenerateExcelFile(reportData);

                // Tạo tên file
                string fileName = $"AgencyPaymentReport_{month}_{year}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                // Upload file lên Firebase và lấy URL
                string firebaseUrl = await _firebaseService.UploadFileAsync(new MemoryStream(excelData), fileName);

                return ResponseHandler.Success(firebaseUrl, "Báo cáo đã được tạo và tải lên thành công.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"Lỗi khi tạo báo cáo: {ex.Message}");
            }
        }
        private byte[] GenerateExcelFile(List<AgencyPaymentReportViewModel> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo cáo thanh toán đại lý");
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Tên đối tác";
                worksheet.Cells[1, 3].Value = "Tháng/Năm";
                worksheet.Cells[1, 4].Value = "Phí nhượng quyền";
                worksheet.Cells[1, 5].Value = "Doanh thu tháng";
                worksheet.Cells[1, 6].Value = "Phần trăm doanh thu";
                worksheet.Cells[1, 7].Value = "Số tiền chia sẻ";
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].RowNumber;
                    worksheet.Cells[i + 2, 2].Value = data[i].AgencyName;
                    worksheet.Cells[i + 2, 3].Value = data[i].MonthYear;
                    worksheet.Cells[i + 2, 4].Value = data[i].FranchiseFee;
                    worksheet.Cells[i + 2, 5].Value = data[i].MonthlyRevenue;
                    worksheet.Cells[i + 2, 6].Value = data[i].RevenuePercentage;
                    worksheet.Cells[i + 2, 7].Value = data[i].SharedAmount;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}

