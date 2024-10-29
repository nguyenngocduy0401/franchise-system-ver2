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
        Studied=1,                   // Đã Học
        Enrolled = 2 ,                // Đang Học
        DroppedOut=3 ,               //Thôi học
        Pending=4 ,                  //Chưa Tư Vấn 
        NotConsult=5 ,               //Đã Tư vấn 
        Cancel=6 ,                   //Cancel Khóa học    
    }
}
