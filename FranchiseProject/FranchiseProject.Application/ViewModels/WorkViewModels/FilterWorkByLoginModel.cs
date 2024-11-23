using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.WorkViewModels
{
    public class FilterWorkByLoginModel
    {
        public string? Search { get; set; }
        public WorkLevelEnum? Level { get; set; }
        public WorkStatusEnum? Status { get; set; }
        public WorkStatusSubmitEnum? Submit { get; set; }
        public bool? IsReported { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
