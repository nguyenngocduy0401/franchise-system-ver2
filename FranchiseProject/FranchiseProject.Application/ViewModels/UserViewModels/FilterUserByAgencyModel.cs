using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserViewModels
{
    public class FilterUserByAgencyModel
    {
        public string? Search { get; set; }
        public IsActiveEnum? IsActive { get; set; }
        public AgencyRolesEnum? Role { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
