﻿using AutoMapper;
using ClosedXML;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using FranchiseProject.Application.ViewModels.DocumentViewModel;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace FranchiseProject.Application.Services
{
    public class DocumentService : IDocumentService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IEmailService _emailService;
        private readonly IValidator<UploadDocumentViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public DocumentService(IValidator<UploadDocumentViewModel> validator, IMapper mapper, IUnitOfWork unitOfWork,
            IClaimsService claimsService, UserManager<User> userManager, RoleManager<Role> roleManager,
            IEmailService emailService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _roleManager = roleManager;
            _userManager = userManager;
            _validator = validator;
            _emailService = emailService;
        }
        #endregion
        public async Task<ApiResponse<bool>> UpdaloadDocumentAsyc(UploadDocumentViewModel document)
        {
            var response = new ApiResponse<bool>();
            try
            {

                ValidationResult validationResult = await _validator.ValidateAsync(document);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var doc = _mapper.Map<Document>(document);

                doc.Appoved = document.Approved;

                doc.Type = document.DocumentType;
                await _unitOfWork.DocumentRepository.AddAsync(doc);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Upload failed!");

                response = ResponseHandler.Success(true, "upload tài liệu thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<DocumentViewModel>> GetDocumentByIdAsync(Guid documentId)
        {
            var response = new ApiResponse<DocumentViewModel>();
            try
            {

                var document = await _unitOfWork.DocumentRepository.GetByIdAsync(documentId);
                if (document == null) { return ResponseHandler.Success<DocumentViewModel>(null, "Không tìm thấy tài liệu!"); }

                var documentViewModel = _mapper.Map<DocumentViewModel>(document);
                if (document.AgencyId.HasValue)
                {
                    var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(document.AgencyId.Value);
                    documentViewModel.AgencyName = agency?.Name;
                }
                response = ResponseHandler.Success(documentViewModel, "Lấy thông tin tài liệu thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<DocumentViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<DocumentViewModel>>> FilterDocumentAsync(FilterDocumentViewModel filterModel)
        {
            var response = new ApiResponse<Pagination<DocumentViewModel>>();
            try
            {

                Expression<Func<Document, bool>> filter = d =>
                    (!filterModel.AgencyId.HasValue || d.AgencyId == filterModel.AgencyId) &&
                    (!filterModel.Type.HasValue || d.Type == filterModel.Type) &&
                    (d.Status.HasValue ||d.Status == filterModel.Status) &&
                    (d.Appoved == true);


                var documents = await _unitOfWork.DocumentRepository.GetFilterAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(d => d.CreationDate),
                     includeProperties: "Agency",
                    pageIndex: filterModel.PageIndex,
                    pageSize: filterModel.PageSize
                );

                var documentViewModels = _mapper.Map<Pagination<DocumentViewModel>>(documents);
                foreach (var doc in documentViewModels.Items)
                {
                    if (doc.AgencyId.HasValue)
                    {
                        var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(doc.AgencyId.Value);
                        doc.AgencyName = agency?.Name;
                    }
                }
                if (documentViewModels.Items.IsNullOrEmpty())
                    return ResponseHandler.Success(documentViewModels, "Không tìm thấy tài liệu phù hợp!");

                response = ResponseHandler.Success(documentViewModels, "Lọc tài liệu thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<DocumentViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteDocumentAsync(Guid documentId)
        {
            var response = new ApiResponse<bool>();
            try
            {


                var document = await _unitOfWork.DocumentRepository.GetExistByIdAsync(documentId);
                if (document == null) return ResponseHandler.Success(false, "Tài liệu không khả dụng!");

                _unitOfWork.DocumentRepository.SoftRemove(document);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Xóa thất bại!");

                response = ResponseHandler.Success(true, "Xóa tài liệu thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateDocumentAsync(Guid documentId, UpdateDocumentViewModel updateDocumentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var document = await _unitOfWork.DocumentRepository.GetExistByIdAsync(documentId);
                if (document == null) return ResponseHandler.Success(false, "Tài liệu không tồn tại!");

                if (updateDocumentModel.Title != null)
                    document.Title = updateDocumentModel.Title;

                if (updateDocumentModel.URLFile != null)
                    document.URLFile = updateDocumentModel.URLFile;

                if (updateDocumentModel.ExpirationDate.HasValue)
                    document.ExpirationDate = updateDocumentModel.ExpirationDate.Value;

                document.Type = updateDocumentModel.Type;

                _unitOfWork.DocumentRepository.Update(document);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Cập nhật thất bại!");

                response = ResponseHandler.Success(true, "Cập nhật tài liệu thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task NotifyCustomersOfExpiringDocuments()
        {
            var agencies = await _unitOfWork.AgencyRepository.GetAgencyEduLicenseExpiredAsync();

            foreach (var agency in agencies)
            {
                if (agency == null || string.IsNullOrEmpty(agency.Email))
                {

                    continue;
                }

                var emailMessage = new MessageModel
                {
                    To = agency.Email,
                    Subject = "[futuretech-noreply] Thông Báo Hết Hạn Giấy Phép Giáo Dục",
                    Body = $"<p>Kính gửi {agency.Name},</p>" +
                      $"<p>Chúng tôi xin thông báo rằng giấy phép giáo dục của bạn với Futuretech đã hết hạn.</p>" +
                      $"<p>Để đảm bảo các hoạt động giáo dục không bị gián đoạn, chúng tôi kính mời bạn liên hệ với đội ngũ của chúng tôi để thực hiện các thủ tục gia hạn giấy phép.</p>" +
                      $"<p>Vui lòng sử dụng các thông tin dưới đây để liên hệ:</p>" +
                      $"<ul>" +
                      $"<li><strong>Email hỗ trợ:</strong> support@futuretech.com</li>" +
                      $"<li><strong>Số điện thoại:</strong> 0123-456-789</li>" +
                      $"</ul>" +
                      $"<p>Nếu bạn đã hoàn thành gia hạn giấy phép, vui lòng bỏ qua email này.</p>" +
                      $"<p>Chúng tôi mong muốn tiếp tục đồng hành cùng bạn trên con đường phát triển giáo dục sắp tới.</p>" +
                      $"<p>Trân trọng,</p>" +
                      $"<p>Đội ngũ Futuretech</p>"
                };

                await _emailService.SendEmailAsync(emailMessage);
            }
        }

        public async Task<ApiResponse<DocumentViewModel>> GetDocumentbyAgencyId(Guid agencyId, DocumentType type)
        {
            var response = new ApiResponse<DocumentViewModel>();
            try
            {
                var contract = await _unitOfWork.DocumentRepository.GetMostRecentAgreeSignByAgencyIdAsync(agencyId, type);
                if (contract == null)
                {
                    return ResponseHandler.Success<DocumentViewModel>(null, "Không tìm thấy ");
                }
                var contractViewModel = _mapper.Map<DocumentViewModel>(contract);
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(contract.AgencyId.Value);
                contractViewModel.AgencyName = agency.Name;

                return ResponseHandler.Success(contractViewModel, "Truy xuất thành công");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<DocumentViewModel>($"Lỗi truy xuất : {ex.Message}");
            }
        }
        public async Task<ApiResponse<List<DocumentViewModel>>> GetAllDocumentsByAgencyIdAsync(Guid agencyId)
        {
            try
            {
                var documents = await _unitOfWork.DocumentRepository.GetAllAsync(d => d.AgencyId == agencyId);
                if (documents == null || !documents.Any())
                {
                    return ResponseHandler.Success<List<DocumentViewModel>>(null, "Không tìm thấy tài liệu cho Agency này");
                }

                var documentViewModels = _mapper.Map<List<DocumentViewModel>>(documents);
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                string agencyName = agency?.Name ?? "Unknown";
                documentViewModels.ForEach(d => d.AgencyName = agencyName);

                return ResponseHandler.Success(documentViewModels, "Truy xuất thành công");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<List<DocumentViewModel>>($"Lỗi truy xuất : {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateDocument(Guid id)
        {
            try
            {
                var documents = await _unitOfWork.DocumentRepository.GetExistByIdAsync(id);

                if (documents == null)
                {
                    return ResponseHandler.Success<bool>(false, "Không tìm thấy giấy tờ liên quan !");
                }
                documents.Appoved = true;
                documents.Status = DocumentStatus.Active;
                 _unitOfWork.DocumentRepository.Update(documents);
               await _unitOfWork.SaveChangeAsync();

                return ResponseHandler.Success(true, "Duyệt tài liệu thành công");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Lỗi truy xuất : {ex.Message}");
            }
        }
    }
}

