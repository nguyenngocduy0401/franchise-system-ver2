using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentStatusEnum
    {
        pending=0  ,        // Chờ  
        Waitlisted = 1,     //Chờ để join vào lớp
        Enrolled = 2,       // Đã vào lớp 
    }
}
