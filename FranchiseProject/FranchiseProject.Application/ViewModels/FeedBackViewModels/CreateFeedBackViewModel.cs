using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.FeedBackViewModels
{
    public class CreateFeedBackViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ClassId { get; set; }
       
    }
}
