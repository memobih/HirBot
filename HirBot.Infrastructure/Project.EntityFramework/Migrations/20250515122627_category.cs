using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CategoryID",
                table: "Exams",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Categories_CategoryID",
                table: "Exams",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Categories_CategoryID",
                table: "Exams");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Exams_CategoryID",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Exams");
        }
    }
}
