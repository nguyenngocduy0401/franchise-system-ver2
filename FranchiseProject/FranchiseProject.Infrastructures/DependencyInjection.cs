using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string appConfiguration)
        {
            #region Service DI
            #endregion
            #region Repository DI
            #endregion
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(appConfiguration));
            return services;
        }
    }
}
