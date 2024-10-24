﻿using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class RegisterCourseRepository : IRegisterCourseRepository
    {
        private readonly AppDbContext _dbContext;
        public RegisterCourseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public  Task<List<string>> GetCourseNamesByUserIdAsync(string userId)
        {
            return  _dbContext.RegisterCourses
            .Where(rc => rc.UserId == userId && rc.StudentCourseStatus == StudentCourseStatusEnum.NotStudied) 
            .Include(rc => rc.Course) 
            .Select(rc => rc.Course.Name) 
            .ToListAsync();
            }
        public async Task AddAsync(RegisterCourse registerCourse)
        {
            await _dbContext.Set<RegisterCourse>().AddAsync(registerCourse);
        }
    }
}
