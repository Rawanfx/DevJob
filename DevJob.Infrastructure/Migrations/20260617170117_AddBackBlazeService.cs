using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBackBlazeService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "MockInterviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "MockInterviews");
        }
    }
}
