using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.DocumentViewModel;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
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
    }
}
