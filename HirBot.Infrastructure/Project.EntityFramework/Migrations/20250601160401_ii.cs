using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SocialMeediaLink",
                table: "Companies",
                newName: "TwitterLink");

            migrationBuilder.AddColumn<string>(
                name: "FacebookLink",
                table: "Companies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InstgrameLink",
                table: "Companies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TikTokLink",
                table: "Companies",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookLink",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "InstgrameLink",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TikTokLink",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "TwitterLink",
                table: "Companies",
                newName: "SocialMeediaLink");
        }
    }
}
