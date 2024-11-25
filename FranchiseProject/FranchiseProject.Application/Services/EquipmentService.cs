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
     
       
       
        public async Task<ApiResponse<object>> ImportEquipmentsFromExcelAsync(IFormFile file)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = file.OpenReadStream())
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var equipments = new List<Equipment>();
                    var errors = new List<string>();
                    var existingSerialNumbers = await _unitOfWork.EquipmentRepository
                        .GetTableAsTracking()
                        .Select(e => e.SerialNumber)
                        .ToListAsync();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var equipmentName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        var serialNumber = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        var priceString = worksheet.Cells[row, 4].Value?.ToString().Trim();
                        var note = worksheet.Cells[row, 5].Value?.ToString().Trim();

                        if (string.IsNullOrEmpty(equipmentName) || string.IsNullOrEmpty(serialNumber) || string.IsNullOrEmpty(priceString))
                        {
                            errors.Add($"Dòng {row}: Thiếu thông tin bắt buộc");
                            continue;
                        }

                        if (equipmentName.Length > 50 || note?.Length > 50)
                        {
                            errors.Add($"Dòng {row}: Tên thiết bị hoặc ghi chú không được quá 50 ký tự");
                            continue;
                        }

                        if (existingSerialNumbers.Contains(serialNumber))
                        {
                            errors.Add($"Dòng {row}: Serial number '{serialNumber}' đã tồn tại");
                            continue;
                        }

                        if (!double.TryParse(priceString, out double price))
                        {
                            errors.Add($"Dòng {row}: Giá không hợp lệ");
                            continue;
                        }

                        var equipment = new Equipment
                        {
                            EquipmentName = equipmentName,
                            SerialNumber = serialNumber,
                            Price = price,
                            Note = note,
                            Status = EquipmentStatusEnum.Available
                        };

                        equipments.Add(equipment);
                        existingSerialNumbers.Add(serialNumber);
                    }

                    if (errors.Any())
                    {
                        var errorWorksheet = package.Workbook.Worksheets.Add("Errors");
                        errorWorksheet.Cells["A1"].Value = "Lỗi";
                        for (int i = 0; i < errors.Count; i++)
                        {
                            errorWorksheet.Cells[i + 2, 1].Value = errors[i];
                        }

                        var errorFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_error.xlsx";
                        var errorFileStream = new MemoryStream();
                        package.SaveAs(errorFileStream);
                        errorFileStream.Position = 0;

                        return ResponseHandler.Success<object>(
                            new { ErrorFile = errorFileStream, FileName = errorFileName },
                            "Import không thành công. Vui lòng xem file lỗi."
                        );
                    }

                    await _unitOfWork.EquipmentRepository.AddRangeAsync(equipments);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                    if (!isSuccess) throw new Exception("Import failed!");

                    return ResponseHandler.Success<object>(true, "Equipment imported successfully!");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<object>(ex.Message);
            }
        }
        
    }
}


