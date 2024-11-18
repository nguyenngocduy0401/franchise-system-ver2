using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.DocumentViewModel;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
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

       // [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Xóa tài liệu bằng id {Authorize = Manager, Admin}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteDocumentAsync(Guid id)
        {
            return await _documentService.DeleteDocumentAsync(id);
        }

       // [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Tạo mới tài liệu {Authorize = Manager, Admin}")]
        [HttpPost]
        public async Task<ApiResponse<bool>> CreateDocumentAsync(UploadDocumentViewModel uploadDocumentModel)
        {
            return await _documentService.UpdaloadDocumentAsyc(uploadDocumentModel);
        }

       // [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Cập nhật tài liệu {Authorize = Manager, Admin}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateDocumentAsync(Guid id, UpdateDocumentViewModel updateDocumentModel)
        {
            return await _documentService.UpdateDocumentAsync(id, updateDocumentModel);
        }
      //  [Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Tìm tài liệu bằng id{Authorize = Manager, Admin}")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<DocumentViewModel>> GetDocumentByIdAsync(Guid id)
        {
            return await _documentService.GetDocumentByIdAsync(id);
        }
        //[Authorize(Roles = AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "Tìm kiếm tài liệu{Authorize = Manager, Admin}")]
        [HttpGet()]
        public async Task<ApiResponse<Pagination<DocumentViewModel>>> FilterDocumentAsync([FromQuery] FilterDocumentViewModel filterDocumentModel)
        {
            return await _documentService.FilterDocumentAsync(filterDocumentModel);
        }

     
    }
}