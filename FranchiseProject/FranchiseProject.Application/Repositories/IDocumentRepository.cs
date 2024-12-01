using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IDocumentRepository :IGenericRepository<Document>
    {
        Task<bool> HasActiveAgreementContractAsync(Guid agencyId);
        Task<bool> HasActiveBusinessLicenseAsync(Guid agencyId);
        Task<Document> GetMostRecentAgreeSignByAgencyIdAsync(Guid agencyId, DocumentType type);
    }
}
