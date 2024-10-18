using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.SessionViewModels
{
    public class SessionViewModel
    {
        public Guid? Id { get; set; }
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? Chapter { get; set; }
        public string? Description { get; set; }
        public Guid? CourseId { get; set; }
    }
}
