using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModel
{
    public class StudentViewModel
    {
        public string? AgencyId { get; set; }
        public string? StudentName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public List<string>? Course { get; set; }
    }
}
