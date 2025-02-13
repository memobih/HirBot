using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurentJopID",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_CurentJopID",
                table: "users",
                column: "CurentJopID");

            migrationBuilder.AddForeignKey(
                name: "FK_users_Experiences_CurentJopID",
                table: "users",
                column: "CurentJopID",
                principalTable: "Experiences",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_Experiences_CurentJopID",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CurentJopID",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CurentJopID",
                table: "users");
        }
    }
}
