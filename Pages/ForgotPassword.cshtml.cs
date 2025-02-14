
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public string Email { get; set; }

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Page("/ResetPassword",
                        pageHandler: null,
                        values: new { email = Email, token },
                        protocol: Request.Scheme);

                    // Send email with resetLink (implement IEmailSender)
                    // Temporary implementation:
                    TempData["ResetLink"] = resetLink;
                    return RedirectToPage("/ForgotPasswordConfirmation");
                }
            }

            ModelState.AddModelError(string.Empty, "If your email exists, we've sent a reset link");
            return Page();
        }
    }
}