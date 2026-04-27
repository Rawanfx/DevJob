using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chats_convesations_ConversationId",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "FK_convesations_userCvDatas_UserId",
                table: "convesations");

            migrationBuilder.DropForeignKey(
                name: "FK_cVs_AspNetUsers_UserId",
                table: "cVs");

            migrationBuilder.DropForeignKey(
                name: "FK_cVs_UserProfile_userprofileId",
                table: "cVs");

            migrationBuilder.DropForeignKey(
                name: "FK_educations_AspNetUsers_ApplicationUserId",
                table: "educations");

            migrationBuilder.DropForeignKey(
                name: "FK_educations_cVs_cvId",
                table: "educations");

            migrationBuilder.DropForeignKey(
                name: "FK_educations_userCvDatas_UserID",
                table: "educations");

            migrationBuilder.DropForeignKey(
                name: "FK_experiences_AspNetUsers_ApplicationUserId",
                table: "experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_experiences_cVs_cvId",
                table: "experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_experiences_userCvDatas_UserId",
                table: "experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_Company_CompanyId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_userCvDatas_userId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_cVs_cvId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_userCvDatas_UserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_cVs_cvId",
                table: "ProjectSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_skills_SkillId",
                table: "ProjectSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_recommendedJobs_Jobs_jobId",
                table: "recommendedJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_recommendedJobs_userCvDatas_userId",
                table: "recommendedJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_RequiredSkills_skills_SkillId",
                table: "RequiredSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedJobs_userCvDatas_userId",
                table: "SavedJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_searchKeyWords_cVs_cvId",
                table: "searchKeyWords");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_cVs_cvId",
                table: "skills");

            migrationBuilder.DropForeignKey(
                name: "FK_skillsGaps_userCvDatas_UserId",
                table: "skillsGaps");

            migrationBuilder.DropForeignKey(
                name: "FK_trainnings_cVs_cvId",
                table: "trainnings");

            migrationBuilder.DropForeignKey(
                name: "FK_trainnings_userCvDatas_UserId",
                table: "trainnings");

            migrationBuilder.DropForeignKey(
                name: "FK_userCvDatas_AspNetUsers_UserId",
                table: "userCvDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_userCvDatas_cVs_cvId",
                table: "userCvDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobs_cVs_cvId",
                table: "UserJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobs_userCvDatas_userId",
                table: "UserJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferenceـJobs_userCvDatas_userId",
                table: "UserPreferenceـJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_userPreferences_userCvDatas_userId",
                table: "userPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrefernce_Skills_skills_SkillId",
                table: "UserPrefernce_Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrefernce_Skills_userCvDatas_userCvData",
                table: "UserPrefernce_Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_userSkills_cVs_cvId",
                table: "userSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_userSkills_skills_SkillId",
                table: "userSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_userSkills_userCvDatas_UserId",
                table: "userSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userSkills",
                table: "userSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userCvDatas",
                table: "userCvDatas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_skills",
                table: "skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_searchKeyWords",
                table: "searchKeyWords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_recommendedJobs",
                table: "recommendedJobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_experiences",
                table: "experiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_educations",
                table: "educations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cVs",
                table: "cVs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_chats",
                table: "chats");

            migrationBuilder.RenameTable(
                name: "userSkills",
                newName: "UserSkills");

            migrationBuilder.RenameTable(
                name: "userCvDatas",
                newName: "UserCvDatas");

            migrationBuilder.RenameTable(
                name: "skills",
                newName: "Skills");

            migrationBuilder.RenameTable(
                name: "searchKeyWords",
                newName: "SearchKeyWords");

            migrationBuilder.RenameTable(
                name: "recommendedJobs",
                newName: "RecommendedJobs");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "experiences",
                newName: "Experiences");

            migrationBuilder.RenameTable(
                name: "educations",
                newName: "Educations");

            migrationBuilder.RenameTable(
                name: "cVs",
                newName: "CVs");

            migrationBuilder.RenameTable(
                name: "chats",
                newName: "Chats");

            migrationBuilder.RenameIndex(
                name: "IX_userSkills_UserId",
                table: "UserSkills",
                newName: "IX_UserSkills_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_userSkills_cvId",
                table: "UserSkills",
                newName: "IX_UserSkills_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_userCvDatas_UserId",
                table: "UserCvDatas",
                newName: "IX_UserCvDatas_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_userCvDatas_cvId",
                table: "UserCvDatas",
                newName: "IX_UserCvDatas_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_skills_cvId",
                table: "Skills",
                newName: "IX_Skills_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_searchKeyWords_cvId",
                table: "SearchKeyWords",
                newName: "IX_SearchKeyWords_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_recommendedJobs_userId",
                table: "RecommendedJobs",
                newName: "IX_RecommendedJobs_userId");

            migrationBuilder.RenameIndex(
                name: "IX_recommendedJobs_jobId",
                table: "RecommendedJobs",
                newName: "IX_RecommendedJobs_jobId");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_userId",
                table: "Notifications",
                newName: "IX_Notifications_userId");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_CompanyId",
                table: "Notifications",
                newName: "IX_Notifications_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_experiences_UserId",
                table: "Experiences",
                newName: "IX_Experiences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_experiences_cvId",
                table: "Experiences",
                newName: "IX_Experiences_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_experiences_ApplicationUserId",
                table: "Experiences",
                newName: "IX_Experiences_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_educations_UserID",
                table: "Educations",
                newName: "IX_Educations_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_educations_cvId",
                table: "Educations",
                newName: "IX_Educations_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_educations_ApplicationUserId",
                table: "Educations",
                newName: "IX_Educations_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_cVs_userprofileId",
                table: "CVs",
                newName: "IX_CVs_userprofileId");

            migrationBuilder.RenameIndex(
                name: "IX_cVs_UserId",
                table: "CVs",
                newName: "IX_CVs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_chats_ConversationId",
                table: "Chats",
                newName: "IX_Chats_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                columns: new[] { "SkillId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCvDatas",
                table: "UserCvDatas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                table: "Skills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchKeyWords",
                table: "SearchKeyWords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecommendedJobs",
                table: "RecommendedJobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Educations",
                table: "Educations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CVs",
                table: "CVs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_convesations_ConversationId",
                table: "Chats",
                column: "ConversationId",
                principalTable: "convesations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_convesations_UserCvDatas_UserId",
                table: "convesations",
                column: "UserId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_AspNetUsers_UserId",
                table: "CVs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_UserProfile_userprofileId",
                table: "CVs",
                column: "userprofileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_AspNetUsers_ApplicationUserId",
                table: "Educations",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_CVs_cvId",
                table: "Educations",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_UserCvDatas_UserID",
                table: "Educations",
                column: "UserID",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_AspNetUsers_ApplicationUserId",
                table: "Experiences",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_CVs_cvId",
                table: "Experiences",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_UserCvDatas_UserId",
                table: "Experiences",
                column: "UserId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CVs_cvId",
                table: "Projects",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_UserCvDatas_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_CVs_cvId",
                table: "ProjectSkills",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_Skills_SkillId",
                table: "ProjectSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendedJobs_Jobs_jobId",
                table: "RecommendedJobs",
                column: "jobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendedJobs_UserCvDatas_userId",
                table: "RecommendedJobs",
                column: "userId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredSkills_Skills_SkillId",
                table: "RequiredSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedJobs_UserCvDatas_userId",
                table: "SavedJobs",
                column: "userId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchKeyWords_CVs_cvId",
                table: "SearchKeyWords",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_CVs_cvId",
                table: "Skills",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skillsGaps_UserCvDatas_UserId",
                table: "skillsGaps",
                column: "UserId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trainnings_CVs_cvId",
                table: "trainnings",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trainnings_UserCvDatas_UserId",
                table: "trainnings",
                column: "UserId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCvDatas_AspNetUsers_UserId",
                table: "UserCvDatas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCvDatas_CVs_cvId",
                table: "UserCvDatas",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobs_CVs_cvId",
                table: "UserJobs",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobs_UserCvDatas_userId",
                table: "UserJobs",
                column: "userId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferenceـJobs_UserCvDatas_userId",
                table: "UserPreferenceـJobs",
                column: "userId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userPreferences_UserCvDatas_userId",
                table: "userPreferences",
                column: "userId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrefernce_Skills_Skills_SkillId",
                table: "UserPrefernce_Skills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrefernce_Skills_UserCvDatas_userCvData",
                table: "UserPrefernce_Skills",
                column: "userCvData",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_CVs_cvId",
                table: "UserSkills",
                column: "cvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_UserCvDatas_UserId",
                table: "UserSkills",
                column: "UserId",
                principalTable: "UserCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_convesations_ConversationId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_convesations_UserCvDatas_UserId",
                table: "convesations");

            migrationBuilder.DropForeignKey(
                name: "FK_CVs_AspNetUsers_UserId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_CVs_UserProfile_userprofileId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_AspNetUsers_ApplicationUserId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_CVs_cvId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_UserCvDatas_UserID",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_AspNetUsers_ApplicationUserId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_CVs_cvId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_UserCvDatas_UserId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Company_CompanyId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserCvDatas_userId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CVs_cvId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_UserCvDatas_UserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_CVs_cvId",
                table: "ProjectSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_Skills_SkillId",
                table: "ProjectSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_RecommendedJobs_Jobs_jobId",
                table: "RecommendedJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_RecommendedJobs_UserCvDatas_userId",
                table: "RecommendedJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_RequiredSkills_Skills_SkillId",
                table: "RequiredSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedJobs_UserCvDatas_userId",
                table: "SavedJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchKeyWords_CVs_cvId",
                table: "SearchKeyWords");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_CVs_cvId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_skillsGaps_UserCvDatas_UserId",
                table: "skillsGaps");

            migrationBuilder.DropForeignKey(
                name: "FK_trainnings_CVs_cvId",
                table: "trainnings");

            migrationBuilder.DropForeignKey(
                name: "FK_trainnings_UserCvDatas_UserId",
                table: "trainnings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCvDatas_AspNetUsers_UserId",
                table: "UserCvDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCvDatas_CVs_cvId",
                table: "UserCvDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobs_CVs_cvId",
                table: "UserJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobs_UserCvDatas_userId",
                table: "UserJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferenceـJobs_UserCvDatas_userId",
                table: "UserPreferenceـJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_userPreferences_UserCvDatas_userId",
                table: "userPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrefernce_Skills_Skills_SkillId",
                table: "UserPrefernce_Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrefernce_Skills_UserCvDatas_userCvData",
                table: "UserPrefernce_Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_CVs_cvId",
                table: "UserSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_UserCvDatas_UserId",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCvDatas",
                table: "UserCvDatas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchKeyWords",
                table: "SearchKeyWords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecommendedJobs",
                table: "RecommendedJobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Educations",
                table: "Educations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CVs",
                table: "CVs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.RenameTable(
                name: "UserSkills",
                newName: "userSkills");

            migrationBuilder.RenameTable(
                name: "UserCvDatas",
                newName: "userCvDatas");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "skills");

            migrationBuilder.RenameTable(
                name: "SearchKeyWords",
                newName: "searchKeyWords");

            migrationBuilder.RenameTable(
                name: "RecommendedJobs",
                newName: "recommendedJobs");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "Experiences",
                newName: "experiences");

            migrationBuilder.RenameTable(
                name: "Educations",
                newName: "educations");

            migrationBuilder.RenameTable(
                name: "CVs",
                newName: "cVs");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "chats");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkills_UserId",
                table: "userSkills",
                newName: "IX_userSkills_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkills_cvId",
                table: "userSkills",
                newName: "IX_userSkills_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCvDatas_UserId",
                table: "userCvDatas",
                newName: "IX_userCvDatas_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCvDatas_cvId",
                table: "userCvDatas",
                newName: "IX_userCvDatas_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_cvId",
                table: "skills",
                newName: "IX_skills_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchKeyWords_cvId",
                table: "searchKeyWords",
                newName: "IX_searchKeyWords_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_RecommendedJobs_userId",
                table: "recommendedJobs",
                newName: "IX_recommendedJobs_userId");

            migrationBuilder.RenameIndex(
                name: "IX_RecommendedJobs_jobId",
                table: "recommendedJobs",
                newName: "IX_recommendedJobs_jobId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_userId",
                table: "notifications",
                newName: "IX_notifications_userId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CompanyId",
                table: "notifications",
                newName: "IX_notifications_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_UserId",
                table: "experiences",
                newName: "IX_experiences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_cvId",
                table: "experiences",
                newName: "IX_experiences_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_ApplicationUserId",
                table: "experiences",
                newName: "IX_experiences_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_UserID",
                table: "educations",
                newName: "IX_educations_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_cvId",
                table: "educations",
                newName: "IX_educations_cvId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_ApplicationUserId",
                table: "educations",
                newName: "IX_educations_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CVs_userprofileId",
                table: "cVs",
                newName: "IX_cVs_userprofileId");

            migrationBuilder.RenameIndex(
                name: "IX_CVs_UserId",
                table: "cVs",
                newName: "IX_cVs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_ConversationId",
                table: "chats",
                newName: "IX_chats_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userSkills",
                table: "userSkills",
                columns: new[] { "SkillId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_userCvDatas",
                table: "userCvDatas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_skills",
                table: "skills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_searchKeyWords",
                table: "searchKeyWords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_recommendedJobs",
                table: "recommendedJobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notifications",
                table: "notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_experiences",
                table: "experiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_educations",
                table: "educations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cVs",
                table: "cVs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_chats",
                table: "chats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chats_convesations_ConversationId",
                table: "chats",
                column: "ConversationId",
                principalTable: "convesations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_convesations_userCvDatas_UserId",
                table: "convesations",
                column: "UserId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cVs_AspNetUsers_UserId",
                table: "cVs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cVs_UserProfile_userprofileId",
                table: "cVs",
                column: "userprofileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_educations_AspNetUsers_ApplicationUserId",
                table: "educations",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_educations_cVs_cvId",
                table: "educations",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_educations_userCvDatas_UserID",
                table: "educations",
                column: "UserID",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_experiences_AspNetUsers_ApplicationUserId",
                table: "experiences",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_experiences_cVs_cvId",
                table: "experiences",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_experiences_userCvDatas_UserId",
                table: "experiences",
                column: "UserId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_Company_CompanyId",
                table: "notifications",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_userCvDatas_userId",
                table: "notifications",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_cVs_cvId",
                table: "Projects",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_userCvDatas_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_cVs_cvId",
                table: "ProjectSkills",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_skills_SkillId",
                table: "ProjectSkills",
                column: "SkillId",
                principalTable: "skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recommendedJobs_Jobs_jobId",
                table: "recommendedJobs",
                column: "jobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_recommendedJobs_userCvDatas_userId",
                table: "recommendedJobs",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredSkills_skills_SkillId",
                table: "RequiredSkills",
                column: "SkillId",
                principalTable: "skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedJobs_userCvDatas_userId",
                table: "SavedJobs",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_searchKeyWords_cVs_cvId",
                table: "searchKeyWords",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_cVs_cvId",
                table: "skills",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skillsGaps_userCvDatas_UserId",
                table: "skillsGaps",
                column: "UserId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trainnings_cVs_cvId",
                table: "trainnings",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trainnings_userCvDatas_UserId",
                table: "trainnings",
                column: "UserId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userCvDatas_AspNetUsers_UserId",
                table: "userCvDatas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userCvDatas_cVs_cvId",
                table: "userCvDatas",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobs_cVs_cvId",
                table: "UserJobs",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobs_userCvDatas_userId",
                table: "UserJobs",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferenceـJobs_userCvDatas_userId",
                table: "UserPreferenceـJobs",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userPreferences_userCvDatas_userId",
                table: "userPreferences",
                column: "userId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrefernce_Skills_skills_SkillId",
                table: "UserPrefernce_Skills",
                column: "SkillId",
                principalTable: "skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrefernce_Skills_userCvDatas_userCvData",
                table: "UserPrefernce_Skills",
                column: "userCvData",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userSkills_cVs_cvId",
                table: "userSkills",
                column: "cvId",
                principalTable: "cVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userSkills_skills_SkillId",
                table: "userSkills",
                column: "SkillId",
                principalTable: "skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userSkills_userCvDatas_UserId",
                table: "userSkills",
                column: "UserId",
                principalTable: "userCvDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
