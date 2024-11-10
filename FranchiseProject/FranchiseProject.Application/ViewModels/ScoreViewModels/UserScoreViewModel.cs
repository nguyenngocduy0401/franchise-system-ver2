using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ScoreViewModels
{
    public class UserScoreViewModel
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public double? Score { get; set; }
        public string? SubmitFileName { get; set; }
        public string? SubmitUrl { get; set; }

    }
}
