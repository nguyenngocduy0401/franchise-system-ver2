using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IFranchiseRegistrationRequestRepository
    {
        Task AddAsync(FranchiseRegistrationRequests franchiseRequest);
        Task<FranchiseRegistrationRequests> GetByIdAsync(Guid id);
        Task<List<FranchiseRegistrationRequests>> GetAllAsync(Expression<Func<FranchiseRegistrationRequests, bool>> filter);
    }
}
