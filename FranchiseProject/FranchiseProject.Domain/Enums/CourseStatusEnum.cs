using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Enums
{
    public enum CourseStatusEnum
    {
        Draft,// Nháp
        PendingApproval,// Chờ phê duyệt
        AvailableForFranchise,// Sẵn sàng cho nhượng quyền
        TemporarilySuspended,// Tạm đóng
        Closed,// Đóng
    }
}
