using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateToneAnalyzer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Company_CompanyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserCvDatas_userId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "StrainScore",
                table: "ToneAnalysisResult");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_userId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserCvData1Id",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserCvData1Id",
                table: "Notifications",
                column: "UserCvData1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserCvDatas_UserCvData1Id",
                table: "Notifications",
                column: "UserCvData1Id",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserCvDatas_UserCvData1Id",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserCvData1Id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserCvData1Id",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notifications",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                newName: "IX_Notifications_userId");

            migrationBuilder.AddColumn<float>(
                name: "StrainScore",
                table: "ToneAnalysisResult",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Company_CompanyId",
                table: "Notifications",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserCvDatas_userId",
                table: "Notifications",
                column: "userId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
