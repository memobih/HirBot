using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class initupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_Experiences_CurentJopID",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CurentJopID",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_CurentJopID",
                table: "users",
                column: "CurentJopID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_Experiences_CurentJopID",
                table: "users",
                column: "CurentJopID",
                principalTable: "Experiences",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
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
    }
}
