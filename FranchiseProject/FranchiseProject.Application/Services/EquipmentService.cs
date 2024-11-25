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
using System.Drawing;

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
        public EquipmentService(IValidator<UploadDocumentViewModel> validator, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _roleManager = roleManager;
            _userManager = userManager;
            _validator = validator;
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

                    var existingEquipments = await _unitOfWork.EquipmentRepository
                        .GetTableAsTracking()
                        .ToListAsync();
                    var existingSerialNumbers = existingEquipments.Select(e => e.SerialNumber).ToList();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var equipmentName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        var serialNumber = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        var priceString = worksheet.Cells[row, 4].Value?.ToString().Trim();
                        var note = worksheet.Cells[row, 5].Value?.ToString().Trim();

                        if (string.IsNullOrEmpty(equipmentName))
                        {
                            errors.Add($"Row {row}: Equipment name is required.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(serialNumber))
                        {
                            errors.Add($"Row {row}: Serial number is required.");
                            continue;
                        }

                        if (existingSerialNumbers.Contains(serialNumber))
                        {
                            errors.Add($"Row {row}: Serial number '{serialNumber}' already exists.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(priceString) || !double.TryParse(priceString, out double price))
                        {
                            errors.Add($"Row {row}: Invalid price format.");
                            continue;
                        }

                        if (price < 0)
                        {
                            errors.Add($"Row {row}: Price cannot be negative.");
                            continue;
                        }

                        if (equipmentName.Length > 100)
                        {
                            errors.Add($"Row {row}: Equipment name exceeds 100 characters.");
                            continue;
                        }

                        if (serialNumber.Length > 50)
                        {
                            errors.Add($"Row {row}: Serial number exceeds 50 characters.");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(note) && note.Length > 500)
                        {
                            errors.Add($"Row {row}: Note exceeds 500 characters.");
                            continue;
                        }

                        var existingEquipment = existingEquipments.FirstOrDefault(e =>
                            e.EquipmentName == equipmentName &&
                            e.SerialNumber == serialNumber &&
                            e.Price ==double.Parse( priceString )&&
                            e.Note == note);

                        if (existingEquipment == null)
                        {
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
                                
                            };
                            serialNumberHistoriesToAdd.Add(serialNumberHistory);

                            existingSerialNumbers.Add(serialNumber);
                        }
                    }

                    if (errors.Any())
                    {
                        return ResponseHandler.Failure<object>("Import partially failed. " + string.Join(" ", errors));
                    }

                    if (equipmentsToAdd.Any())
                    {
                        await _unitOfWork.EquipmentRepository.AddRangeAsync(equipmentsToAdd);
                        await _unitOfWork.EquipmentSerialNumberHistoryRepository.AddRangeAsync(serialNumberHistoriesToAdd);
                        var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                        if (!isSuccess) throw new Exception("Import failed!");

                        return ResponseHandler.Success<object>(true, $"Đã thêm thành công {equipmentsToAdd.Count} thiết bị mới!");
                    }
                    else
                    {
                        return ResponseHandler.Success<object>(true, "Không có thiết bị mới để thêm.");
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<object>(ex.Message);
            }
        }

        public async Task<ApiResponse<byte[]>> GenerateEquipmentReportAsync(Guid agencyId)
        {
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                if (contract == null)
                {
                    return ResponseHandler.Failure<byte[]>("No contract found for this agency.");
                }
                var equipments = await _unitOfWork.EquipmentRepository.GetEquipmentByContractIdAsync(contract.Id);
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
                    var content = package.GetAsByteArray();

                    return ResponseHandler.Success(content, "Equipment report generated successfully.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<byte[]>($"Failed to generate equipment report: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> UpdateEquipmentStatusAsync(Guid contractId, List<Guid> equipmentIds)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    return ResponseHandler.Failure<bool>("Contract not found.");
                }
                var equipments = await _unitOfWork.EquipmentRepository.GetEquipmentByContractIdAsync(contractId);

                var equipmentsToUpdate = equipments.Where(e => equipmentIds.Contains(e.Id)).ToList();

                if (equipmentsToUpdate.Count == 0)
                {
                    return ResponseHandler.Failure<bool>("No matching equipment found for the given contract and equipment IDs.");
                }
                
                foreach (var equipment in equipmentsToUpdate)
                {
                    if (equipment.Status != EquipmentStatusEnum.Available)
                    {
                        return ResponseHandler.Failure<bool>($"Equipment {equipment.Id} is not in Available status.");
                    }

                    equipment.Status = EquipmentStatusEnum.Repair;
                    _unitOfWork.EquipmentRepository.Update(equipment);

                }
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
        public async Task<ApiResponse<bool>> UpdateEquipmentStatusAsync(Guid contractId, List<UpdateEquipmentRangeViewModel> updateModels)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    return ResponseHandler.Failure<bool>("Contract not found.");
                }

                var equipments = await _unitOfWork.EquipmentRepository.GetEquipmentByContractIdAsync(contractId);
                var now = DateTime.Now;

                foreach (var updateModel in updateModels)
                {
                    var equipment = equipments.FirstOrDefault(e => e.Id == updateModel.EquipmentId);
                    if (equipment == null)
                    {
                        return ResponseHandler.Failure<bool>($"Equipment with ID {updateModel.EquipmentId} not found.");
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
    }
}


