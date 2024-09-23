using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FranchiseProject.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: new Guid("550ee872-ea09-42a0-b9ac-809890debafb"),
                column: "EndTime",
                value: new DateTime(2024, 9, 28, 8, 44, 23, 197, DateTimeKind.Local).AddTicks(1278));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: new Guid("550ee872-ea09-42a0-b9ac-809890debafb"),
                column: "EndTime",
                value: new DateTime(2024, 9, 28, 8, 40, 49, 339, DateTimeKind.Local).AddTicks(181));
        }
    }
}
