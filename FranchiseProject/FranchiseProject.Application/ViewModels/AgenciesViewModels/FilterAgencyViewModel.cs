﻿using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ConsultationViewModels
{
    public class FilterAgencyViewModel
    {
        public AgencyStatusEnum? Status {  get; set; }
        public string? SearchInput {  get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
