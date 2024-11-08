using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum AgencyStatusEnum
    {
        Processing,                  // Chờ duyệt
        Approved,                    // Đã duyệt
        Active,                      // Đang hoạt động
        Suspended,                   // Tạm ngưng hoạt động
        Terminated,                  // Chấm dứt hợp tác
        Inactive                     // Không hoạt động
    }

}
