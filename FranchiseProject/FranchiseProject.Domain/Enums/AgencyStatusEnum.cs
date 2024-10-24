using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum AgencyStatusEnum
    {
       
        Processing   = 1,       // Chờ duyệt
        Approved = 2,           // Đã duyệt
        Active = 3,             // Đang hoạt động
        Suspended = 4,          // Tạm ngưng hoạt động
        Terminated = 5,         // Chấm dứt hợp tác
        Inactive = 6            // Không hoạt động
    }

}
