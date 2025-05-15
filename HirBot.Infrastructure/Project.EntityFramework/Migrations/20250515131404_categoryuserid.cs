using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class categoryuserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Categories",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserID",
                table: "Categories",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_users_UserID",
                table: "Categories",
                column: "UserID",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_users_UserID",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Categories");
        }
    }
}
