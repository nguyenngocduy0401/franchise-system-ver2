using FranchiseProject.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FranchiseProject.Domain.Entity
{
    public class Document : BaseEntity
    {
        public string? Title { get; set; }
        public string? URLFile { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public DocumentType? Type { get; set; }
        public DocumentStatus Status{ get; set;}
        public bool Appoved { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }

    }
}
