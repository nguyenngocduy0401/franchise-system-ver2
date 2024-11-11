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
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizDetail> QuizDetails { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<RegisterForm> RegisterForms { get; set; }
        public DbSet<Syllabus> Syllabuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserAppointment> UserAppointments { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RegisterCourse> RegisterCourses { get; set; }
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
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegisterCourseConfiguration).Assembly);
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
                         Status = AgencyStatusEnum.Approved
                     }
                     );
            #region SLot
            modelBuilder.Entity<Slot>().HasData(
                new Slot
                {
                    Id = Guid.Parse("849116FA-DD9C-49A4-A019-7616B7447AE9"),
                    Name = "SLot 1",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0)

                },
                new Slot
                {
                    Id = Guid.Parse("A994E524-943D-4022-B258-DE37662055C9"),
                    Name = "SLot 2",
                    StartTime = new TimeSpan(15, 0, 0),
                    EndTime = new TimeSpan(21, 0, 0)
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
                    Name = "JAVA_TEST_SU25"
                },
                new Class
                {
                    Id = Guid.Parse("99129374-30F6-4F57-978F-583353684CA5"),
                    Capacity = 30,
                    CurrentEnrollment = 1,
                    Name = "OOP_TEST_SU25"
                },
                new Class
                {
                    Id = Guid.Parse("A2A94DDC-FF9E-484C-8D2A-6F9D5DD21279"),
                    Capacity = 30,
                    Name = "MLN131_TEST_SU25",
                    CurrentEnrollment = 1
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
            #region Syllabus
            modelBuilder.Entity<Syllabus>().HasData(
                new Syllabus
                {
                    Id = Guid.Parse("990ca87d-c261-4cb3-1b9d-08dcf34d3900"),
                    Description = "Khóa học này sẽ cung cấp kiến thức cơ bản về lý thuyết thông tin, hệ thống máy tính và các phương pháp phát triển phần mềm, với trọng tâm vào lập trình hướng thủ tục " +
                    "(function-oriented programming). Học viên sẽ học các kỹ năng liên quan đến thiết kế chương trình, viết mã, kiểm thử và phát triển kỷ luật lập trình.\"",
                    StudentTask = "Học sinh có trách nhiệm làm tất cả các bài tập được giao bởi giảng viên trên lớp hoặc ở nhà và nộp đúng hạn",
                    TimeAllocation = "Giờ học (150 giờ) = 45 giờ học trên lớp(60 * 45') + 1 giờ thi cuối kỳ + 104 giờ tự học",
                    ToolsRequire = "- Internet\n- C language utility",
                    Scale = 10,
                    MinAvgMarkToPass = 5,
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
                    CourseCategoryId = Guid.Parse("f8fd80dd-c470-4ecf-7940-08dcf20adbbc"),
                    SyllabusId = Guid.Parse("990ca87d-c261-4cb3-1b9d-08dcf34d3900")

                }
            );
            #endregion
            #region Session
            modelBuilder.Entity<Session>().HasData(
            new Session { Id = Guid.NewGuid(), Number = 1, Topic = "Giới thiệu khóa học", Chapter = "Chương 1", Description = "Giới thiệu tổng quan về khóa học, các chủ đề sẽ được học, yêu cầu và phương pháp đánh giá.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 2, Topic = "Cài đặt Công cụ Lập trình", Chapter = "Chương 1", Description = "Hướng dẫn cài đặt và cấu hình công cụ lập trình, giới thiệu môi trường làm việc cho lập trình C.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 3, Topic = "Module A: Giới thiệu về ngôn ngữ lập trình C và Trình biên dịch C", Chapter = "Chương 1", Description = "Giới thiệu ngôn ngữ lập trình C, cách thức hoạt động của trình biên dịch C, và cú pháp cơ bản.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 4, Topic = "Giới thiệu về bài tập", Chapter = "Chương 1", Description = "Giới thiệu cấu trúc bài tập, cách thức nộp bài và yêu cầu cần đạt.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 5, Topic = "Module B: Tính toán - Biến số", Chapter = "Chương 2", Description = "Tìm hiểu về các biến trong C, cách khai báo, kiểu dữ liệu và cách thức sử dụng biến trong tính toán.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 6, Topic = "Module B: Tính toán - Các thao tác bộ nhớ cơ bản", Chapter = "Chương 2", Description = "Giải thích các thao tác bộ nhớ trong C, cách lưu trữ và xử lý dữ liệu trong bộ nhớ.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 7, Topic = "Tính toán cơ bản: Biểu thức", Chapter = "", Description = "Giới thiệu các biểu thức trong C, các phép toán cơ bản như cộng, trừ, nhân, chia, và các phép toán logic.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 8, Topic = "Module C: Lô-gic cơ bản - Cấu trúc trình tự, Cấu trúc lựa chọn", Chapter = "Chương 3", Description = "Học về các cấu trúc điều khiển trong C như cấu trúc trình tự và cấu trúc lựa chọn (if, switch).", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 9, Topic = "Module C: Lô-gic cơ bản - Cấu trúc lặp", Chapter = "Chương 3", Description = "Giới thiệu các cấu trúc lặp trong C như for, while, và do-while, cách sử dụng chúng trong lập trình.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 10, Topic = "Module C: Lô-gic cơ bản - Phong cách lập trình", Chapter = "Chương 3", Description = "Học cách viết mã có cấu trúc, dễ hiểu, tuân thủ các quy tắc về phong cách lập trình tốt.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 11, Topic = "Lô-gic cơ bản: Walkthroughs", Chapter = "Chương 3", Description = "Hướng dẫn chi tiết về cách sử dụng các cấu trúc logic trong việc giải quyết các bài toán thực tế.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 12, Topic = "Workshop 1: Nhập/Xuất, tính toán và lô-gic cơ bản", Chapter = "Chương 1, 2, 3", Description = "Thực hành về các kỹ năng nhập/xuất dữ liệu, tính toán và sử dụng các cấu trúc logic cơ bản.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 13, Topic = "Module D: Tính mô-đun và Hàm - Hàm C, Phạm vi biến", Chapter = "Chương 4", Description = "Giới thiệu khái niệm tính mô-đun, cách sử dụng hàm trong C và phạm vi của biến trong lập trình.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 14, Topic = "Đánh giá Workshop 1", Chapter = "Chương 1, 2, 3", Description = "Đánh giá kết quả của workshop 1 và phân tích lỗi thường gặp.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 15, Topic = "Tính mô-đun và Hàm", Chapter = "Chương 4", Description = "Tìm hiểu sâu hơn về cách chia chương trình thành các mô-đun và sử dụng hàm trong lập trình.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 16, Topic = "Tính mô-đun và Hàm", Chapter = "Chương 4", Description = "Thực hành viết và sử dụng hàm, tối ưu hóa mã nguồn bằng cách chia thành các mô-đun.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 17, Topic = "Tính mô-đun và Hàm", Chapter = "", Description = "Tiếp tục thực hành về hàm và tính mô-đun.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 18, Topic = "Workshop 2: Tính mô-đun và Hàm", Chapter = "Chương 4", Description = "Thực hành các bài tập liên quan đến tính mô-đun và sử dụng hàm trong C.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 19, Topic = "Con trỏ", Chapter = "Chương 4", Description = "Giới thiệu khái niệm con trỏ, cách khai báo, sử dụng và các ứng dụng của con trỏ trong lập trình.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 20, Topic = "Con trỏ", Chapter = "Chương 4", Description = "Thực hành với các bài tập sử dụng con trỏ để quản lý bộ nhớ và dữ liệu.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") },
            new Session { Id = Guid.NewGuid(), Number = 21, Topic = "Con trỏ", Chapter = "Chương 4", Description = "Tiếp tục thực hành và làm quen với con trỏ trong lập trình C.", CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014") }
            );
            #endregion
            #region Chapter
            modelBuilder.Entity<Chapter>().HasData(
            new Chapter
            {
                Id = Guid.Parse("fa0ef489-0a03-4901-8e0a-70fd69b324d3"),
                Number = 1,
                Topic = "Chương 1 : Giới thiệu về chương trình và cách nó hoạt động trên máy tính",
                Description = "Chương này sẽ trình bày cách một chương trình được tạo ra từ mã nguồn, biên dịch thành mã máy và được chạy trên máy tính. Học viên sẽ tìm hiểu về quá trình chuyển đổi từ mã lệnh thành một chương trình thực thi...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("c89711d7-1e02-4ec8-8c6f-7e232aa50f8c"),
                Number = 2,
                Topic = "Chương 2 : Biến, biểu thức và các phép toán cơ bản",
                Description = "Giải thích về khái niệm biến, biểu thức và các phép toán cơ bản trong lập trình C. Chương này sẽ cung cấp các ví dụ minh họa cách khai báo và sử dụng biến, cách thực hiện các phép toán số học...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("620d9ca5-c6a0-4b2c-9d20-b42635a9376c"),
                Number = 3,
                Topic = "Chương 3 : Cấu trúc logic và phong cách lập trình trong C",
                Description = "Giải thích về khái niệm biến, biểu thức và các phép toán cơ bản trong lập trình C. Chương này sẽ cung cấp các ví dụ minh họa cách khai báo và sử dụng biến...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("d42aaba5-c73e-4494-968c-4dda0baf33f4"),
                Number = 4,
                Topic = "Chương 4 : Tính modular và các hàm trong lập trình C",
                Description = "Giải thích về khái niệm biến, biểu thức và các phép toán cơ bản trong lập trình C. Chương này sẽ cung cấp các ví dụ minh họa cách khai báo và sử dụng biến...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("aad07753-6f1c-41ab-ae04-d6acad448216"),
                Number = 5,
                Topic = "Chương 5 : Thư viện C và cách sử dụng",
                Description = "Chương này sẽ giới thiệu các thư viện chuẩn của ngôn ngữ C, cách sử dụng chúng trong chương trình. Học viên sẽ học cách khai báo và sử dụng các hàm từ thư viện...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("b91e33b2-3810-4d7f-bafb-4190831e0ae4"),
                Number = 6,
                Topic = "Chương 6 : Mảng và cách sử dụng trong lập trình C",
                Description = "Học viên sẽ tìm hiểu về mảng (arrays) trong lập trình C, cách khai báo và sử dụng mảng một chiều và hai chiều...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("7aa7aa40-7f23-4c73-9375-7171a284f370"),
                Number = 7,
                Topic = "Chương 7 : Chuỗi và cách sử dụng trong lập trình C",
                Description = "Chương này tập trung vào khái niệm chuỗi (strings) trong ngôn ngữ C, cách khai báo và xử lý chuỗi...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Chapter
            {
                Id = Guid.Parse("c34aee85-2527-4b7e-9117-eff651cdad70"),
                Number = 8,
                Topic = "Chương 8 : Tệp và cách sử dụng trong lập trình C",
                Description = "Hướng dẫn cách sử dụng tệp trong lập trình C để lưu trữ và xử lý dữ liệu. Chương này sẽ bao gồm cách mở, đọc, ghi, và đóng tệp...",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            }
        );
            #endregion
            #region Assessment
            modelBuilder.Entity<Assessment>().HasData(
            new Assessment
            {
                Id = Guid.Parse("bf0ecd1a-27ba-4295-ba44-9d32bb103595"),
                Number = 1,
                Type = "Participation",
                Content = "Điểm danh",
                Quantity = 1,
                Weight = 10, // 10%
                CompletionCriteria = 0,
                Method = AssessmentMethodEnum.Online,
                Duration = null,
                QuestionType = null,
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Assessment
            {
                Id = Guid.Parse("74a5614a-56e5-43c9-9d9d-beb0ecda76c1"),
                Number = 2,
                Type = "Progress test",
                Content = "Luyện tập",
                Quantity = 2,
                Weight = 20, // 20%
                CompletionCriteria = 0,
                Method = AssessmentMethodEnum.Online,
                Duration = "20 phút",
                QuestionType = "Trắc nghiệm",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Assessment
            {
                Id = Guid.Parse("9841a317-70c1-4433-9644-a059194ef27d"),
                Number = 3,
                Type = "Assignment",
                Content = "Kiểm tra giữa khóa",
                Quantity = 1,
                Weight = 30, // 30%
                CompletionCriteria = 0,
                Method = AssessmentMethodEnum.Offline,
                Duration = "Tại nhà",
                QuestionType = "Giáo viên tự chọn",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            },
            new Assessment
            {
                Id = Guid.Parse("a385db00-01b3-46d0-932c-5c0d3a6a3fe9"),
                Number = 4,
                Type = "Final Exam",
                Content = "Kiểm tra cuối khóa",
                Quantity = 1,
                Weight = 40, // 40%
                CompletionCriteria = 4,
                Method = AssessmentMethodEnum.Online,
                Duration = "20 phút",
                QuestionType = "Trắc nghiệm",
                CourseId = Guid.Parse("1b182028-e25d-43b0-ba63-08dcf207c014")
            }
        );
            #endregion

            modelBuilder.HasAnnotation("TriggerSetup", true);
        }
        public override int SaveChanges()
        {
            CreateAgencyStatusTrigger();
            CreateClassRoomDatesTrigger();
            return base.SaveChanges();
        }
        private void CreateAgencyStatusTrigger()
        {
            Database.ExecuteSqlRaw(@"
            CREATE TRIGGER trg_UpdateUserStatusWhenAgencyStatusChange
            ON Agencies
            AFTER UPDATE
            AS
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM inserted i
                    JOIN deleted d ON i.Id = d.Id
                    WHERE i.Status IN (6) AND i.Status <> d.Status
                )
                BEGIN
                    UPDATE AspNetUsers
                    SET Status = 1
                    WHERE AgencyId IN (
                        SELECT Id
                        FROM inserted
                        WHERE Status IN (5, 6)
                    );
                END
            END;");
        }

        private void CreateClassRoomDatesTrigger()
        {
            Database.ExecuteSqlRaw(@"
            CREATE TRIGGER trg_UpdateClassRoomDates
            ON ClassSchedules
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                DECLARE @ClassId UNIQUEIDENTIFIER;
                IF EXISTS (SELECT * FROM inserted)
                BEGIN
                    SET @ClassId = (SELECT TOP 1 ClassId FROM inserted);
                END
                ELSE IF EXISTS (SELECT * FROM deleted)
                BEGIN
                    SET @ClassId = (SELECT TOP 1 ClassId FROM deleted);
                END
                IF @ClassId IS NOT NULL
                BEGIN
                    DECLARE @MinDate DATE;
                    DECLARE @MaxDate DATE;
                    SELECT 
                        @MinDate = MIN(Date),
                        @MaxDate = MAX(Date)
                    FROM ClassSchedules
                    WHERE ClassId = @ClassId;

                    UPDATE ClassRooms
                    SET FromDate = @MinDate,
                        ToDate = @MaxDate,
                        Status = 0
                    WHERE ClassId = @ClassId;
                END
            END;");
        }

    }
}
