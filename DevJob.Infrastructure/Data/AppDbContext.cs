using DevJob.Application.DTOs.Jobs;
using DevJob.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace DevJob.Infrastructure.Data
{ public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<SavedJobs> SavedJobs { get; set; }
        public DbSet<Conversation> convesations { get; set; }
      
       public DbSet<chats> Chats { get; set; }
        public DbSet<Notification> Notifications { get; set; }
       public DbSet<CompanyProfile> Company { get; set; }
       public DbSet<UserProfile> UserProfile { get; set; }
       public DbSet<Job> Jobs { get; set; }
    
        public DbSet<RequiredSkills> RequiredSkills { get; set; }
        public DbSet<UserCvData> UserCvDatas { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<ProjectSkills> ProjectSkills { get; set; }
        public DbSet<SkillsGap> skillsGaps { get; set; }
        public DbSet<Trainning> trainnings { get; set; }
        public DbSet<UserJob> UserJobs{ get; set; }
        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<SearchKeyWord> SearchKeyWords { get; set; }
        public DbSet<RecommendedJobs> RecommendedJobs { get; set; }
        public DbSet<UserPreferenceـJobs> UserPreferenceـJobs { get; set; }
        public DbSet<UserPrefernce_Skills> UserPrefernce_Skills { get; set; }
        public DbSet<UserPreference> userPreferences { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            const string id1 = "fba5305e-4f8f-4317-870f-89c909d74049";
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole() { Name = "Company", Id = id1, NormalizedName = "COMPANY" },
                new IdentityRole() { Name = "Developer", Id = "52bea598-e636-4ef7-ba98-c1bf09f66a63", NormalizedName = "DEVELOPER" });


         
            foreach (var relationship in builder.Model.GetEntityTypes()
         .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
           

            builder.Entity<UserSkills>()
                .HasKey(x => new { x.SkillId, x.UserId });

            builder.Entity<UserSkills>()
                .HasOne(x => x.UserCvData)
                .WithMany(x => x.UserSkills)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserCvData>()
                .HasOne(x => x.CV)
                .WithMany(x => x.UserCvDatas)
                .HasForeignKey(x => x.cvId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectSkills>()
                .HasOne(x => x.Projects)
                .WithMany(x => x.ProjectSkills)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProjectSkills>()
                .HasOne(x => x.Skills)
                .WithMany(x => x.ProjectSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProjectSkills>()
                .HasKey(x => new { x.SkillId, x.ProjectId })
                ;
            builder.Entity<UserSkills>()
                .HasOne(x => x.Skills)
                .WithMany(x => x.UserSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserJob>()
                .HasOne(x => x.UserCvData)
                .WithMany(x => x.UserJobs)
                .HasForeignKey(x => x.userId);
            builder.Entity<UserJob>()
                .HasOne(x => x.job)
                .WithMany(x => x.UserJobs)
                .HasForeignKey(x => x.jobID);
            builder.Entity<UserJob>()
                .HasKey(x => new { x.jobID, x.userId });

            builder.Entity<RequiredSkills>()
                .HasKey(x => new { x.SkillId, x.JobId });
            builder.Entity<RequiredSkills>()
            .HasOne(x => x.Job)
            .WithMany(x => x.RequiredSkills)
            .HasForeignKey(x => x.JobId);
            builder.Entity<RequiredSkills>()
                .HasOne(x => x.skills)
                .WithMany(x => x.requiredSkills)
                .HasForeignKey(x => x.SkillId);

            builder.Entity<Skills>()
        .HasOne(s => s.CV)
        .WithMany(c => c.Skills) 
        .HasForeignKey(s => s.cvId)
        .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
