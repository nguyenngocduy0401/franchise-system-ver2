using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using FranchiseProject.Infrastructures.FluentAPIs;
using iText.StyledXmlParser.Jsoup.Parser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures
{
    public class AppDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
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
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<Quiz> Quizs { get; set; }
        public DbSet<QuizDetail> QuizDetails { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Syllabus> Syllabuses { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<WorkDetail> WorkDetails { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentDetail> AppointmentDetails { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<ChapterMaterial> ChapterMaterials { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceConfiguration).Assembly);
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
            modelBuilder.Entity<Agency>().HasData(
                     new Agency
                     {
                         Id = Guid.Parse("BE37023D-6A58-4B4B-92E5-39DCECE45473"),
                         Status = AgencyStatusEnum.Partner
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
                    Name="JAVA_TEST_SU25"
                },
                new Class
                {
                    Id= Guid.Parse("99129374-30F6-4F57-978F-583353684CA5"),
                    Capacity= 30,
                    CurrentEnrollment= 1,
                    Name="OOP_TEST_SU25"
                },
                new Class
                {
                    Id=Guid.Parse("A2A94DDC-FF9E-484C-8D2A-6F9D5DD21279"),
                    Capacity= 30,
                    Name="MLN131_TEST_SU25",
                    CurrentEnrollment=1
                }
                );
            #endregion
            #region CourseCategory
            modelBuilder.Entity<CourseCategory>().HasData(
                new CourseCategory
                {
                    Id = Guid.Parse("264c1d37-40f3-4dd9-793e-08dcf20adbbc"),
                    Name = "Thuật toán",
                    Description = "Các khóa học liên quan đến thuật toán, bao gồm các khái niệm cơ bản, các loại thuật toán, và ứng dụng của chúng trong lập trình và khoa học máy tính."
                },
                new CourseCategory
                {
                    Id = Guid.Parse("f1390dbc-82f7-4cd4-793f-08dcf20adbbc"),
                    Name = "Kiến thức cơ sở",
                    Description = "Các khóa học cung cấp kiến thức nền tảng về khoa học máy tính, bao gồm các khái niệm cơ bản, ngôn ngữ lập trình cơ bản, và các nguyên lý thiết kế hệ thống."
                },
                new CourseCategory
                {
                    Id = Guid.Parse("f8fd80dd-c470-4ecf-7940-08dcf20adbbc"),
                    Name = "Lập trình cơ sở",
                    Description = "Các khóa học tập trung vào các kỹ năng lập trình cơ bản, như lập trình hướng đối tượng, cấu trúc dữ liệu, và thuật toán cơ bản, giúp học viên xây dựng nền tảng vững chắc trong lập trình."
                },
                new CourseCategory
                {
                    Id = Guid.Parse("228efc7b-2659-4186-7941-08dcf20adbbc"),
                    Name = "Lập trình nâng cao",
                    Description = "Các khóa học chuyên sâu về lập trình, bao gồm các kỹ thuật lập trình phức tạp, thiết kế hệ thống, lập trình đa luồng, và tối ưu hóa hiệu suất ứng dụng."
                }
            );
            #endregion
            #region Course
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014"),
                    Name = "Nhập môn lập trình với C",
                    Description = "Khóa học lập trình căn bản với ngữ C giành cho người mới bắt đầu học lập trình",
                    URLImage = "string", 
                    NumberOfLession = 20,
                    Price = 2000000,
                    Code = "PRF",
                    Version = 0,
                    Status = CourseStatusEnum.Draft,
                    CourseCategoryId = Guid.Parse("f8fd80dd-c470-4ecf-7940-08dcf20adbbc")
                }
            );
            #endregion
        }
    }
}
