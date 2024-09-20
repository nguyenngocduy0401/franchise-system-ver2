﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IConsultationRepository
    {
        Task AddAsync(FranchiseRegistrationRequests franchiseRequest);
        Task<FranchiseRegistrationRequests> GetByIdAsync(Guid id);
        Task<List<FranchiseRegistrationRequests>> GetFilteredRequestsAsync(ConsultationStatusEnum? status);

        Task  Update(FranchiseRegistrationRequests entity);
    }
}