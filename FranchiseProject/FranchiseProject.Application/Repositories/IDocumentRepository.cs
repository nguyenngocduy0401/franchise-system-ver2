using FranchiseProject.Domain.Entity;
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
    }
}
