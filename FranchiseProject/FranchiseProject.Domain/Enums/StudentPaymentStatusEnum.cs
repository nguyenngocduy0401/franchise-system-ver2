using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentPaymentStatusEnum
    {
  
        Pending_Payment=1,//chờ thanh toán
        Completed = 2,  //hoàn thành thanh toán
        Dropped =3,     //học sanh bỏ lớp 
        Refunded=4      // học sinh hủy lớp trước khi lớp bắt đầu 

    }
}
