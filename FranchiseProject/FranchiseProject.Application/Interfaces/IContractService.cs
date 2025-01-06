﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IContractService
    {
        Task NotifyCustomersOfExpiringContracts();
        Task<ApiResponse<bool>> UploadContractAsync(CreateContractViewModel create);
        Task<ApiResponse<bool>> UpdateContractAsync(UpdateContractViewModel update, string id);
        Task<ApiResponse<Pagination<ContractViewModel>>> FilterContractViewModelAsync(FilterContractViewModel filter);
        Task<ApiResponse<ContractViewModel>> GetContractByIdAsync(string id);
        Task<ApiResponse<string>> DownloadContractAsPdfAsync(Guid agencyId);
        Task<ApiResponse<ContractViewModel>> GetContractbyAgencyId(Guid agencyId);
        Task<ApiResponse<bool>> AgencyUploadContractAsync(string ContractDocumentImageURL, Guid AgencyId);
        Task<ApiResponse<string>> UploadContractTemplateAsync(IFormFile file);
    }
}
