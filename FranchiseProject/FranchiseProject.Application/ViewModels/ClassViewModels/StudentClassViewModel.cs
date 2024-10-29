using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModel
{
    public class StudentClassViewModel
    {
        public string? UserName { get; set; }
        public string? UserId {  get; set; }
        public string? StudentName {  get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? URLImage { get; set; }
    }
}
