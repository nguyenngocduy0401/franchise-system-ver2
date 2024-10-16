using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentPaymentStatusEnum
    {
        Completed=1,
        Pending_Payment=3,
        Dropped =2,//học sanh bỏ lớp 
        Refunded=3// học sinh hủy lớp trước khi lớp bắt đầu 

    }
}
