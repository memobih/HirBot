using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class updatesoneducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "startDate",
                table: "Educations",
                newName: "Start_Date");

            migrationBuilder.RenameColumn(
                name: "endDate",
                table: "Educations",
                newName: "End_Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start_Date",
                table: "Educations",
                newName: "startDate");

            migrationBuilder.RenameColumn(
                name: "End_Date",
                table: "Educations",
                newName: "endDate");
        }
    }
}
