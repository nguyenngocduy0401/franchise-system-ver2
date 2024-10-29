using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FranchiseProject.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class newv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("038c49cd-929f-4e5c-b446-48f9875c973c"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("04da7b1d-68d8-4017-b509-fc9f05b325bc"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("38bfbe76-0a8e-4db0-b492-ea393485ddac"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("54f7f5d9-726f-4845-9079-7ba332448e7e"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("5a93d5b5-4de1-409f-af7d-4f213479bc58"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("5ced9876-2c56-4319-86f9-8a301e034230"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("61e833b1-b297-4916-ac99-cc50bd36d3f1"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("68c95ad3-8873-4a0d-bd84-b6abc5d531ea"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("73adb397-640d-4f09-b498-e26f94e11f4f"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("8212e263-3362-431b-ab17-e93a903cf701"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("8e9d12bc-a62c-4147-a1da-46c5a7a26cbd"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("98b6e13d-e8a1-45ed-aa1f-8248b4113c1d"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("a858f6fe-eef5-4fde-b197-4a0b4dece83f"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("aa15ff35-3f71-4b4e-9c8c-259ef803b94a"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("ac8f3e63-605a-4b8a-95f6-b6381e52276b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("b13242da-c9c4-48f6-8488-afbb5d1b18e5"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("b3e5e84e-402c-43af-b9a4-77b56688ca6b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("cb527b71-0163-4f51-9bfc-5c6b3721760d"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("ddc2b201-f29a-49fe-94a4-834f2ce06e15"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("e4e96e78-e096-41c1-8ad9-bd65c32b94f4"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("ebc40d39-03f0-4d18-b578-232436c110cd"));

            migrationBuilder.AlterColumn<double>(
                name: "CompletionCriteria",
                table: "Assessments",
                type: "float",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("74a5614a-56e5-43c9-9d9d-beb0ecda76c1"),
                column: "CompletionCriteria",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("9841a317-70c1-4433-9644-a059194ef27d"),
                column: "CompletionCriteria",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("a385db00-01b3-46d0-932c-5c0d3a6a3fe9"),
                column: "CompletionCriteria",
                value: 4.0);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("bf0ecd1a-27ba-4295-ba44-9d32bb103595"),
                column: "CompletionCriteria",
                value: 0.0);

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: new Guid("550ee872-ea09-42a0-b9ac-809890debafb"),
                column: "EndTime",
                value: new DateTime(2024, 11, 2, 18, 11, 17, 733, DateTimeKind.Local).AddTicks(3416));

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "Chapter", "CourseId", "CreatedBy", "CreationDate", "DeleteBy", "DeletionDate", "Description", "IsDeleted", "ModificationBy", "ModificationDate", "Number", "Topic" },
                values: new object[,]
                {
                    { new Guid("051eb0d3-c2fc-46b6-9f44-4e328a53e99f"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm tính mô-đun, cách sử dụng hàm trong C và phạm vi của biến trong lập trình.", false, null, null, 13, "Module D: Tính mô-đun và Hàm - Hàm C, Phạm vi biến" },
                    { new Guid("08afc0a2-5e7d-474b-bee4-ae1a25c451cb"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn chi tiết về cách sử dụng các cấu trúc logic trong việc giải quyết các bài toán thực tế.", false, null, null, 11, "Lô-gic cơ bản: Walkthroughs" },
                    { new Guid("225555ca-561b-4f83-8215-bdb62021725b"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành về các kỹ năng nhập/xuất dữ liệu, tính toán và sử dụng các cấu trúc logic cơ bản.", false, null, null, 12, "Workshop 1: Nhập/Xuất, tính toán và lô-gic cơ bản" },
                    { new Guid("2ef77ceb-f0d6-4c70-a408-ee0515df3281"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành các bài tập liên quan đến tính mô-đun và sử dụng hàm trong C.", false, null, null, 18, "Workshop 2: Tính mô-đun và Hàm" },
                    { new Guid("3032d9ab-e5b7-46bb-8a9c-dbfbf93d61bb"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu về các biến trong C, cách khai báo, kiểu dữ liệu và cách thức sử dụng biến trong tính toán.", false, null, null, 5, "Module B: Tính toán - Biến số" },
                    { new Guid("39112aa5-53be-462d-a361-f89d78d25553"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn cài đặt và cấu hình công cụ lập trình, giới thiệu môi trường làm việc cho lập trình C.", false, null, null, 2, "Cài đặt Công cụ Lập trình" },
                    { new Guid("3f7c4928-ddee-4a96-8cad-6711dbb2ab64"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đánh giá kết quả của workshop 1 và phân tích lỗi thường gặp.", false, null, null, 14, "Đánh giá Workshop 1" },
                    { new Guid("6a37f398-3502-4c3e-a061-217e1a4a2ae4"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu tổng quan về khóa học, các chủ đề sẽ được học, yêu cầu và phương pháp đánh giá.", false, null, null, 1, "Giới thiệu khóa học" },
                    { new Guid("7252e890-bfea-4de8-8b43-96189945e287"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành viết và sử dụng hàm, tối ưu hóa mã nguồn bằng cách chia thành các mô-đun.", false, null, null, 16, "Tính mô-đun và Hàm" },
                    { new Guid("8258da51-3b28-4258-ac22-e507600360cb"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm con trỏ, cách khai báo, sử dụng và các ứng dụng của con trỏ trong lập trình.", false, null, null, 19, "Con trỏ" },
                    { new Guid("88a165da-78c3-44cd-b015-be0584408389"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các cấu trúc lặp trong C như for, while, và do-while, cách sử dụng chúng trong lập trình.", false, null, null, 9, "Module C: Lô-gic cơ bản - Cấu trúc lặp" },
                    { new Guid("89d85ef3-726a-4ddc-a722-4b1b97ed6248"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành về hàm và tính mô-đun.", false, null, null, 17, "Tính mô-đun và Hàm" },
                    { new Guid("8cc96ecd-c0fb-4eeb-b187-7dd694f177bd"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu sâu hơn về cách chia chương trình thành các mô-đun và sử dụng hàm trong lập trình.", false, null, null, 15, "Tính mô-đun và Hàm" },
                    { new Guid("9717a6ca-9bd0-4187-95ed-61c46a42b2d7"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học cách viết mã có cấu trúc, dễ hiểu, tuân thủ các quy tắc về phong cách lập trình tốt.", false, null, null, 10, "Module C: Lô-gic cơ bản - Phong cách lập trình" },
                    { new Guid("c85a1203-44a3-4d35-af1e-da62cca0a94d"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giải thích các thao tác bộ nhớ trong C, cách lưu trữ và xử lý dữ liệu trong bộ nhớ.", false, null, null, 6, "Module B: Tính toán - Các thao tác bộ nhớ cơ bản" },
                    { new Guid("c8f39a34-5fd0-4d3e-b881-a4c538109254"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các biểu thức trong C, các phép toán cơ bản như cộng, trừ, nhân, chia, và các phép toán logic.", false, null, null, 7, "Tính toán cơ bản: Biểu thức" },
                    { new Guid("d3077af2-ebb6-4e38-9f23-d4b1ee0d189f"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu ngôn ngữ lập trình C, cách thức hoạt động của trình biên dịch C, và cú pháp cơ bản.", false, null, null, 3, "Module A: Giới thiệu về ngôn ngữ lập trình C và Trình biên dịch C" },
                    { new Guid("d5373f90-54ed-4f54-82c9-9420be34a4b8"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành và làm quen với con trỏ trong lập trình C.", false, null, null, 21, "Con trỏ" },
                    { new Guid("e8c6d155-81d8-4568-b1fd-468d7bc04832"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học về các cấu trúc điều khiển trong C như cấu trúc trình tự và cấu trúc lựa chọn (if, switch).", false, null, null, 8, "Module C: Lô-gic cơ bản - Cấu trúc trình tự, Cấu trúc lựa chọn" },
                    { new Guid("eff5134f-660d-4882-b1c4-03401ef7edf9"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành với các bài tập sử dụng con trỏ để quản lý bộ nhớ và dữ liệu.", false, null, null, 20, "Con trỏ" },
                    { new Guid("f06b4993-71af-46d4-a2e6-d9571ecb0921"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu cấu trúc bài tập, cách thức nộp bài và yêu cầu cần đạt.", false, null, null, 4, "Giới thiệu về bài tập" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("051eb0d3-c2fc-46b6-9f44-4e328a53e99f"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("08afc0a2-5e7d-474b-bee4-ae1a25c451cb"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("225555ca-561b-4f83-8215-bdb62021725b"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("2ef77ceb-f0d6-4c70-a408-ee0515df3281"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("3032d9ab-e5b7-46bb-8a9c-dbfbf93d61bb"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("39112aa5-53be-462d-a361-f89d78d25553"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("3f7c4928-ddee-4a96-8cad-6711dbb2ab64"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("6a37f398-3502-4c3e-a061-217e1a4a2ae4"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("7252e890-bfea-4de8-8b43-96189945e287"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("8258da51-3b28-4258-ac22-e507600360cb"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("88a165da-78c3-44cd-b015-be0584408389"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("89d85ef3-726a-4ddc-a722-4b1b97ed6248"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("8cc96ecd-c0fb-4eeb-b187-7dd694f177bd"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("9717a6ca-9bd0-4187-95ed-61c46a42b2d7"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("c85a1203-44a3-4d35-af1e-da62cca0a94d"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("c8f39a34-5fd0-4d3e-b881-a4c538109254"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("d3077af2-ebb6-4e38-9f23-d4b1ee0d189f"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("d5373f90-54ed-4f54-82c9-9420be34a4b8"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("e8c6d155-81d8-4568-b1fd-468d7bc04832"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("eff5134f-660d-4882-b1c4-03401ef7edf9"));

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: new Guid("f06b4993-71af-46d4-a2e6-d9571ecb0921"));

            migrationBuilder.AlterColumn<string>(
                name: "CompletionCriteria",
                table: "Assessments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("74a5614a-56e5-43c9-9d9d-beb0ecda76c1"),
                column: "CompletionCriteria",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("9841a317-70c1-4433-9644-a059194ef27d"),
                column: "CompletionCriteria",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("a385db00-01b3-46d0-932c-5c0d3a6a3fe9"),
                column: "CompletionCriteria",
                value: "4");

            migrationBuilder.UpdateData(
                table: "Assessments",
                keyColumn: "Id",
                keyValue: new Guid("bf0ecd1a-27ba-4295-ba44-9d32bb103595"),
                column: "CompletionCriteria",
                value: "0");

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: new Guid("550ee872-ea09-42a0-b9ac-809890debafb"),
                column: "EndTime",
                value: new DateTime(2024, 11, 1, 13, 32, 38, 736, DateTimeKind.Local).AddTicks(6141));

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "Chapter", "CourseId", "CreatedBy", "CreationDate", "DeleteBy", "DeletionDate", "Description", "IsDeleted", "ModificationBy", "ModificationDate", "Number", "Topic" },
                values: new object[,]
                {
                    { new Guid("038c49cd-929f-4e5c-b446-48f9875c973c"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu tổng quan về khóa học, các chủ đề sẽ được học, yêu cầu và phương pháp đánh giá.", false, null, null, 1, "Giới thiệu khóa học" },
                    { new Guid("04da7b1d-68d8-4017-b509-fc9f05b325bc"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học về các cấu trúc điều khiển trong C như cấu trúc trình tự và cấu trúc lựa chọn (if, switch).", false, null, null, 8, "Module C: Lô-gic cơ bản - Cấu trúc trình tự, Cấu trúc lựa chọn" },
                    { new Guid("38bfbe76-0a8e-4db0-b492-ea393485ddac"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành về các kỹ năng nhập/xuất dữ liệu, tính toán và sử dụng các cấu trúc logic cơ bản.", false, null, null, 12, "Workshop 1: Nhập/Xuất, tính toán và lô-gic cơ bản" },
                    { new Guid("54f7f5d9-726f-4845-9079-7ba332448e7e"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Học cách viết mã có cấu trúc, dễ hiểu, tuân thủ các quy tắc về phong cách lập trình tốt.", false, null, null, 10, "Module C: Lô-gic cơ bản - Phong cách lập trình" },
                    { new Guid("5a93d5b5-4de1-409f-af7d-4f213479bc58"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành về hàm và tính mô-đun.", false, null, null, 17, "Tính mô-đun và Hàm" },
                    { new Guid("5ced9876-2c56-4319-86f9-8a301e034230"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn cài đặt và cấu hình công cụ lập trình, giới thiệu môi trường làm việc cho lập trình C.", false, null, null, 2, "Cài đặt Công cụ Lập trình" },
                    { new Guid("61e833b1-b297-4916-ac99-cc50bd36d3f1"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giải thích các thao tác bộ nhớ trong C, cách lưu trữ và xử lý dữ liệu trong bộ nhớ.", false, null, null, 6, "Module B: Tính toán - Các thao tác bộ nhớ cơ bản" },
                    { new Guid("68c95ad3-8873-4a0d-bd84-b6abc5d531ea"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hướng dẫn chi tiết về cách sử dụng các cấu trúc logic trong việc giải quyết các bài toán thực tế.", false, null, null, 11, "Lô-gic cơ bản: Walkthroughs" },
                    { new Guid("73adb397-640d-4f09-b498-e26f94e11f4f"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tiếp tục thực hành và làm quen với con trỏ trong lập trình C.", false, null, null, 21, "Con trỏ" },
                    { new Guid("8212e263-3362-431b-ab17-e93a903cf701"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành các bài tập liên quan đến tính mô-đun và sử dụng hàm trong C.", false, null, null, 18, "Workshop 2: Tính mô-đun và Hàm" },
                    { new Guid("8e9d12bc-a62c-4147-a1da-46c5a7a26cbd"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành viết và sử dụng hàm, tối ưu hóa mã nguồn bằng cách chia thành các mô-đun.", false, null, null, 16, "Tính mô-đun và Hàm" },
                    { new Guid("98b6e13d-e8a1-45ed-aa1f-8248b4113c1d"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu cấu trúc bài tập, cách thức nộp bài và yêu cầu cần đạt.", false, null, null, 4, "Giới thiệu về bài tập" },
                    { new Guid("a858f6fe-eef5-4fde-b197-4a0b4dece83f"), "Chương 1", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu ngôn ngữ lập trình C, cách thức hoạt động của trình biên dịch C, và cú pháp cơ bản.", false, null, null, 3, "Module A: Giới thiệu về ngôn ngữ lập trình C và Trình biên dịch C" },
                    { new Guid("aa15ff35-3f71-4b4e-9c8c-259ef803b94a"), "Chương 2", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu về các biến trong C, cách khai báo, kiểu dữ liệu và cách thức sử dụng biến trong tính toán.", false, null, null, 5, "Module B: Tính toán - Biến số" },
                    { new Guid("ac8f3e63-605a-4b8a-95f6-b6381e52276b"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tìm hiểu sâu hơn về cách chia chương trình thành các mô-đun và sử dụng hàm trong lập trình.", false, null, null, 15, "Tính mô-đun và Hàm" },
                    { new Guid("b13242da-c9c4-48f6-8488-afbb5d1b18e5"), "Chương 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các cấu trúc lặp trong C như for, while, và do-while, cách sử dụng chúng trong lập trình.", false, null, null, 9, "Module C: Lô-gic cơ bản - Cấu trúc lặp" },
                    { new Guid("b3e5e84e-402c-43af-b9a4-77b56688ca6b"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm con trỏ, cách khai báo, sử dụng và các ứng dụng của con trỏ trong lập trình.", false, null, null, 19, "Con trỏ" },
                    { new Guid("cb527b71-0163-4f51-9bfc-5c6b3721760d"), "Chương 1, 2, 3", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đánh giá kết quả của workshop 1 và phân tích lỗi thường gặp.", false, null, null, 14, "Đánh giá Workshop 1" },
                    { new Guid("ddc2b201-f29a-49fe-94a4-834f2ce06e15"), "", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu các biểu thức trong C, các phép toán cơ bản như cộng, trừ, nhân, chia, và các phép toán logic.", false, null, null, 7, "Tính toán cơ bản: Biểu thức" },
                    { new Guid("e4e96e78-e096-41c1-8ad9-bd65c32b94f4"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thực hành với các bài tập sử dụng con trỏ để quản lý bộ nhớ và dữ liệu.", false, null, null, 20, "Con trỏ" },
                    { new Guid("ebc40d39-03f0-4d18-b578-232436c110cd"), "Chương 4", new Guid("1b182028-e25d-43b0-ba63-08dcf207c014"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Giới thiệu khái niệm tính mô-đun, cách sử dụng hàm trong C và phạm vi của biến trong lập trình.", false, null, null, 13, "Module D: Tính mô-đun và Hàm - Hàm C, Phạm vi biến" }
                });
        }
    }
}
