using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum StudentPaymentStatusEnum
    {
        Pending_Payment=0,      //chờ thanh toán
        Advance_Payment=2,      // Đã thanh toán 1 phần tiền  
        Completed = 1,          //hoàn thành thanh toán
        Late_Payment=3,//Qua han thanh toan
        Refund =4,
        RequestRefund =5,

    }
}
