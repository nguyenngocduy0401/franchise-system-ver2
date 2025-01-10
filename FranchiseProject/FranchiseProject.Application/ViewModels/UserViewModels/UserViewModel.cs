﻿using FranchiseProject.Application.ViewModels.AgenciesViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? Role { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? URLImage { get; set; }
        public UserStatusEnum? status { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Gender { get; set; }
        public Guid? AgencyId { get; set; }
        public AgencyUserViewModel? Agency {  get; set; }
    }
}
