using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.RegisterCourseViewModels
{
    public class StudentCourseViewModel
    {
        public string? UserId { get; set; }
        public string? courseName { get; set; }
         public StudentCourseStatusEnum? StudentCourseStatus { get; set; }
    }
}
