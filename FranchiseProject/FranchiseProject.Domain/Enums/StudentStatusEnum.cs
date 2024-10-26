using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentStatusEnum
    {
        NotConsult=0  ,        // chua tu van 
        Pending=1,             //Đã tư vấn 
        Waitlisted = 2,     //Chờ để join vào lớp
        Enrolled = 3,       // Đã vào lớp 
        Cancel = 4          //Cancel
    }
}
