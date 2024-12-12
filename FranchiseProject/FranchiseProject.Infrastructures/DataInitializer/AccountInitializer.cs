/*using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
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
        private readonly ICurrentTime _currentTime;
        public AccountInitializer(UserManager<User> userManager, RoleManager<Role> roleManager,
            ICurrentTime currentTime)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _currentTime = currentTime;
        }

        public async Task AccountInitializeAsync()
        {
            string[] roles = { 
                AppRole.Admin,
                AppRole.Student,
                AppRole.Instructor, 
                AppRole.Manager,
                AppRole.AgencyManager, 
                AppRole.AgencyStaff, 
                AppRole.SystemInstructor, 
                AppRole.SystemConsultant, 
                AppRole.SystemTechnician };

            foreach (var role in roles)
            {
                var userRole = await _userManager.FindByNameAsync(role);

                if (userRole == null)
                {
                    var newUser = await _userManager.CreateAsync(new User { UserName = role, FullName = role, CreateAt = _currentTime.GetCurrentTime() }, "abc123");

                    if (newUser.Succeeded)
                    {
                        var getUser = await _userManager.FindByNameAsync(role);
                        await _userManager.AddToRoleAsync(getUser, role);
                    }
                }
                if(role != AppRole.Student && role != AppRole.Instructor && role != AppRole.AgencyManager && role != AppRole.AgencyStaff){
                    for (int i = 1; i <= 10; i++)
                    {
                        userRole = await _userManager.FindByNameAsync(role + $"{i}");
                        if (userRole == null)
                        {
                            var newUser = await _userManager.CreateAsync(new User { UserName = role + $"{i}", FullName = role + $"{i}", CreateAt = _currentTime.GetCurrentTime().AddSeconds(i) }, "abc123");

                            if (newUser.Succeeded)
                            {
                                var getUser = await _userManager.FindByNameAsync(role + $"{i}");
                                await _userManager.AddToRoleAsync(getUser, role);
                            }
                        }
                    }
                }
            }
        }
    }
}
*/using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
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
        private readonly ICurrentTime _currentTime;
        public AccountInitializer(UserManager<User> userManager, RoleManager<Role> roleManager,
            ICurrentTime currentTime)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _currentTime = currentTime;
        }

        public async Task AccountInitializeAsync()
        {
            string[] roles = {
        AppRole.Admin,
        AppRole.Student,
        AppRole.Instructor,
        AppRole.Manager,
        AppRole.AgencyManager,
        AppRole.AgencyStaff,
        AppRole.SystemInstructor,
        AppRole.SystemConsultant,
        AppRole.SystemTechnician };

            foreach (var role in roles)
            {
                var userRole = await _userManager.FindByNameAsync(role);

                // Tạo tài khoản chính cho mỗi vai trò
                if (userRole == null)
                {
                    var newUser = await _userManager.CreateAsync(new User
                    {
                        UserName = role,
                        FullName = GenerateFullNameForFirstUserIni(role),
                        CreateAt = _currentTime.GetCurrentTime()
                    }, "abc123");

                    if (newUser.Succeeded)
                    {
                        var getUser = await _userManager.FindByNameAsync(role);
                        await _userManager.AddToRoleAsync(getUser, role);
                    }
                }

                // Tạo 10 tài khoản bổ sung nếu vai trò không thuộc các vai trò cụ thể
                if (role != AppRole.Student && role != AppRole.Instructor && role != AppRole.AgencyManager && role != AppRole.AgencyStaff)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        string userName = role + i;
                        userRole = await _userManager.FindByNameAsync(userName);

                        if (userRole == null)
                        {
                            var fullName = GenerateFullName(i);

                            var newUser = await _userManager.CreateAsync(new User
                            {
                                UserName = userName,
                                FullName = fullName,
                                CreateAt = _currentTime.GetCurrentTime().AddSeconds(i)
                            }, "abc123");

                            if (newUser.Succeeded)
                            {
                                var getUser = await _userManager.FindByNameAsync(userName);
                                await _userManager.AddToRoleAsync(getUser, role);
                            }
                        }
                    }
                }
            }
        }

        // Hàm tạo tên đầy đủ theo mẫu "Nguyễn Văn A", "Nguyễn Văn B",...
        private string GenerateFullName(int index)
        {
            char letter = (char)('A' + (index - 1) % 26); // Sử dụng bảng chữ cái từ A đến Z
            return $"Nguyễn Văn {letter}";
        }

        private string GenerateFullNameForFirstUserIni(string role)
        {

            switch (role)
            {
                case AppRole.Manager:
                    return "Nguyễn Ngọc Duy";
                case AppRole.SystemConsultant:
                    return "Lê Công Tính";
                case AppRole.SystemTechnician:
                    return "Trần Lê Việt Hoàng";
                case AppRole.SystemInstructor:
                    return "Nguyễn Trung Hiếu";
            }
            return role.ToString();
        }
    }
}