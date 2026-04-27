using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillName",
                table: "UserPrefernce_Skills");

            migrationBuilder.RenameColumn(
                name: "ExperienceLevel",
                table: "userPreferences",
                newName: "JobLevel");

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "UserPrefernce_Skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserPrefernce_Skills_SkillId",
                table: "UserPrefernce_Skills",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrefernce_Skills_skills_SkillId",
                table: "UserPrefernce_Skills",
                column: "SkillId",
                principalTable: "skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPrefernce_Skills_skills_SkillId",
                table: "UserPrefernce_Skills");

            migrationBuilder.DropIndex(
                name: "IX_UserPrefernce_Skills_SkillId",
                table: "UserPrefernce_Skills");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "UserPrefernce_Skills");

            migrationBuilder.RenameColumn(
                name: "JobLevel",
                table: "userPreferences",
                newName: "ExperienceLevel");

            migrationBuilder.AddColumn<string>(
                name: "SkillName",
                table: "UserPrefernce_Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
