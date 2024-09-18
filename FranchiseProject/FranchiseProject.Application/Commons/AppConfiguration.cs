using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Commons
{
    public class AppConfiguration
    {
        public string DatabaseConnection { get; set; }
        public JwtOptions JwtOptions { get; set; }
        public string RedisConfiguration { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
    }
}
