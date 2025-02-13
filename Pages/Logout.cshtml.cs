using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    // Logout.cshtml.cs
    public class LogoutModel(
SignInManager<ApplicationUser> signInManager,
SessionService sessionService,
ApplicationDbContext context) : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly SessionService _sessionService = sessionService;
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Clear session from database
            var sessions = _context.UserSessions.Where(s => s.UserId == userId);
            _context.UserSessions.RemoveRange(sessions);
            await _context.SaveChangesAsync();

            // Clear ASP.NET Core session
            HttpContext.Session.Clear();


            // Sign out
            await _signInManager.SignOutAsync();

            return RedirectToPage("/Login");
        }
    }
}
