using FranchiseProject.Domain.Entity;
using FranchiseProject.Infrastructures.FluentAPIs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        #region Dbset
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmit> AssignmentSubmits { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CoursesCategories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackAnswer> FeedbackAnswers { get; set; }
        public DbSet<FeedbackOption> FeedbackOptions { get; set; }
        public DbSet<FeedbackQuestion> FeedbackQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<Quiz> Quizs { get; set; }
        public DbSet<QuizDetail> QuizDetails { get; set; }
        public DbSet<Report> Reports { get; set; }
        /*public DbSet<Role> Roles { get; set; }*/
        public DbSet<Score> Scores { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Syllabus> Syllabuses { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<FranchiseRegistrationRequests> FranchiseRegistrationRequests { get; set; }
        /*public DbSet<User> Users { get; set; }*/
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeedbackAnswerConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuizDetailConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentAnswerConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentClassConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentCourseConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssignmentSubmitConfiguration).Assembly);
            modelBuilder.Entity<Contract>().HasData(
                     new Contract
                     {
                         Id = Guid.Parse("550EE872-EA09-42A0-B9AC-809890DEBAFB"),
                         EndTime = DateTime.Now.AddDays(5)
                     }
                     );
        }
    }
}
