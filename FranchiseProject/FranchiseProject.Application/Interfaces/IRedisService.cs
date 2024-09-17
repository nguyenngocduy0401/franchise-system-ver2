using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IRedisService
    {
        Task<bool> StoreJwtTokenAsync(string userName, string jwtToken);
        Task<bool> CheckUserExistAsync(string userName);
        Task RemoveUserIfExistsAsync(string userName);
        Task<bool> CheckJwtTokenExistsAsync(string userName);
    }
}
