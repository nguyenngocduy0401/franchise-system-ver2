using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuestionOptionViewModels
{
    public class QuestionOptionViewModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public bool Status { get; set; }
        public Guid? QuestionId { get; set; }
    }
}
