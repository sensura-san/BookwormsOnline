using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;

namespace WebApplication1.Model
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Activity { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
