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
    public class RoleInitializer
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleInitializer(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task RoleInitializeAsync()
        {
            string[] roleNames = { AppRole.Admin, AppRole.Student, AppRole.Intructor, AppRole.Manager, AppRole.AgencyManager };


            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }
    }
}

