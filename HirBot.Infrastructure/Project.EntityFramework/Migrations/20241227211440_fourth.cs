using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Satue",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "WebsiteUrle",
                table: "Companies",
                newName: "websiteUrl");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Companies",
                newName: "street");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Companies",
                newName: "TaxIndtefierNumber");

            migrationBuilder.RenameColumn(
                name: "BusinessLicence",
                table: "Companies",
                newName: "SocialMeediaLink");

            migrationBuilder.RenameColumn(
                name: "AdditionalInformation",
                table: "Companies",
                newName: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "Companies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Governate",
                table: "Companies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "Companies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Governate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "country",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "websiteUrl",
                table: "Companies",
                newName: "WebsiteUrle");

            migrationBuilder.RenameColumn(
                name: "street",
                table: "Companies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "TaxIndtefierNumber",
                table: "Companies",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "SocialMeediaLink",
                table: "Companies",
                newName: "BusinessLicence");

            migrationBuilder.RenameColumn(
                name: "Comments",
                table: "Companies",
                newName: "AdditionalInformation");

            migrationBuilder.AddColumn<bool>(
                name: "Satue",
                table: "Companies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
