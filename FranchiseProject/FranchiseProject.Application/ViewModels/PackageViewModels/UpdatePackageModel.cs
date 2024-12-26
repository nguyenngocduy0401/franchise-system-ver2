using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PackageViewModels
{
    public class UpdatePackageModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? NumberOfUsers { get; set; }
        public PackageStatusEnum Status { get; set; }
    }
}
