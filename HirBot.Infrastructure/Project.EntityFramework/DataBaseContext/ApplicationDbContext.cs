using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

            modelBuilder.Entity<ApplicationUser>().ToTable("users");
            modelBuilder.Entity<IdentityRole>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("User_Logins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_tokens"); 
        } 

        public DbSet<ApplicationUser> users { get; set; }

    }
}
