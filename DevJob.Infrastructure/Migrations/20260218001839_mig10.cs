using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobLevel",
                table: "userPreferences");

            migrationBuilder.RenameColumn(
                name: "JobType",
                table: "userPreferences",
                newName: "ExperienceLevel");

            migrationBuilder.CreateTable(
                name: "UserPreferenceـJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobLevel = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferenceـJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferenceـJobs_userCvDatas_userId",
                        column: x => x.userId,
                        principalTable: "userCvDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPrefernce_Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userCvData = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrefernce_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrefernce_Skills_userCvDatas_userCvData",
                        column: x => x.userCvData,
                        principalTable: "userCvDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferenceـJobs_userId",
                table: "UserPreferenceـJobs",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrefernce_Skills_userCvData",
                table: "UserPrefernce_Skills",
                column: "userCvData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferenceـJobs");

            migrationBuilder.DropTable(
                name: "UserPrefernce_Skills");

            migrationBuilder.RenameColumn(
                name: "ExperienceLevel",
                table: "userPreferences",
                newName: "JobType");

            migrationBuilder.AddColumn<int>(
                name: "JobLevel",
                table: "userPreferences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
