using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class AuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context) => _context = context;

        public async Task LogActivity(string userId, string activity)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Activity = activity,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }
}
