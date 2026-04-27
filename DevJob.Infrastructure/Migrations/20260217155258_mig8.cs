using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cvId",
                table: "UserJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "YearOfex",
                table: "userCvDatas",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserJobs_cvId",
                table: "UserJobs",
                column: "cvId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobs_cVs_cvId",
                table: "UserJobs",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobs_cVs_cvId",
                table: "UserJobs");

            migrationBuilder.DropIndex(
                name: "IX_UserJobs_cvId",
                table: "UserJobs");

            migrationBuilder.DropColumn(
                name: "cvId",
                table: "UserJobs");

            migrationBuilder.DropColumn(
                name: "YearOfex",
                table: "userCvDatas");
        }
    }
}
