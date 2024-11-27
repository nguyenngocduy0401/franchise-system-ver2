using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    internal class WorkRepository : GenericRepository<Work>, IWorkRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public WorkRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<bool> CheckUserWorkExist(Guid workId, string userId)
        {
            var userWork = await _dbContext.UserAppointments
                .Where(e => e.UserId == userId)
                .Select(ua => ua.Appointment)
                .Where(a => a.IsDeleted != true)
                .Select(a => a.Work)
                .Where(a => a.IsDeleted != true && a.Id == workId)
                .Distinct().FirstOrDefaultAsync();
            return (userWork == null) ? false : true;
        }
        public async Task<Work> GetWorkDetailById(Guid id) 
        {
            var work = await _dbContext.Works.Where(e => e.Id == id &&
                                                   e.IsDeleted != true)
                                       .Include(e => e.Appointments.Where(e => e.IsDeleted != true).OrderByDescending(e => e.StartTime))
                                       .FirstOrDefaultAsync();
            return work;
        }
        public IEnumerable<Work> GetAllPreWorkByAgencyId(Guid agencyId) 
        {
            return _dbContext.Works.AsNoTracking()
                .Where(e => e.AgencyId == agencyId && e.IsDeleted != true &&
                (e.Type == WorkTypeEnum.Interview || e.Type == WorkTypeEnum.AgreementSigned ||
                 e.Type == WorkTypeEnum.BusinessRegistered || e.Type == WorkTypeEnum.SiteSurvey ||
                 e.Type == WorkTypeEnum.Design || e.Type == WorkTypeEnum.Quotation ||
                 e.Type == WorkTypeEnum.SignedContract || e.Type == WorkTypeEnum.ConstructionAndTrainning || 
                 e.Type == WorkTypeEnum.Handover || e.Type == WorkTypeEnum.EducationLicenseRegistered))
                .OrderByDescending(e => e.StartDate);
        }

        public async Task<Pagination<Work>> FilterWorksByUserId(string userId,
            Expression<Func<Work, bool>>? filter = null,
            Func<IQueryable<Work>, IOrderedQueryable<Work>>? orderBy = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
             var query = _dbContext.UserAppointments
                    .Where(ua => ua.UserId == userId)
                    .Select(ua => ua.Appointment)
                    .Where(a => a.IsDeleted != true)
                    .Select(a => a.Work)
                    .Where(a => a.IsDeleted != true)
                    .Distinct();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            var itemCount = await query.CountAsync();
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderByDescending(e => e.CreationDate);
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var result = new Pagination<Work>()
            {
                PageIndex = pageIndex ?? 0,
                PageSize = pageSize ?? 10,
                TotalItemsCount = itemCount,
                Items = await query.ToListAsync(),
            };
            return result;
        }
        public async Task<Work> GetPreviousWorkByAgencyId(Guid agencyId, Expression<Func<Work, bool>>? filter = null) 
        {
            var work = await _dbContext.Agencies.Where(e => e.Id == agencyId)
                                       .SelectMany(e => e.Works)
                                       .Where(filter)
                                       .OrderByDescending(e => e.EndDate)
                                       .FirstOrDefaultAsync();
            return work;
        }
        
    }
}

