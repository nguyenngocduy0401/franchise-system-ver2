using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IWorkRepository : IGenericRepository<Work>
    {
        IEnumerable<Work> GetAllWorkByAgencyId(Guid agencyId);
    }
}
