using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PackageViewModels
{
    public class FilterPackageModel
    {
        public string? Search { get; set; }
        public PackageStatusEnum? Status { get; set; }
        public SortPackageStatusEnum? SortBy { get; set; }
        public SortDirectionEnum? SortDirection { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
