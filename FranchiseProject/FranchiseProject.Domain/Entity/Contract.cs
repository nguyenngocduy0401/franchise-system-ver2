﻿using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Contract : BaseEntity
    {
        public string? Title { get; set; }
        public string? ContractCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? DepositPercentage { get; set; }
        public double? Total { get; set; }
        public double? PaidAmount { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public double? RevenueSharePercentage { get; set; }
        public double? FrachiseFee { get; set; }
        public ContractStatusEnum? Status { get; set; }
        public Guid? PackageId { get; set; }
        [ForeignKey("PackageId")]
        public Package? Package { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
    }
}
