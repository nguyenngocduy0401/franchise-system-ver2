using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum AgencyStatusEnum
    {
        Pending = 1,          // Chờ
        Processing = 2,       // Đang xử lý
        Approved = 3,         // Xét duyệt thành công
                               // 
        Partner = 4,          // Đối tác
        Expired = 5           // Hết hạn

    }
}
