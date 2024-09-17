using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Commons
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool isSuccess { get; set; } = true;
        public string? Message { get; set; }
    }
}
