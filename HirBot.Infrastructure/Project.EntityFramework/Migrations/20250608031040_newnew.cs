using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class newnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoApply",
                table: "users",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsAuto",
                table: "Applications",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAuto",
                table: "Applications");

            migrationBuilder.AlterColumn<int>(
                name: "IsAutoApply",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
