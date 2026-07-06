using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserCvDatas_UserCvData1Id",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserCvData1Id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserCvData1Id",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
       

            
          
        }
    }
}
