using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AttendanceViewModels
{
	public class ClassScheduleDetailViewModel
	{
		public string Id { get; set; }
		public string CourseCode { get; set; }
		public string ClassName { get; set; }
		public string? Date { get; set; }
		public TimeSpan? StartTime { get; set; }
		public TimeSpan? EndTime { get; set; }
		public int? NumberOfStudent { get; set; }
		public List<StudentClassScheduleViewModel>? StudentInfo { get; set; }

	}
}
