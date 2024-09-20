using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class ConsultationRepository: IConsultationRepository
    {
        private readonly AppDbContext _context;

        public ConsultationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FranchiseRegistrationRequests franchiseRequest)
        {
            await _context.FranchiseRegistrationRequests.AddAsync(franchiseRequest);
        }
       /* public async Task<FranchiseRegistrationRequests> GetByIdAsync(Guid id)
        {
            return await _context.FranchiseRegistrationRequests.FindAsync(id);
        }*/
      
        public async Task<FranchiseRegistrationRequests> GetByIdAsync(Guid id)
        {
            return await _context.FranchiseRegistrationRequests
                .Include(x => x.User) 
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<FranchiseRegistrationRequests>> GetFilteredRequestsAsync(ConsultationStatusEnum? status)
        {
            if (status.HasValue)
            {
                return await _context.FranchiseRegistrationRequests
                    .Where(x => x.Status == status.Value)
                    .ToListAsync();
            }
            else
            {
                return await _context.FranchiseRegistrationRequests
                    .ToListAsync();
            }
        }
        public async Task Update(FranchiseRegistrationRequests entity)
        {
            
            _context.FranchiseRegistrationRequests.Update(entity);
        }
    }
}
