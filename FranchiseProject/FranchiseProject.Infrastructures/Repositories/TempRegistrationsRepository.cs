using DocumentFormat.OpenXml.InkML;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class TempRegistrationsRepository :GenericRepository<TempRegistrations> ,ITempRegistrationsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public TempRegistrationsRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<TempRegistrations?> GetByUserIdAndCourseIdAsync(string userId, Guid courseId)
        {
            if (string.IsNullOrEmpty(userId) || courseId == Guid.Empty)
            {
                return null;
            }
            if (_dbContext.TempRegistrations == null)
            {
                throw new Exception("TempRegistrations set is null.");
            }
            try
            {
                var result = _dbContext.TempRegistrations
     .Where(tr => (userId == null || tr.UserId == userId) &&
                  (courseId == null || tr.CourseId == courseId))
     .FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetByUserIdAndCourseIdAsync: {ex.Message}");
                throw; // Re-throw the exception after logging
            }
        }
    }
}
