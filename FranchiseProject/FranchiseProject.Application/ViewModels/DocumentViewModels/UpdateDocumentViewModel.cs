

using FranchiseProject.Domain.Enums;

namespace FranchiseProject.Application.ViewModels.DocumentViewModels
{
    public class UpdateDocumentViewModel
    {
        public string? Title { get; set; }
        public string? URLFile { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public DocumentType? Type { get; set; }
      
    }
}
