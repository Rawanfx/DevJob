using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addInterviewSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    InterviewStatus = table.Column<int>(type: "int", nullable: false),
                    LiveKitRoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssessmentReport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSessions_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewSessions_UserCvDatas_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "UserCvDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_CandidateId",
                table: "InterviewSessions",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_JobId",
                table: "InterviewSessions",
                column: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewSessions");
        }
    }
}
