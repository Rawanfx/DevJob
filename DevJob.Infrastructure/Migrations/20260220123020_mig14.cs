using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_AspNetUsers_userId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "deviceId",
                table: "userCvDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_userCvDatas_userId",
                table: "notifications",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_userCvDatas_userId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "deviceId",
                table: "userCvDatas");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "notifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_AspNetUsers_userId",
                table: "notifications",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
