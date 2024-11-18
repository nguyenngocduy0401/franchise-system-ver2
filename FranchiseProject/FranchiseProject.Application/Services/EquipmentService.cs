using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
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
        public async Task<ApiResponse<bool>> CreateEquipmentAsync(List<EquipmentRequestViewModel> models)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (models == null || !models.Any())
                {
                    return ResponseHandler.Failure<bool>("Danh sách thiết bị không được để trống.");
                }

                var equipmentsToAdd = new List<Equipment>();
                var equipmentsToUpdate = new List<Equipment>();

                foreach (var model in models)
                {
                    var contract = await _unitOfWork.ContractRepository.GetByIdAsync(model.ContractId);
                    if (contract == null)
                    {
                        return ResponseHandler.Failure<bool>($"Không tìm thấy hợp đồng !");
                    }

                    var equipmentTypePrice = await _unitOfWork.EquipmentTypePriceRepository.FindAsync(e => e.Type == model.Type);
                    if (equipmentTypePrice == null || !equipmentTypePrice.Any())
                    {
                        return ResponseHandler.Failure<bool>($"Không tìm thấy giá cho loại thiết bị: {model.Type}");
                    }
                    var price = equipmentTypePrice.First().Price;

                    var existingEquipment = await _unitOfWork.EquipmentRepository.FindAsync(e =>
                        e.ContractId == model.ContractId && e.Type == model.Type);

                    if (existingEquipment.Any())
                    {
                        var equipment = existingEquipment.First();
                        equipment.Quantity += model.Quantity;
                        equipment.Note = model.Note;
                        equipmentsToUpdate.Add(equipment);

                    }
                    else
                    {
                        var newEquipment = new Equipment
                        {
                            Id = Guid.NewGuid(),
                            Type = model.Type,
                            ContractId = model.ContractId,
                            CreationDate = DateTime.UtcNow,
                            Status = EquipmentStatusEnum.Available,
                            Price = price,
                            Note=model.Note,
                            Quantity = model.Quantity
                        };
                        equipmentsToAdd.Add(newEquipment);
                    }
                }

                if (equipmentsToAdd.Any())
                    await _unitOfWork.EquipmentRepository.AddRangeAsync(equipmentsToAdd);

                if (equipmentsToUpdate.Any())
                    _unitOfWork.EquipmentRepository.UpdateRange(equipmentsToUpdate);

                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Tạo/Cập nhật thiết bị thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể tạo/cập nhật thiết bị. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi tạo/cập nhật thiết bị: {ex.Message}");
            }
        }
        public async Task<ApiResponse<byte[]>> ExportEquipmentsToExcelAsync(Guid contractId)
        {
            try
            {
                var equipments = await _unitOfWork.EquipmentRepository.FindAsync(e => e.ContractId == contractId);
                if (!equipments.Any())
                {
                    return ResponseHandler.Failure<byte[]>("Không tìm thấy thiết bị cho hợp đồng này.");
                }

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Thiết bị");

                    worksheet.Cells["A1"].Value = "STT";
                    worksheet.Cells["B1"].Value = "Loại thiết bị";
                    worksheet.Cells["C1"].Value = "Số lượng";
                    worksheet.Cells["D1"].Value = "Giá";
                    worksheet.Cells["E1"].Value = "Tổng giá";

                    var headerCells = worksheet.Cells["A1:E1"];
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    int row = 2;
                    decimal totalPrice = 0;

                    foreach (var equipment in equipments)
                    {
                        worksheet.Cells[row, 1].Value = row - 1;
                        worksheet.Cells[row, 2].Value = TranslateEquipmentType(equipment.Type.ToString());
                        worksheet.Cells[row, 3].Value = equipment.Quantity;
                        worksheet.Cells[row, 4].Value = equipment.Price;
                        worksheet.Cells[row, 5].Formula = $"C{row}*D{row}";

                        totalPrice += (decimal)(equipment.Quantity ?? 0) * (decimal)(equipment.Price ?? 0);
                        row++;
                    }

                    worksheet.Cells[row + 1, 4].Value = "Tổng giá trị:";
                    worksheet.Cells[row + 1, 5].Value = totalPrice;
                    worksheet.Cells[row + 1, 5].Style.Font.Bold = true;

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    return ResponseHandler.Success(package.GetAsByteArray(), "Xuất Excel thành công.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<byte[]>($"Lỗi khi xuất Excel: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> AddEquipmentAfterContractSigningAsync(Guid contractId, List<EquipmentRequestViewModel> equipmentRequests)
        {
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    return ResponseHandler.Failure<bool>("Không tìm thấy hợp đồng.");
                }

                if (contract.AgencyId == null)
                {
                    return ResponseHandler.Failure<bool>("Hợp đồng này chưa được liên kết với Agency.");
                }

                var equipmentsToAdd = new List<Equipment>();

                foreach (var request in equipmentRequests)
                {
                    var equipmentTypePrice = await _unitOfWork.EquipmentTypePriceRepository.FindAsync(e => e.Type == request.Type);
                    if (!equipmentTypePrice.Any())
                    {
                        return ResponseHandler.Failure<bool>($"Không tìm thấy giá cho loại thiết bị: {request.Type}");
                    }

                    var price = equipmentTypePrice.First().Price;

                    var newEquipment = new Equipment
                    {
                        Id = Guid.NewGuid(),
                        Type = request.Type,
                        ContractId = contractId,
                        CreationDate = DateTime.UtcNow,
                        Status = EquipmentStatusEnum.Available,
                        Price = price,
                        Quantity = request.Quantity
                    };

                    equipmentsToAdd.Add(newEquipment);
                }

                await _unitOfWork.EquipmentRepository.AddRangeAsync(equipmentsToAdd);
                var result = await _unitOfWork.SaveChangeAsync() > 0;

                if (result)
                {
                    return ResponseHandler.Success(true, "Thêm thiết bị cho Agency thành công.");
                }
                else
                {
                    return ResponseHandler.Failure<bool>("Không thể thêm thiết bị. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi khi thêm thiết bị: {ex.Message}");
            }
        }
        public async Task<ApiResponse<Pagination<EquipmentViewModel>>> GetEquipmentsByAgencyIdAsync(Guid agencyId, int pageIndex, int pageSize)
        {
            var response = new ApiResponse<Pagination<EquipmentViewModel>>();

            try
            {
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if (agency == null)
                {
                    return ResponseHandler.Failure<Pagination<EquipmentViewModel>>("Không tìm thấy Agency.");
                }

                var contracts = await _unitOfWork.ContractRepository.FindAsync(c => c.AgencyId == agencyId);
                if (!contracts.Any())
                {
                    return ResponseHandler.Success<Pagination<EquipmentViewModel>>(null, "Agency này chưa có hợp đồng nào.");
                }

                var contractIds = contracts.Select(c => c.Id).ToList();

                var query = _unitOfWork.EquipmentRepository.GetTableAsTracking()
                    .Where(e => contractIds.Contains(e.ContractId.Value));

                var totalCount = await query.CountAsync();

                var equipments = await query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var equipmentViewModels = equipments.Select(e => new EquipmentViewModel
                {
                    Id = e.Id,
                    Type = e.Type,
                    Status = e.Status,
                    Quantity = e.Quantity,
                    Note = e.Note,
                    Price = e.Price,
                  //  ContractId = e.ContractId
                }).ToList();

                var equipmentPagination = new Pagination<EquipmentViewModel>
                {
                    Items = equipmentViewModels,
                    TotalItemsCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                response = ResponseHandler.Success(equipmentPagination, "Lấy danh sách thiết bị thành công.");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<EquipmentViewModel>>($"Lỗi khi lấy danh sách thiết bị: {ex.Message}");
            }

            return response;
        }

        private string TranslateEquipmentType(string type)
        {
          
            switch (type)
            {
                case "Table":
                    return "Bàn";
                case "Chair":
                    return "Ghế";

                default:
                    return type;
            }
        }
    }
}


