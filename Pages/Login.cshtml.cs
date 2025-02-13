using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    // Login.cshtml.cs
    public class LoginModel(
        SignInManager<ApplicationUser> signInManager,
        SessionService sessionService,
        AuditService auditService
        ) : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly SessionService _sessionService = sessionService;
        private readonly AuditService _auditService = auditService;

        [BindProperty]
        public Login Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    isPersistent: false,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Get the user
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

                    // Generate session ID
                    var sessionId = Guid.NewGuid().ToString();

                    // Store session in database
                    await _sessionService.CreateSessionAsync(user.Id, sessionId);

                    // Store session ID in ASP.NET Core session
                    HttpContext.Session.SetString("SessionId", sessionId);
                    HttpContext.Session.SetString("UserId", user.Id);

                    // Audit log
                    await _auditService.LogActivity(user.Id, "User logged in");

                    return RedirectToPage("/Index");
                }
                else if (result.IsLockedOut)
                {
                    // Lockout error
                    ModelState.AddModelError(string.Empty, "Your account has been locked out due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    // Invalid login attempt
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return Page();
        }

    }
}
