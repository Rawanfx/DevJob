using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpeechConfidence",
                table: "SpeechAnalysisResults");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "MockInterviews");

            migrationBuilder.DropColumn(
                name: "DominantEmotion",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "EmotionBreakdownJson",
                table: "FaceAnalyses");

            migrationBuilder.RenameColumn(
                name: "AvgSmile",
                table: "FaceAnalyses",
                newName: "PoorPostureWindowPct");

            migrationBuilder.RenameColumn(
                name: "AvgEyeContact",
                table: "FaceAnalyses",
                newName: "FramesWithPoseDetectedPct");

            migrationBuilder.AlterColumn<string>(
                name: "SuggestionsJson",
                table: "SpeechAnalysisResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "SpeechAnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SuggestionsJson",
                table: "FaceAnalyses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<float>(
                name: "AvgBrowTensionScore",
                table: "FaceAnalyses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "AvgEyeContactPct",
                table: "FaceAnalyses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "AvgHeadMovementScore",
                table: "FaceAnalyses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "BlinkRatePerMinute",
                table: "FaceAnalyses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "DominantHeadMovementType",
                table: "FaceAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "FramesWithFaceDetectedPct",
                table: "FaceAnalyses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "FramesWithHandDetectedPct",
                table: "FaceAnalyses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "TotalFaceTouchEvents",
                table: "FaceAnalyses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InterviewVideos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewVideos_MockInterviewQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "MockInterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MockInterviewReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MockInterviewId = table.Column<int>(type: "int", nullable: false),
                    OverallScore = table.Column<float>(type: "real", nullable: false),
                    CommunicationScore = table.Column<float>(type: "real", nullable: false),
                    ConfidenceScore = table.Column<float>(type: "real", nullable: false),
                    BodyLanguageScore = table.Column<float>(type: "real", nullable: false),
                    EmotionalProfile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpeechProfile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyLanguageSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockInterviewReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockInterviewReport_MockInterviews_MockInterviewId",
                        column: x => x.MockInterviewId,
                        principalTable: "MockInterviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToneAnalysisResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockInterviewQuestionId = table.Column<int>(type: "int", nullable: false),
                    DominantEmotion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmotionScoresJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PitchMean = table.Column<float>(type: "real", nullable: false),
                    PitchStd = table.Column<float>(type: "real", nullable: false),
                    EnergyMean = table.Column<float>(type: "real", nullable: false),
                    SpeakingRate = table.Column<float>(type: "real", nullable: false),
                    StrainScore = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToneAnalysisResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToneAnalysisResult_MockInterviewQuestions_MockInterviewQuestionId",
                        column: x => x.MockInterviewQuestionId,
                        principalTable: "MockInterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewVideos_QuestionId",
                table: "InterviewVideos",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviewReport_MockInterviewId",
                table: "MockInterviewReport",
                column: "MockInterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ToneAnalysisResult_MockInterviewQuestionId",
                table: "ToneAnalysisResult",
                column: "MockInterviewQuestionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewVideos");

            migrationBuilder.DropTable(
                name: "MockInterviewReport");

            migrationBuilder.DropTable(
                name: "ToneAnalysisResult");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "SpeechAnalysisResults");

            migrationBuilder.DropColumn(
                name: "AvgBrowTensionScore",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "AvgEyeContactPct",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "AvgHeadMovementScore",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "BlinkRatePerMinute",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "DominantHeadMovementType",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "FramesWithFaceDetectedPct",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "FramesWithHandDetectedPct",
                table: "FaceAnalyses");

            migrationBuilder.DropColumn(
                name: "TotalFaceTouchEvents",
                table: "FaceAnalyses");

            migrationBuilder.RenameColumn(
                name: "PoorPostureWindowPct",
                table: "FaceAnalyses",
                newName: "AvgSmile");

            migrationBuilder.RenameColumn(
                name: "FramesWithPoseDetectedPct",
                table: "FaceAnalyses",
                newName: "AvgEyeContact");

            migrationBuilder.AlterColumn<string>(
                name: "SuggestionsJson",
                table: "SpeechAnalysisResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SpeechConfidence",
                table: "SpeechAnalysisResults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "MockInterviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SuggestionsJson",
                table: "FaceAnalyses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DominantEmotion",
                table: "FaceAnalyses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmotionBreakdownJson",
                table: "FaceAnalyses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
