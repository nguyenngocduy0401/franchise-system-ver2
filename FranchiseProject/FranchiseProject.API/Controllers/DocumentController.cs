using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.DocumentViewModel;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/documents")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

      [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Xóa tài liệu bằng id {Authorize = Manager, Admin}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteDocumentAsync(Guid id)
        {
            return await _documentService.DeleteDocumentAsync(id);
        }

       [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin+"," + AppRole.SystemTechnician + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Tạo mới tài liệu {Authorize = Manager, Admin,SystemTechnician}")]
        [HttpPost]
        public async Task<ApiResponse<bool>> CreateDocumentAsync(UploadDocumentViewModel uploadDocumentModel)
        {
            return await _documentService.UpdaloadDocumentAsyc(uploadDocumentModel);
        }

        [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin + "," + AppRole.SystemTechnician + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Cập nhật tài liệu {Authorize = Manager, Admin,SystemTechnician, AgencyManager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateDocumentAsync(Guid id, UpdateDocumentViewModel updateDocumentModel)
        {
            return await _documentService.UpdateDocumentAsync(id, updateDocumentModel);
        }
       [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin + "," + AppRole.SystemTechnician + ","+ AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Tìm tài liệu bằng id{Authorize = Manager, Admin,SystemTechnician}")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<DocumentViewModel>> GetDocumentByIdAsync(Guid id)
        {
            return await _documentService.GetDocumentByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin + "," + AppRole.SystemTechnician)]
        [SwaggerOperation(Summary = "Tìm kiếm tài liệu{Authorize = Manager, Admin,SystemTechnician}")]
        [HttpGet()]
        public async Task<ApiResponse<Pagination<DocumentViewModel>>> FilterDocumentAsync([FromQuery] FilterDocumentViewModel filterDocumentModel)
        {
            return await _documentService.FilterDocumentAsync(filterDocumentModel);
        }
        [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin+"," + AppRole.SystemTechnician + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Truy xuất tài liệu bằng Agencyid{Authorize = Manager, Admin,SystemTechnician}")]
        [HttpGet("agency/{id}")]
        public async Task<ApiResponse<DocumentViewModel>> GetDocumentbyAgencyId(Guid id, DocumentType type)
        { 
            return await _documentService.GetDocumentbyAgencyId(id, type);
        }
        [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin + "," + AppRole.SystemTechnician + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Truy xuất danh sách tài liệu bằng Agencyid{Authorize = Manager, Admin,SystemTechnician}")]
        [HttpGet("agency/{id}/all")]
        public async Task<ApiResponse<List<DocumentViewModel>>> GetAllDocumentsByAgencyIdAsync(Guid id)
        {
            return await _documentService.GetAllDocumentsByAgencyIdAsync(id);
        }
    }
}