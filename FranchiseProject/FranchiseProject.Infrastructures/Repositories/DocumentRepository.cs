
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class DocumentRepository : GenericRepository<Document>,IDocumentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public DocumentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<bool> HasActiveAgreementContractAsync(Guid agencyId)
        {
            return await _dbContext.Documents
                .AnyAsync(d => d.AgencyId == agencyId &&
                               d.Type == Domain.Enums.DocumentType.AgreementContract &&
                               d.Status == Domain.Enums.DocumentStatus.Active);
        }
        public async Task<bool> HasActiveBusinessLicenseAsync(Guid agencyId)
        {
            return await _dbContext.Documents
                .AnyAsync(d => d.AgencyId == agencyId &&
                               d.Type == Domain.Enums.DocumentType.BusinessLicense &&
                               d.Status == Domain.Enums.DocumentStatus.Active);
        }
        public async Task<Document> GetMostRecentAgreeSignByAgencyIdAsync(Guid agencyId,DocumentType type)
        {
            return await _dbContext.Documents
                .Where(c => c.AgencyId == agencyId && c.Type== type)
                .OrderByDescending(c => c.CreationDate)
                .FirstOrDefaultAsync();
        }
    }
}
