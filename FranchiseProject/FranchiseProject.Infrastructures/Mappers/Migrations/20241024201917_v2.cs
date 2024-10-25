using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FranchiseProject.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("1698026c-c435-4f16-882c-396bc076c5d6"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("1b8bfca0-126c-48a1-8da1-0c584397b9f6"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("2b2c84c8-5b03-4ed2-8ac8-a06f8061fa06"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("3e7f0744-3265-48d5-b08f-5e2307da47be"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("48743490-8ecd-4ff6-8db9-e9fb8dd77ca3"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("49c5ffeb-0296-4022-b484-70ac50b6aae3"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("4d7d0dd4-abc7-41e7-8dac-754342fae51d"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("63f537b9-70dc-4593-9a7b-eaba8c8b596e"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("673b704e-df6c-4459-9ec0-6f15b8c3ac94"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("6a94f91c-0a97-4b1b-ad49-33c1ee440266"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("705d552d-cff3-44bd-baf3-6ebda385295f"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("777bd3d1-152d-4b8c-8309-8a554e02caf0"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("85360bd2-533e-43ae-972a-0bbd0b8ffc71"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("c4995cd8-be05-4a45-8aef-3497c35efbac"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("d3223adf-c908-4d2e-afe0-ba6e1122614e"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("dc4f8e76-a008-45ca-8ac0-2c16f2751a5b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("e520c029-26f9-4053-9154-f3f465d81d97"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("e92ff7f8-8d13-4182-b136-1522c3825bdd"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("f46745fe-eaca-442b-a9cf-7c356566e1b4"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("f53bb695-b311-497a-8711-46092edbf144"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("fbaab02a-7182-4086-820a-45d59ba24d75"));

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "QuestionOptions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: new Guid("550ee872-ea09-42a0-b9ac-809890debafb"),
                column: "EndTime",
                value: new DateTime(2024, 10, 30, 3, 19, 13, 837, DateTimeKind.Local).AddTicks(7327));

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "Chapter", "CourseId", "CreatedBy", "CreationDate", "DeleteBy", "DeletionDate", "Description", "IsDeleted", "ModificationBy", "ModificationDate", "Number", "Topic" },
                values: new object[,]
                {
                    { new Guid("0555da39-610f-4d15-a072-224564575216"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm tính mô-đun, cách sử dụng hàm trong C và phạm vi của biến trong lập trình.", false, null, null, 13, "Module D: Tính mô-đun và Hàm - Hàm C, Phạm vi biến" },
                    { new Guid("0d3b2195-0fcc-4556-a485-4fa529912292"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu tổng quan về khóa học, các chủ đề sẽ được học, yêu cầu và phương pháp đánh giá.", false, null, null, 1, "Giới thiệu khóa học" },
                    { new Guid("145cebbf-fdfa-4e67-bf49-ba45e5d8891b"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các biểu thức trong C, các phép toán cơ bản như cộng, trừ, nhân, chia, và các phép toán logic.", false, null, null, 7, "Tính toán cơ bản: Biểu thức" },
                    { new Guid("1cc3bc29-9fdc-4101-89f2-6a6b15582775"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học cách viết mã có cấu trúc, dễ hiểu, tuân thủ các quy tắc về phong cách lập trình tốt.", false, null, null, 10, "Module C: Lô-gic cơ bản - Phong cách lập trình" },
                    { new Guid("227d48c2-37f5-448d-b70c-440051cd6f33"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đánh giá kết quả của workshop 1 và phân tích lỗi thường gặp.", false, null, null, 14, "Đánh giá Workshop 1" },
                    { new Guid("357e48aa-e058-4c87-a684-fc8f323c37c7"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu về các biến trong C, cách khai báo, kiểu dữ liệu và cách thức sử dụng biến trong tính toán.", false, null, null, 5, "Module B: Tính toán - Biến số" },
                    { new Guid("36a79128-689a-4fb6-a6df-9dc8b134b05d"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu cấu trúc bài tập, cách thức nộp bài và yêu cầu cần đạt.", false, null, null, 4, "Giới thiệu về bài tập" },
                    { new Guid("36bacec8-4c8c-4b31-bdff-148bf4ab1980"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu ngôn ngữ lập trình C, cách thức hoạt động của trình biên dịch C, và cú pháp cơ bản.", false, null, null, 3, "Module A: Giới thiệu về ngôn ngữ lập trình C và Trình biên dịch C" },
                    { new Guid("384a7ab3-d1de-4489-a759-707c00986acc"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn cài đặt và cấu hình công cụ lập trình, giới thiệu môi trường làm việc cho lập trình C.", false, null, null, 2, "Cài đặt Công cụ Lập trình" },
                    { new Guid("3f062ce7-6bb3-4920-8639-06ec1e7e7f1d"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giải thích các thao tác bộ nhớ trong C, cách lưu trữ và xử lý dữ liệu trong bộ nhớ.", false, null, null, 6, "Module B: Tính toán - Các thao tác bộ nhớ cơ bản" },
                    { new Guid("5843c1f8-5ea2-4795-a14b-36fffb6047f5"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành về hàm và tính mô-đun.", false, null, null, 17, "Tính mô-đun và Hàm" },
                    { new Guid("6ef1fde9-25f6-4028-b4a2-1a6a1b6a767b"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm con trỏ, cách khai báo, sử dụng và các ứng dụng của con trỏ trong lập trình.", false, null, null, 19, "Con trỏ" },
                    { new Guid("8b303783-80d6-40bb-9981-781ab21d91aa"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành với các bài tập sử dụng con trỏ để quản lý bộ nhớ và dữ liệu.", false, null, null, 20, "Con trỏ" },
                    { new Guid("8e79a290-a54b-4417-be74-f81e47111686"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn chi tiết về cách sử dụng các cấu trúc logic trong việc giải quyết các bài toán thực tế.", false, null, null, 11, "Lô-gic cơ bản: Walkthroughs" },
                    { new Guid("924aa5ee-7c89-4d70-b1f9-d98513e8a46c"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các cấu trúc lặp trong C như for, while, và do-while, cách sử dụng chúng trong lập trình.", false, null, null, 9, "Module C: Lô-gic cơ bản - Cấu trúc lặp" },
                    { new Guid("9caa5e45-d719-482d-aa07-1868cf096dce"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành viết và sử dụng hàm, tối ưu hóa mã nguồn bằng cách chia thành các mô-đun.", false, null, null, 16, "Tính mô-đun và Hàm" },
                    { new Guid("9da9fe8a-827c-4e92-b7bd-455c7e81f5d6"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học về các cấu trúc điều khiển trong C như cấu trúc trình tự và cấu trúc lựa chọn (if, switch).", false, null, null, 8, "Module C: Lô-gic cơ bản - Cấu trúc trình tự, Cấu trúc lựa chọn" },
                    { new Guid("a1ffb37c-3674-454f-af1f-42c7d4b1bcbd"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành về các kỹ năng nhập/xuất dữ liệu, tính toán và sử dụng các cấu trúc logic cơ bản.", false, null, null, 12, "Workshop 1: Nhập/Xuất, tính toán và lô-gic cơ bản" },
                    { new Guid("a60b0386-e39d-48a6-b42e-3f0dab559b36"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu sâu hơn về cách chia chương trình thành các mô-đun và sử dụng hàm trong lập trình.", false, null, null, 15, "Tính mô-đun và Hàm" },
                    { new Guid("a67768b5-1c90-4c77-be12-c214d5644b3b"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành các bài tập liên quan đến tính mô-đun và sử dụng hàm trong C.", false, null, null, 18, "Workshop 2: Tính mô-đun và Hàm" },
                    { new Guid("ea35a8f3-e7d2-4c2e-bdd6-b4448cfd5fde"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành và làm quen với con trỏ trong lập trình C.", false, null, null, 21, "Con trỏ" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("0555da39-610f-4d15-a072-224564575216"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("0d3b2195-0fcc-4556-a485-4fa529912292"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("145cebbf-fdfa-4e67-bf49-ba45e5d8891b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("1cc3bc29-9fdc-4101-89f2-6a6b15582775"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("227d48c2-37f5-448d-b70c-440051cd6f33"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("357e48aa-e058-4c87-a684-fc8f323c37c7"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("36a79128-689a-4fb6-a6df-9dc8b134b05d"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("36bacec8-4c8c-4b31-bdff-148bf4ab1980"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("384a7ab3-d1de-4489-a759-707c00986acc"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("3f062ce7-6bb3-4920-8639-06ec1e7e7f1d"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("5843c1f8-5ea2-4795-a14b-36fffb6047f5"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("6ef1fde9-25f6-4028-b4a2-1a6a1b6a767b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("8b303783-80d6-40bb-9981-781ab21d91aa"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("8e79a290-a54b-4417-be74-f81e47111686"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("924aa5ee-7c89-4d70-b1f9-d98513e8a46c"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("9caa5e45-d719-482d-aa07-1868cf096dce"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("9da9fe8a-827c-4e92-b7bd-455c7e81f5d6"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("a1ffb37c-3674-454f-af1f-42c7d4b1bcbd"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("a60b0386-e39d-48a6-b42e-3f0dab559b36"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("a67768b5-1c90-4c77-be12-c214d5644b3b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("ea35a8f3-e7d2-4c2e-bdd6-b4448cfd5fde"));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "QuestionOptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: new Guid("550ee872-ea09-42a0-b9ac-809890debafb"),
                column: "EndTime",
                value: new DateTime(2024, 10, 29, 15, 26, 51, 95, DateTimeKind.Local).AddTicks(8413));

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "Chapter", "CourseId", "CreatedBy", "CreationDate", "DeleteBy", "DeletionDate", "Description", "IsDeleted", "ModificationBy", "ModificationDate", "Number", "Topic" },
                values: new object[,]
                {
                    { new Guid("1698026c-c435-4f16-882c-396bc076c5d6"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu sâu hơn về cách chia chương trình thành các mô-đun và sử dụng hàm trong lập trình.", false, null, null, 15, "Tính mô-đun và Hàm" },
                    { new Guid("1b8bfca0-126c-48a1-8da1-0c584397b9f6"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành về hàm và tính mô-đun.", false, null, null, 17, "Tính mô-đun và Hàm" },
                    { new Guid("2b2c84c8-5b03-4ed2-8ac8-a06f8061fa06"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành và làm quen với con trỏ trong lập trình C.", false, null, null, 21, "Con trỏ" },
                    { new Guid("3e7f0744-3265-48d5-b08f-5e2307da47be"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn chi tiết về cách sử dụng các cấu trúc logic trong việc giải quyết các bài toán thực tế.", false, null, null, 11, "Lô-gic cơ bản: Walkthroughs" },
                    { new Guid("48743490-8ecd-4ff6-8db9-e9fb8dd77ca3"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn cài đặt và cấu hình công cụ lập trình, giới thiệu môi trường làm việc cho lập trình C.", false, null, null, 2, "Cài đặt Công cụ Lập trình" },
                    { new Guid("49c5ffeb-0296-4022-b484-70ac50b6aae3"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các biểu thức trong C, các phép toán cơ bản như cộng, trừ, nhân, chia, và các phép toán logic.", false, null, null, 7, "Tính toán cơ bản: Biểu thức" },
                    { new Guid("4d7d0dd4-abc7-41e7-8dac-754342fae51d"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành với các bài tập sử dụng con trỏ để quản lý bộ nhớ và dữ liệu.", false, null, null, 20, "Con trỏ" },
                    { new Guid("63f537b9-70dc-4593-9a7b-eaba8c8b596e"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu cấu trúc bài tập, cách thức nộp bài và yêu cầu cần đạt.", false, null, null, 4, "Giới thiệu về bài tập" },
                    { new Guid("673b704e-df6c-4459-9ec0-6f15b8c3ac94"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các cấu trúc lặp trong C như for, while, và do-while, cách sử dụng chúng trong lập trình.", false, null, null, 9, "Module C: Lô-gic cơ bản - Cấu trúc lặp" },
                    { new Guid("6a94f91c-0a97-4b1b-ad49-33c1ee440266"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành các bài tập liên quan đến tính mô-đun và sử dụng hàm trong C.", false, null, null, 18, "Workshop 2: Tính mô-đun và Hàm" },
                    { new Guid("705d552d-cff3-44bd-baf3-6ebda385295f"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu ngôn ngữ lập trình C, cách thức hoạt động của trình biên dịch C, và cú pháp cơ bản.", false, null, null, 3, "Module A: Giới thiệu về ngôn ngữ lập trình C và Trình biên dịch C" },
                    { new Guid("777bd3d1-152d-4b8c-8309-8a554e02caf0"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành về các kỹ năng nhập/xuất dữ liệu, tính toán và sử dụng các cấu trúc logic cơ bản.", false, null, null, 12, "Workshop 1: Nhập/Xuất, tính toán và lô-gic cơ bản" },
                    { new Guid("85360bd2-533e-43ae-972a-0bbd0b8ffc71"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm con trỏ, cách khai báo, sử dụng và các ứng dụng của con trỏ trong lập trình.", false, null, null, 19, "Con trỏ" },
                    { new Guid("c4995cd8-be05-4a45-8aef-3497c35efbac"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học về các cấu trúc điều khiển trong C như cấu trúc trình tự và cấu trúc lựa chọn (if, switch).", false, null, null, 8, "Module C: Lô-gic cơ bản - Cấu trúc trình tự, Cấu trúc lựa chọn" },
                    { new Guid("d3223adf-c908-4d2e-afe0-ba6e1122614e"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành viết và sử dụng hàm, tối ưu hóa mã nguồn bằng cách chia thành các mô-đun.", false, null, null, 16, "Tính mô-đun và Hàm" },
                    { new Guid("dc4f8e76-a008-45ca-8ac0-2c16f2751a5b"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đánh giá kết quả của workshop 1 và phân tích lỗi thường gặp.", false, null, null, 14, "Đánh giá Workshop 1" },
                    { new Guid("e520c029-26f9-4053-9154-f3f465d81d97"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm tính mô-đun, cách sử dụng hàm trong C và phạm vi của biến trong lập trình.", false, null, null, 13, "Module D: Tính mô-đun và Hàm - Hàm C, Phạm vi biến" },
                    { new Guid("e92ff7f8-8d13-4182-b136-1522c3825bdd"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học cách viết mã có cấu trúc, dễ hiểu, tuân thủ các quy tắc về phong cách lập trình tốt.", false, null, null, 10, "Module C: Lô-gic cơ bản - Phong cách lập trình" },
                    { new Guid("f46745fe-eaca-442b-a9cf-7c356566e1b4"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu tổng quan về khóa học, các chủ đề sẽ được học, yêu cầu và phương pháp đánh giá.", false, null, null, 1, "Giới thiệu khóa học" },
                    { new Guid("f53bb695-b311-497a-8711-46092edbf144"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu về các biến trong C, cách khai báo, kiểu dữ liệu và cách thức sử dụng biến trong tính toán.", false, null, null, 5, "Module B: Tính toán - Biến số" },
                    { new Guid("fbaab02a-7182-4086-820a-45d59ba24d75"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giải thích các thao tác bộ nhớ trong C, cách lưu trữ và xử lý dữ liệu trong bộ nhớ.", false, null, null, 6, "Module B: Tính toán - Các thao tác bộ nhớ cơ bản" }
                });
        }
    }
}
