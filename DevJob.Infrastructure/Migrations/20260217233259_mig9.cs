using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobLevel = table.Column<int>(type: "int", nullable: false),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    MinimumSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userPreferences_userCvDatas_userId",
                        column: x => x.userId,
                        principalTable: "userCvDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userPreferences_userId",
                table: "userPreferences",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userPreferences");
        }
    }
}
