using JN.ApiDemo.Identity.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JN.ApiDemo.Identity.Data
{

    // extending default IdentityUser class for AspNetCore.Identity
    // extend from IdentityDbContext for normal use; 
    // extend from IdentityDbContext<TUser, TRole, TKey> when User and Roles classes are used

    public class IdentityDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options)
            : base(options)
        {
        }

        public DbSet<ApiKey> ApiKeys { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ignore fields not needed
            modelBuilder.Entity<ApplicationUser>().Ignore(e => e.EmailConfirmed);
            modelBuilder.Entity<ApplicationUser>().Ignore(e => e.PhoneNumberConfirmed);
            

            // cascade delete relation from ApplicationUser to ApiKey Entities
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ApiKey>(p => p.ApiKeys)
                .WithOne(q => q.User)
                .OnDelete(DeleteBehavior.Cascade);



        }

        
    }
}
