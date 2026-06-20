using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMockInterviewQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectPoints",
                table: "MockInterviewQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MissingPoints",
                table: "MockInterviewQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SuggestedAnswer",
                table: "MockInterviewQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectPoints",
                table: "MockInterviewQuestions");

            migrationBuilder.DropColumn(
                name: "MissingPoints",
                table: "MockInterviewQuestions");

            migrationBuilder.DropColumn(
                name: "SuggestedAnswer",
                table: "MockInterviewQuestions");
        }
    }
}
