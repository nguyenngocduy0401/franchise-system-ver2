using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.VideoViewModels
{
    public class VideoViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
    }
}
