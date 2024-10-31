using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.FeedBackViewModels
{
    public class FeedBackViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? UserId { get; set; }
        public string?  UserName { get; set; }
    }
}
