


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RadarService.Authorization.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContext) : base(dbContext)
        {

        }
        //private static DbContextOptions GetOptions(string connectionString)
        //{
        //    return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        //=> optionsBuilder.UseSqlServer("Server=localhost\\SQLExpress;Database=RadarAuthDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
