using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class makeuserskillidnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_UserSkills_UserSkillID",
                table: "Exams");

            migrationBuilder.AlterColumn<int>(
                name: "UserSkillID",
                table: "Exams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_UserSkills_UserSkillID",
                table: "Exams",
                column: "UserSkillID",
                principalTable: "UserSkills",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_UserSkills_UserSkillID",
                table: "Exams");

            migrationBuilder.AlterColumn<int>(
                name: "UserSkillID",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_UserSkills_UserSkillID",
                table: "Exams",
                column: "UserSkillID",
                principalTable: "UserSkills",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
