using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class RedisToken
    {
        public string UserName { get; set; }
        public string JWTToken { get; set; }
    }
}
