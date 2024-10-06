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
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
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
            #region SLot
            modelBuilder.Entity<Slot>().HasData(
                new Slot
                {
                    Id = Guid.Parse("849116FA-DD9C-49A4-A019-7616B7447AE9"),
                    Name="SLot 1",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0)

                },
                new Slot
                {
                    Id = Guid.Parse("A994E524-943D-4022-B258-DE37662055C9"),
                    Name ="SLot 2",
                    StartTime = new TimeSpan(15, 0, 0),
                    EndTime = new TimeSpan (21, 0, 0)
                }
                );
            #endregion
            #region CLass
            modelBuilder.Entity<Class>().HasData(
                new Class
                {
                    Id = Guid.Parse("99E3AF58-64B4-4304-AE6A-2D8782E9CAED"),
                    Capacity = 30,
                    CurrentEnrollment = 1,
                    Name="JAVA_TEST_SU25",
                    TermId = Guid.Parse("1FEA8F3B-4FC2-49E5-B059-23821B9AF45A"),
                },
                new Class
                {
                    Id= Guid.Parse("99129374-30F6-4F57-978F-583353684CA5"),
                    Capacity= 30,
                    CurrentEnrollment= 1,
                    Name="OOP_TEST_SU25",
                    TermId=Guid.Parse("1FEA8F3B-4FC2-49E5-B059-23821B9AF45A")
                },
                new Class
                {
                    Id=Guid.Parse("A2A94DDC-FF9E-484C-8D2A-6F9D5DD21279"),
                    Capacity= 30,
                    Name="MLN131_TEST_SU25",
                    CurrentEnrollment=1,
                    TermId=Guid.Parse("1FEA8F3B-4FC2-49E5-B059-23821B9AF45A")
                }
                );
            #endregion
            #region Term
            modelBuilder.Entity<Term>().HasData(
                new Term
                {
                    Id= Guid.Parse("1FEA8F3B-4FC2-49E5-B059-23821B9AF45A"),
                    Name= "SP25",
                    StartDate=  new DateTime(2025,01,01),
                    EndDate= new DateTime(2025,04,01)
                },
                new Term
                {
                    Id = Guid.Parse("97B016EA-591F-4198-8251-5AB4AE8E88EC"),
                    Name = "SU25",
                    StartDate = new DateTime(2025, 04, 02),
                    EndDate = new DateTime(2025, 08, 01)
                },
                new Term
                {
                    Id=Guid.Parse("C60E3315-6C8B-4855-B1CB-FA92C7E4B593"),
                    Name="FA25",
                    StartDate= new DateTime(2025,08,02),
                    EndDate= new DateTime(2025,12,01),
                });
           
            
            #endregion 
        }
    }
}
