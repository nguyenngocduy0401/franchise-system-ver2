using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FranchiseProject.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class mb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "FranchiseRegistrationRequests",
                newName: "CusomterName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "FranchiseRegistrationRequests",
                newName: "ConsultantUserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CusomterName",
                table: "FranchiseRegistrationRequests",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "ConsultantUserName",
                table: "FranchiseRegistrationRequests",
                newName: "Name");
        }
    }
}
