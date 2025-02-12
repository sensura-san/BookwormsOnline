using WebApplication1.Model;

namespace WebApplication1.Model
{
    public class AuditLog
    {
       public int UserId { get; set; }
        public string Activity { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
