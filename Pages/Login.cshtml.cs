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
                     _sessionService.CreateSessionAsync(user.Id, sessionId);

                    // Store session ID in ASP.NET Core session
                    _httpContextAccessor.HttpContext.Session.SetString("SessionId", sessionId);
                    _httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id);

                    // Audit log
                    await _auditService.LogActivity(Convert.ToInt32(user.Id), "User logged in");

                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                }
            }
            return Page();
        }
    }
}
