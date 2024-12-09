using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserViewModels
{
    public class UserWorkViewModel
    {
        public string? Id { get; set; }
        public string? Role { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public int? WorkCount { get; set; }
    }
}
