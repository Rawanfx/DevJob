using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_CompanyId",
                table: "notifications",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_Company_CompanyId",
                table: "notifications",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_Company_CompanyId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_CompanyId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "notifications");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
