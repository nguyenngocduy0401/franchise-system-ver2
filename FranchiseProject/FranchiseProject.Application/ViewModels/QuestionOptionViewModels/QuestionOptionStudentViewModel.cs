using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuestionOptionViewModels
{
    public class QuestionOptionStudentViewModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public Guid? QuestionId { get; set; }
    }
}
