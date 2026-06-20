using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMockInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "MockInterviews");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "MockInterviews",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "MockInterviews",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "MockInterviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
