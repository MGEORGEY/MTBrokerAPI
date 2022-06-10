using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MTBrokerAPI.Model;
using MTBrokerAPI.Model.FileMngt;

namespace MTBrokerAPI
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<MT940> MT940s { get; set; }


        public DbSet<Tag61And86Group> Tag61And86Groups { get; set; }

        public DbSet<UserFile> UserFiles { get; set; }

    }
}
