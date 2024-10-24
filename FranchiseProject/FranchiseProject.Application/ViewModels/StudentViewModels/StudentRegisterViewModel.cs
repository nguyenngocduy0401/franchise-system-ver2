using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModels
{
    public class StudentRegisterViewModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email {  get; set; }
        public string? PhoneNumber {  get; set; }
        public List<string>? CourseName { get; set; }
    }
}
