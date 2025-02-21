using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class updatesssssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start_Date",
                table: "Educations",
                newName: "startDate");

            migrationBuilder.RenameColumn(
                name: "End_Date",
                table: "Educations",
                newName: "endDate");

            migrationBuilder.AddColumn<string>(
                name: "companyName",
                table: "Experiences",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Privacy",
                table: "Educations",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "companyName",
                table: "Experiences");

            migrationBuilder.RenameColumn(
                name: "startDate",
                table: "Educations",
                newName: "Start_Date");

            migrationBuilder.RenameColumn(
                name: "endDate",
                table: "Educations",
                newName: "End_Date");

            migrationBuilder.AlterColumn<bool>(
                name: "Privacy",
                table: "Educations",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
