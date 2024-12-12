using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Org.BouncyCastle.Bcpg.OpenPgp;
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
        public string? ContractCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? DepositPercentage { get; set; }
        public double? EquipmentFee { get; set; }
        public double? DesignFee { get; set; }
        public double? FrachiseFee { get; set; }
        public double? Total { get; set; }
        public double? PaidAmount { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public double? RevenueSharePercentage { get; set; }
        public ContractStatusEnum? Status { get; set; }
        public string? AgencyName { get; set; }
    }
}
