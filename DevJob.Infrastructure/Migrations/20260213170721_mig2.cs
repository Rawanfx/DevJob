using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recommendedJobs_userCvDatas_userId",
                table: "recommendedJobs");

            migrationBuilder.DropIndex(
                name: "IX_recommendedJobs_userId",
                table: "recommendedJobs");

            migrationBuilder.AddColumn<int>(
                name: "UserCvData1Id",
                table: "recommendedJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_recommendedJobs_UserCvData1Id",
                table: "recommendedJobs",
                column: "UserCvData1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_recommendedJobs_userCvDatas_UserCvData1Id",
                table: "recommendedJobs",
                column: "UserCvData1Id",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recommendedJobs_userCvDatas_UserCvData1Id",
                table: "recommendedJobs");

            migrationBuilder.DropIndex(
                name: "IX_recommendedJobs_UserCvData1Id",
                table: "recommendedJobs");

            migrationBuilder.DropColumn(
                name: "UserCvData1Id",
                table: "recommendedJobs");

            migrationBuilder.CreateIndex(
                name: "IX_recommendedJobs_userId",
                table: "recommendedJobs",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_recommendedJobs_userCvDatas_userId",
                table: "recommendedJobs",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
