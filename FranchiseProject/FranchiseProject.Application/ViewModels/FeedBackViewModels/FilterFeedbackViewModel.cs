using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.FeedBackViewModels
{
    public class FilterFeedbackViewModel
    {
      public string? CourseId { get; set; }
      public int PageIndex { get; set; } = 1;
      public int PageSize { get; set; } = 10;

    }
}
