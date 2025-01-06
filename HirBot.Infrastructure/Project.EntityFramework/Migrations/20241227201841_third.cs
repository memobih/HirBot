using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyID",
                table: "users",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_users_CompanyID",
                table: "users",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_users_Companies_CompanyID",
                table: "users",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_Companies_CompanyID",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CompanyID",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "users");
        }
    }
}
