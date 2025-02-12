using WebApplication1.Model;

namespace WebApplication1.Services
{
    // SessionService.cs
    public class SessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateSessionAsync(string userId, string sessionId)
        {
            // Remove existing sessions for this user (optional)
            var existingSessions = _context.UserSessions.Where(s => s.UserId == userId);
            _context.UserSessions.RemoveRange(existingSessions);

            // Add new session
            // this is alr safe :D cuz it uses the model as a parameter, thus preventing sql injection
            // https://stackoverflow.com/questions/3968466/how-secure-is-entity-framework
            _context.UserSessions.Add(new UserSession
            {
                UserId = userId,
                SessionId = sessionId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(20)
            });
            await _context.SaveChangesAsync();
        }

        public bool ValidateSession(string userId, string sessionId)
        {
            return _context.UserSessions.Any(s =>
                s.UserId == userId &&
                s.SessionId == sessionId &&
                s.ExpiresAt > DateTime.UtcNow);
        }
    }
}
