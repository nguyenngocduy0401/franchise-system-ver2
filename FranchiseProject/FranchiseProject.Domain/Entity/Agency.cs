﻿using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Agency : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? NumberOfRoom {  get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? PositionImageURL { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
     //   public string? DesignFee { get; set; }
        public AgencyStatusEnum Status { get; set; }
        public AgencyActivitiesStatusEnum ActivityStatus { get; set; }
        public virtual ICollection<Contract>? Contracts { get; set; }
        public virtual ICollection<Slot>? Slots { get; set; }
    }
}
