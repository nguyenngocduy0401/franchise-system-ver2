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
        Active,                      // Đang hoạt động
        Suspended,                   // Tạm ngưng hoạt động
        Inactive                     // Không hoạt động
    }

}
