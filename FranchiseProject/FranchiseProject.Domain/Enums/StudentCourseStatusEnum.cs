using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentCourseStatusEnum
    {
        NotStudied=0,           // Chưa Học--
        Studied=1,             // Đã Học
        CurrentlyStudying=2 , // Đang Học
        DroppedOut=3 ,    //Thôi học
        Pending=4 ,
    }
}
