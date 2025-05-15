using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HirBot.Data.Entities;
using HirBot.Comman.Idenitity;

namespace HirBot.EntityFramework.DataBaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Specify the foreign key
            modelBuilder.Entity<IdentityRole>().HasData(
                  new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                  new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
              ); 
            modelBuilder.Entity<ApplicationUser>().ToTable("users");
            modelBuilder.Entity<IdentityRole>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("User_Logins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
            modelBuilder.Entity<ApplicationUser>()
           .HasOne(u => u.CurentJop)
           .WithOne(e=>e.UserJop)
           .OnDelete(DeleteBehavior.SetNull); // Set to NULL on delete
           
        } 
          
        public DbSet<ApplicationUser> users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobRequirment> JobRequirements { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserAnwer> UserAnswers { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; } 
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<NotificationReciver> NotificationRecivers { get; set; }
    }
}
