using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class updateexperiences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyID",
                table: "Experiences",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "rate",
                table: "Experiences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Companies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_CompanyID",
                table: "Experiences",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Companies_CompanyID",
                table: "Experiences",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Companies_CompanyID",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_CompanyID",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "rate",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Companies");
        }
    }
}
