using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModels
{
    public class AddQuizAndAssignmentModel
    {
        public List<Quiz>? Quizzes { get; set; }
        public List<Assignment>? Assignments { get; set;}
    }
}
