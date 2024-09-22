using FranchiseProject.Application.Commons;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserViewModels
{
    public class FilterUserByAdminModel
    {
        public string? Search { get; set; }
        public IsActiveEnum? IsActive { get; set; }
        public RolesEnum? Role {get; set; }
        public Guid? AgencyId { get; set; }
        public Guid? ContractId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
