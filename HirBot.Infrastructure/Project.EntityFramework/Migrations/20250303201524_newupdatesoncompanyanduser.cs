using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class newupdatesoncompanyanduser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Companies",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserID",
                table: "Companies",
                column: "UserID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_users_UserID",
                table: "Companies",
                column: "UserID",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_users_UserID",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserID",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Companies");

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
    }
}
