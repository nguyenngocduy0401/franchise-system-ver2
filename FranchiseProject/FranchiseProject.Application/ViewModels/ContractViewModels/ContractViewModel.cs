using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public  class ContractViewModel
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Total { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public double? RevenueSharePercentage { get; set; }
        public string? AgencyName { get; set; }
    }
}
