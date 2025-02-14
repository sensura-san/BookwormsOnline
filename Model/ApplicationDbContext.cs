using WebApplication1.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Services;

namespace WebApplication1.Model
{
    public class ApplicationDbContext(IConfiguration configuration) : IdentityDbContext<ApplicationUser>
    {

        private readonly IConfiguration _configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        //public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }
    }

}
