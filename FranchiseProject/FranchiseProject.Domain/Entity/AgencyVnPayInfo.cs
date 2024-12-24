using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class AgencyVnPayInfo : BaseEntity
    {


        public string? TmnCode { get; set; }
        public string? HashSecret { get; set; }

        public Guid AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
    }
}

