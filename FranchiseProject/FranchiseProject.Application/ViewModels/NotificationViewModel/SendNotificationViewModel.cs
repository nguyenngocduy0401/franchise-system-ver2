using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.NotificationViewModel
{
    public class SendNotificationViewModel
    {
        public List<string>? userIds { get; set; }
        public string? message { get; set; }
    }
}
