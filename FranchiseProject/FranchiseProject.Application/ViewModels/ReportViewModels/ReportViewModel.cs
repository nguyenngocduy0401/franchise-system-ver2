using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ReportViewModels
{

    public class ReportViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public ReportStatusEnum Status { get; set; }
        public Guid? AgencyId { get; set; }
        public string? AgencyName { get; set; }

        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime CreationDate { get; set; }
        public List<EquipmentViewModel>? Equipments { get; set; }
        public string? RespondedById { get; set; }
    }

}
