using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deviceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deviceId",
                table: "UserCvDatas");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "deviceId",
                table: "UserCvDatas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
