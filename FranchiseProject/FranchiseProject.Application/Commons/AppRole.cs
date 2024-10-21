using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Commons
{
    public static class AppRole
    {
        public const string Admin = "Administrator";
        public const string Manager = "Manager";
        public const string AgencyManager = "AgencyManager";
        public const string Student = "Student";
        public const string Instructor = "Instructor";
        public const string SystemInstructor = "SystemInstructor"; //giáo viên của trung tâm đào tạo
        public const string SystemConsultant = "Consultant"; // Tư vấn viên
        public const string SystemTechnician = "Technician";// Thiết kế, khảo sảt
        public const string AgencyStaff = "AgencyStaff";// tư vấn cho học viên
    }
}
