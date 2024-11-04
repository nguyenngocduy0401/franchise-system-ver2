using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssignmentViewModels
{
    public class StudentAssScorseNumberViewModel
    {
        public double ScoreNumber { get; set; }
        public string? UserId { get; set; }
        public Guid? AssignmentId { get; set; }

    }
}
