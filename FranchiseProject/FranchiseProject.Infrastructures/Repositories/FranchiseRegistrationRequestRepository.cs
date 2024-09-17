﻿using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class FranchiseRegistrationRequestRepository: IFranchiseRegistrationRequestRepository
    {
        private readonly AppDbContext _context;

        public FranchiseRegistrationRequestRepository(AppDbContext context)
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
        public async Task<List<FranchiseRegistrationRequests>> GetAllAsync(Expression<Func<FranchiseRegistrationRequests, bool>> filter)
        {
            return await _context.FranchiseRegistrationRequests.Where(filter).ToListAsync();
        }
        public async Task<FranchiseRegistrationRequests> GetByIdAsync(Guid id)
        {
            return await _context.FranchiseRegistrationRequests
                .Include(x => x.User) 
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
