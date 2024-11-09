using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentCourseStatusEnum
    {
        Waitlisted = 0,                // Chưa Học
        Enrolled = 1 ,                // Đang Học
        Pending=2,                  //Đã Tư vấn 
        NotConsult =3,               //Chưa Tư Vấn 
        Cancel =4 ,                   //Cancel Khóa học    
    }
}
