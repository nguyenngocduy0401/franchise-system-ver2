using FranchiseProject.Application.Commons;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.DataInitializer
{
    public class AccountInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public AccountInitializer(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AccountInitializeAsync()
        {
            string[] roles = { AppRole.Admin, AppRole.Student, AppRole.Intructor, AppRole.Manager, AppRole.AgencyManager };

            foreach (var role in roles)
            {
                var userRole = await _userManager.FindByNameAsync(role);

                if (userRole == null)
                {
                    var newUser = await _userManager.CreateAsync(new User { UserName = role}, "abc123");

                    if (newUser.Succeeded)
                    {
                        var getUser = await _userManager.FindByNameAsync(role);
                        await _userManager.AddToRoleAsync(getUser, role);
                    }
                }
            }
        }
    }
}
