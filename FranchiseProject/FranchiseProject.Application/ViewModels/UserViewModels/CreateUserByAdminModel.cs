using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserViewModels
{
    public class CreateUserByAdminModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordConfirm { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? URLImage { get; set; }
        public string? AgencyId { get; set; }
        public string? ContractId { get; set; }
    }
}
