using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class testquestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exams_ExamID",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "UserAnswers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ExamID",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_UserID",
                table: "UserAnswers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exams_ExamID",
                table: "Questions",
                column: "ExamID",
                principalTable: "Exams",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_users_UserID",
                table: "UserAnswers",
                column: "UserID",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exams_ExamID",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_users_UserID",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_UserID",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "ExamID",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exams_ExamID",
                table: "Questions",
                column: "ExamID",
                principalTable: "Exams",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
