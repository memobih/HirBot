using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class edit323232 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotifiableType",
                table: "Notifications",
                newName: "Notifiable_Type");

            migrationBuilder.RenameColumn(
                name: "NotifiableID",
                table: "Notifications",
                newName: "Notifiable_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notifiable_Type",
                table: "Notifications",
                newName: "NotifiableType");

            migrationBuilder.RenameColumn(
                name: "Notifiable_ID",
                table: "Notifications",
                newName: "NotifiableID");
        }
    }
}
