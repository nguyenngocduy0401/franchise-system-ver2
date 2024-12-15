using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Quartz.Util;
using System.Drawing;
using System.Linq.Expressions;

namespace FranchiseProject.Application.Services
{
    public class EquipmentService : IEquipmentService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<UploadDocumentViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFirebaseService _firebaseService;
        public EquipmentService( IFirebaseService firebaseService,IValidator<UploadDocumentViewModel> validator, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _roleManager = roleManager;
            _userManager = userManager;
            _validator = validator;
            _firebaseService = firebaseService;
        }
        #endregion



        public async Task<ApiResponse<object>> ImportEquipmentsFromExcelAsync(IFormFile file, Guid agencyId)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = file.OpenReadStream())
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var equipmentsToAdd = new List<Equipment>();
                    var serialNumberHistoriesToAdd = new List<EquipmentSerialNumberHistory>();
                    var errors = new List<string>();
                    var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);

                    if (contract == null)
                    {
                        return ResponseHandler.Success<object>("Không thể thực hiện vì khách hàng chưa kí hợp đồng thỏa thuận!");
                    }
                    var existingEquipments = await _unitOfWork.EquipmentRepository
                        .GetTableAsTracking()
                        .Where(e => e.ContractId == contract.Id)
                        .ToListAsync();

                    _unitOfWork.EquipmentRepository.HardRemoveRange(existingEquipments);
                    var serialNumbers = new HashSet<string>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var equipmentName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        var serialNumber = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        var priceString = worksheet.Cells[row, 4].Value?.ToString().Trim();
                        var note = worksheet.Cells[row, 5].Value?.ToString().Trim();

                        if (!serialNumbers.Add(serialNumber))
                        {
                            errors.Add($"Dòng {row}: Số seri '{serialNumber}' bị trùng lặp trong hợp đồng này.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(equipmentName))
                        {
                            errors.Add($"Dòng {row}: Tên thiết bị là bắt buộc.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(serialNumber))
                        {
                            errors.Add($"Dòng {row}: Số seri là bắt buộc.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(priceString) || !double.TryParse(priceString, out double price))
                        {
                            errors.Add($"Dòng {row}: Định dạng giá không hợp lệ.");
                            continue;
                        }

                        if (price < 0)
                        {
                            errors.Add($"Dòng {row}: Giá không thể là số âm.");
                            continue;
                        }

                        if (equipmentName.Length > 100)
                        {
                            errors.Add($"Dòng {row}: Tên thiết bị vượt quá 100 ký tự.");
                            continue;
                        }

                        if (serialNumber.Length > 50)
                        {
                            errors.Add($"Dòng {row}: Số seri vượt quá 50 ký tự.");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(note) && note.Length > 500)
                        {
                            errors.Add($"Dòng {row}: Ghi chú vượt quá 500 ký tự.");
                            continue;
                        }

                        var equipment = new Equipment
                        {
                            EquipmentName = equipmentName,
                            SerialNumber = serialNumber,
                            Price = double.Parse(priceString),
                            Note = note,
                            Status = EquipmentStatusEnum.Available,
                            ContractId = contract.Id
                        };
                        equipmentsToAdd.Add(equipment);

                        var serialNumberHistory = new EquipmentSerialNumberHistory
                        {
                            Equipment = equipment,
                            SerialNumber = serialNumber,
                            StartDate = DateTime.Now
                        };
                        serialNumberHistoriesToAdd.Add(serialNumberHistory);
                    }

                    if (errors.Any())
                    {
                        return ResponseHandler.Failure<object>("Nhập liệu thất bại. " + string.Join(" ", errors));
                    }

                    await _unitOfWork.EquipmentRepository.AddRangeAsync(equipmentsToAdd);
                    await _unitOfWork.EquipmentSerialNumberHistoryRepository.AddRangeAsync(serialNumberHistoriesToAdd);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                    if (!isSuccess) throw new Exception("Nhập liệu thất bại!");

                    return ResponseHandler.Success<object>(true, $"Đã cập nhật thành công {equipmentsToAdd.Count} thiết bị mới!");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<object>("Đã xảy ra lỗi: " + ex.Message);
            }
        }
        public async Task<ApiResponse<object>> ImportEquipmentsAfterFranchiseFromExcelAsync(IFormFile file, Guid agencyId)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = file.OpenReadStream())
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var equipmentsToAdd = new List<Equipment>();
                    var serialNumberHistoriesToAdd = new List<EquipmentSerialNumberHistory>();
                    var errors = new List<string>();
                    var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);

                    if (contract == null)
                    {
                        return ResponseHandler.Failure<object>("Không tìm thấy hợp đồng hoạt động.");
                    }
                    var serialNumbers = new HashSet<string>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var equipmentName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        var serialNumber = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        var priceString = worksheet.Cells[row, 4].Value?.ToString().Trim();
                        var note = worksheet.Cells[row, 5].Value?.ToString().Trim();

                        if (!serialNumbers.Add(serialNumber))
                        {
                            errors.Add($"Dòng {row}: Số seri '{serialNumber}' bị trùng lặp trong hợp đồng này.");
                            continue;
                        }
                        var serialNumberExists = await _unitOfWork.EquipmentRepository
                            .GetTableAsTracking()
                            .AnyAsync(e => e.ContractId == contract.Id && e.SerialNumber == serialNumber);

                        if (serialNumberExists)
                        {
                            errors.Add($"Dòng {row}: Số seri '{serialNumber}' đã tồn tại trong hợp đồng này.");
                            continue;
                        }
                        if (string.IsNullOrEmpty(equipmentName))
                        {
                            errors.Add($"Dòng {row}: Tên thiết bị là bắt buộc.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(serialNumber))
                        {
                            errors.Add($"Dòng {row}: Số seri là bắt buộc.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(priceString) || !double.TryParse(priceString, out double price))
                        {
                            errors.Add($"Dòng {row}: Định dạng giá không hợp lệ.");
                            continue;
                        }

                        if (price < 0)
                        {
                            errors.Add($"Dòng {row}: Giá không thể là số âm.");
                            continue;
                        }

                        if (equipmentName.Length > 100)
                        {
                            errors.Add($"Dòng {row}: Tên thiết bị vượt quá 100 ký tự.");
                            continue;
                        }

                        if (serialNumber.Length > 50)
                        {
                            errors.Add($"Dòng {row}: Số seri vượt quá 50 ký tự.");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(note) && note.Length > 500)
                        {
                            errors.Add($"Dòng {row}: Ghi chú vượt quá 500 ký tự.");
                            continue;
                        }

                        var equipment = new Equipment
                        {
                            EquipmentName = equipmentName,
                            SerialNumber = serialNumber,
                            Price = double.Parse(priceString),
                            Note = note,
                            Status = EquipmentStatusEnum.Available,
                            ContractId = contract.Id
                        };
                        equipmentsToAdd.Add(equipment);

                        var serialNumberHistory = new EquipmentSerialNumberHistory
                        {
                            Equipment = equipment,
                            SerialNumber = serialNumber,
                            StartDate = DateTime.Now
                        };
                        serialNumberHistoriesToAdd.Add(serialNumberHistory);
                    }

                    if (errors.Any())
                    {
                        return ResponseHandler.Failure<object>("Nhập dữ liệu thất bại. " + string.Join(" ", errors));
                    }

                    await _unitOfWork.EquipmentRepository.AddRangeAsync(equipmentsToAdd);
                    await _unitOfWork.EquipmentSerialNumberHistoryRepository.AddRangeAsync(serialNumberHistoriesToAdd);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                    if (!isSuccess) throw new Exception("Nhập dữ liệu thất bại!");

                    return ResponseHandler.Success<object>(true, $"Đã cập nhật thành công {equipmentsToAdd.Count} thiết bị mới!");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<object>($"Lỗi: {ex.Message}");
            }
        }
        public async Task<ApiResponse<string>> GenerateEquipmentReportAsync(Guid agencyId)
        {
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                if (contract == null)
                {
                    return ResponseHandler.Success<string>("No contract found for this agency.");
                }
                var equipments = await _unitOfWork.EquipmentRepository.GetEquipmentByContractIdAsync(contract.Id);
                if (equipments == null || !equipments.Any())
                {
                    return ResponseHandler.Success<string>("Không có thiết bị nào được tìm thấy cho hợp đồng này.");
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Equipment Report");
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "TÊN THIẾT BỊ";
                    worksheet.Cells[1, 3].Value = "SERINUMBER";
                    worksheet.Cells[1, 4].Value = "GIÁ(VND)";
                    worksheet.Cells[1, 5].Value = "NOTE";

                    using (var range = worksheet.Cells[1, 1, 1, 5])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    int row = 2;
                    double totalPrice = 0;
                    foreach (var equipment in equipments)
                    {
                        worksheet.Cells[row, 1].Value = row - 1;
                        worksheet.Cells[row, 2].Value = equipment.EquipmentName;
                        worksheet.Cells[row, 3].Value = equipment.SerialNumber;
                        worksheet.Cells[row, 4].Value = equipment.Price;
                        worksheet.Cells[row, 5].Value = equipment.Note;

                        totalPrice += equipment.Price ?? 0;
                        row++;
                    }

                    worksheet.Cells[row, 3].Value = "TỔNG TIỀN:";
                    worksheet.Cells[row, 4].Value = totalPrice;
                    using (var range = worksheet.Cells[row, 3, row, 4])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    }

                    worksheet.Cells.AutoFitColumns();
                    string fileName = $"EquipmentReport_{agencyId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    byte[] fileContents = package.GetAsByteArray();
                    using (var ms = new MemoryStream(fileContents))
                    {
                        string firebaseUrl = await _firebaseService.UploadFileAsync(ms, fileName);

                        return ResponseHandler.Success(firebaseUrl, "Equipment report generated and uploaded successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"Failed to generate and upload equipment report: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> UpdateEquipmentStatusAsync( Guid equipmentIds , EquipmentStatusEnum equipmentStatus)
        {
            var response = new ApiResponse<bool>();
            try
            {
              
                var equipment = await _unitOfWork.EquipmentRepository.GetExistByIdAsync(equipmentIds);
                if (equipment == null)
                {
                    response = ResponseHandler.Success(false, "Trang thiết bị không hợp lệ");
                }
                    equipment.Status = equipmentStatus;
                    _unitOfWork.EquipmentRepository.Update(equipment);

                
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Failed to update equipment status.");

                response = ResponseHandler.Success(true, "Equipment status updated successfully from Available to Repair.");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>($"Error updating equipment status: {ex.Message}");
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateEquipmentSeriNumberAsync(Guid contractId, List<UpdateEquipmentRangeViewModel> updateModels)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    return ResponseHandler.Success<bool>(false,"Không tìm thấy hợp đồng !");
                }

                var equipments = await _unitOfWork.EquipmentRepository.GetEquipmentByContractIdAsync(contractId);
                if (equipments == null || !equipments.Any())
            {
                return ResponseHandler.Success<bool>(false,"Không có thiết bị nào được tìm thấy cho hợp đồng này.");
            }
                var now = DateTime.Now;

                foreach (var updateModel in updateModels)
                {
                    var equipment = equipments.FirstOrDefault(e => e.Id == updateModel.EquipmentId);
                    if (equipment == null)
                    {
                        return ResponseHandler.Success<bool>(false,"Không tìm thấy trang thiết bị");
                    }

                    equipment.Status = EquipmentStatusEnum.Available;
                    var currentSerialNumberHistory = await _unitOfWork.EquipmentSerialNumberHistoryRepository
                        .GetTableAsTracking()
                        .Where(h => h.EquipmentId == equipment.Id && h.EndDate == null)
                        .FirstOrDefaultAsync();

                    if (currentSerialNumberHistory != null)
                    {
                        currentSerialNumberHistory.EndDate = now;
                    }
                    var newSerialNumberHistory = new EquipmentSerialNumberHistory
                    {
                        EquipmentId = equipment.Id,
                        SerialNumber = updateModel.SerialNumber,
                        StartDate = now,
                        EndDate = null
                    };

                    await _unitOfWork.EquipmentSerialNumberHistoryRepository.AddAsync(newSerialNumberHistory);
                    equipment.SerialNumber = updateModel.SerialNumber;
                }

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Failed to update equipment status and serial numbers.");

                response = ResponseHandler.Success(true, "Equipment status and serial numbers updated successfully.");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>($"Error updating equipment status and serial numbers: {ex.Message}");
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<EquipmentViewModel>>> GetEquipmentByAgencyIdAsync(FilterEquipmentViewModel filter)
        {
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(filter.AgencyId.Value);
                var equipments = await _unitOfWork.EquipmentRepository.GetEquipmentByContractIdAsync(contract.Id);

                if (equipments == null || !equipments.Any())
                {
                    return ResponseHandler.Success<Pagination<EquipmentViewModel>>(null, "Không có thiết bị nào được tìm thấy cho hợp đồng này.");
                }

                // Chỉ áp dụng bộ lọc Status nếu nó được cung cấp và không rỗng
                if (!string.IsNullOrWhiteSpace(filter.Status.ToString()) && filter.Status!=EquipmentStatusEnum.none)
                {
                    equipments = equipments.Where(e => e.Status == filter.Status).ToList();
                }

                var equipmentViewModels = equipments.Select(e => new EquipmentViewModel
                {
                    Id = e.Id,
                    EquipmentName = e.EquipmentName ?? "",
                    SerialNumber = e.SerialNumber ?? "",
                    Status = e.Status,
                    Note = e.Note ?? "",
                    Price = e.Price,
                    AgencyName = contract.Agency?.Name ?? "Unknown Agency"
                }).ToList();

                var totalItems = equipmentViewModels.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);
                var paginatedEquipments = equipmentViewModels
                    .Skip((filter.PageIndex - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var paginationResult = new Pagination<EquipmentViewModel>
                {
                    Items = paginatedEquipments,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    TotalItemsCount = totalItems,
                };

                return ResponseHandler.Success(paginationResult, "Truy xuất danh sách trang thiết bị thành công");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<Pagination<EquipmentViewModel>>($"Error filtering equipment: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> UpdateEquipmentAsync(Guid equipmentId, UpdateEquipmentViewModel updateModel)
        {
            try
            {
                var equipment = await _unitOfWork.EquipmentRepository.GetByIdAsync(equipmentId);
                if (equipment == null)
                {
                    return ResponseHandler.Success<bool>(false,"Không tìm thấy trang thiết bị!.");
                }

                equipment.EquipmentName = updateModel.EquipmentName;
                equipment.Status = updateModel.Status;
                equipment.Note = updateModel.Note;
                equipment.Price = updateModel.Price;
                if (equipment.SerialNumber != updateModel.SerialNumber)
                {
                    var now = DateTime.UtcNow;
                    var currentSerialNumberHistory = await _unitOfWork.EquipmentSerialNumberHistoryRepository
                        .GetTableAsTracking()
                        .Where(h => h.EquipmentId == equipment.Id && h.EndDate == null)
                        .FirstOrDefaultAsync();

                    if (currentSerialNumberHistory != null)
                    {
                        currentSerialNumberHistory.EndDate = now;
                    }
                    var newSerialNumberHistory = new EquipmentSerialNumberHistory
                    {
                        EquipmentId = equipment.Id,
                        SerialNumber = updateModel.SerialNumber,
                        StartDate = now,
                        EndDate = null
                    };

                    await _unitOfWork.EquipmentSerialNumberHistoryRepository.AddAsync(newSerialNumberHistory);
                    equipment.SerialNumber = updateModel.SerialNumber;
                }

                _unitOfWork.EquipmentRepository.Update(equipment);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    throw new Exception("Failed to update equipment.");
                }

                return ResponseHandler.Success(true, "Equipment updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Error updating equipment: {ex.Message}");
            }
        }
        public async Task<ApiResponse<List<EquipmentSerialNumberHistoryViewModel>>> GetSerialNumberHistoryByEquipmentIdAsync(Guid equipmentId)
        {
            try
            {
                var equipment = await _unitOfWork.EquipmentRepository.GetExistByIdAsync(equipmentId);
                if (equipment == null)
                {
                    return ResponseHandler.Success<List<EquipmentSerialNumberHistoryViewModel>>(null,"không tìm thấy trang thiết bị.");
                }

                var serialNumberHistories = await _unitOfWork.EquipmentSerialNumberHistoryRepository
                    .GetTableAsTracking()
                    .Where(h => h.EquipmentId == equipmentId)
                    .OrderByDescending(h => h.CreationDate)
                    .ToListAsync();

                var serialNumberHistoryViewModels = _mapper.Map<List<EquipmentSerialNumberHistoryViewModel>>(serialNumberHistories);

                return ResponseHandler.Success(serialNumberHistoryViewModels, "Truy xuất thành công.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<List<EquipmentSerialNumberHistoryViewModel>>($"Error retrieving serial number history: {ex.Message}");
            }
        }
    }
}


