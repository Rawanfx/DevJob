using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MockInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewSessions");

            migrationBuilder.CreateTable(
                name: "MockInterviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CvId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Track = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockInterviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockInterviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MockInterviews_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MockInterviews_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MockInterviewQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockInterviewId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AIFeedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    IsFollowUp = table.Column<bool>(type: "bit", nullable: false),
                    ParentQuestionId = table.Column<int>(type: "int", nullable: true),
                    AnsweredInSeconds = table.Column<int>(type: "int", nullable: true),
                    TimedOut = table.Column<bool>(type: "bit", nullable: false),
                    OverallConfidence = table.Column<float>(type: "real", nullable: true),
                    FinalScore = table.Column<float>(type: "real", nullable: false),
                    FinalAvgEyeContact = table.Column<float>(type: "real", nullable: false),
                    FinalSpeechConfidence = table.Column<float>(type: "real", nullable: false),
                    FinalFeedBack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalDominantEmotion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockInterviewQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockInterviewQuestions_MockInterviewQuestions_ParentQuestionId",
                        column: x => x.ParentQuestionId,
                        principalTable: "MockInterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MockInterviewQuestions_MockInterviews_MockInterviewId",
                        column: x => x.MockInterviewId,
                        principalTable: "MockInterviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FaceAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockInterviewQuestionId = table.Column<int>(type: "int", nullable: false),
                    AvgEyeContact = table.Column<float>(type: "real", nullable: false),
                    AvgSmile = table.Column<float>(type: "real", nullable: false),
                    DominantEmotion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmotionBreakdownJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformanceOverTimeJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuggestionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceAnalyses_MockInterviewQuestions_MockInterviewQuestionId",
                        column: x => x.MockInterviewQuestionId,
                        principalTable: "MockInterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpeechAnalysisResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockInterviewQuestionId = table.Column<int>(type: "int", nullable: false),
                    TranscribedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpeechConfidence = table.Column<float>(type: "real", nullable: false),
                    SpeechPace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WordsPerMinute = table.Column<float>(type: "real", nullable: false),
                    PauseCount = table.Column<int>(type: "int", nullable: false),
                    ClarityScore = table.Column<float>(type: "real", nullable: false),
                    SuggestionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeechAnalysisResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeechAnalysisResults_MockInterviewQuestions_MockInterviewQuestionId",
                        column: x => x.MockInterviewQuestionId,
                        principalTable: "MockInterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaceAnalyses_MockInterviewQuestionId",
                table: "FaceAnalyses",
                column: "MockInterviewQuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviewQuestions_MockInterviewId",
                table: "MockInterviewQuestions",
                column: "MockInterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviewQuestions_ParentQuestionId",
                table: "MockInterviewQuestions",
                column: "ParentQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviews_CvId",
                table: "MockInterviews",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviews_JobId",
                table: "MockInterviews",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviews_UserId",
                table: "MockInterviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeechAnalysisResults_MockInterviewQuestionId",
                table: "SpeechAnalysisResults",
                column: "MockInterviewQuestionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaceAnalyses");

            migrationBuilder.DropTable(
                name: "SpeechAnalysisResults");

            migrationBuilder.DropTable(
                name: "MockInterviewQuestions");

            migrationBuilder.DropTable(
                name: "MockInterviews");

            migrationBuilder.CreateTable(
                name: "InterviewSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    AssessmentReport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterviewStatus = table.Column<int>(type: "int", nullable: false),
                    LiveKitRoomName = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
