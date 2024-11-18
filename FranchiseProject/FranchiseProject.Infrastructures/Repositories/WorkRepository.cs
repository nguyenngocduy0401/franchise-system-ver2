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
        public IEnumerable<Work> GetAllPreWorkByAgencyId(Guid agencyId) 
        {
            return _dbContext.Works
                .Where(e => e.AgencyId == agencyId && e.IsDeleted != true &&
                (e.Type == WorkTypeEnum.Interview || e.Type == WorkTypeEnum.AgreementSigned ||
                 e.Type == WorkTypeEnum.BusinessRegistered || e.Type == WorkTypeEnum.SiteSurvey ||
                 e.Type == WorkTypeEnum.Design || e.Type == WorkTypeEnum.Quotation ||
                 e.Type == WorkTypeEnum.SignedContract || e.Type == WorkTypeEnum.ConstructionAndTrainning || 
                 e.Type == WorkTypeEnum.Handover || e.Type == WorkTypeEnum.EducationalSupervision));
        }

        public IEnumerable<Work> GetWorksByUserId(string userId)
        {
            return  _dbContext.UserAppointments
                    .Where(ua => ua.UserId == userId)
                    .Select(ua => ua.Appointment)
                    .Where(a => a.IsDeleted != true)
                    .Select(a => a.Work)
                    .Where(a => a.IsDeleted != true)
                    .Distinct();
        }
    }
}

