using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.DocumentViewModel;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<ApiResponse<bool>> UpdaloadDocumentAsyc(UploadDocumentViewModel document);
        Task<ApiResponse<DocumentViewModel>> GetDocumentByIdAsync(Guid documentId);
        Task<ApiResponse<Pagination<DocumentViewModel>>> FilterDocumentAsync(FilterDocumentViewModel filterModel);
        Task<ApiResponse<bool>> DeleteDocumentAsync(Guid documentId);
        Task<ApiResponse<bool>> UpdateDocumentAsync(Guid documentId, UpdateDocumentViewModel updateDocumentModel);
        Task NotifyCustomersOfExpiringDocuments();
        Task<ApiResponse<DocumentViewModel>> GetDocumentbyAgencyId(Guid agencyId, DocumentType type);
        Task<ApiResponse<List<DocumentViewModel>>> GetAllDocumentsByAgencyIdAsync(Guid agencyId);
        Task<ApiResponse<bool>> UpdateDocument(Guid agencyId, DocumentType type);
    }
}
