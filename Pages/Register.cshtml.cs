using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Services;
using WebApplication1.Attributes;
using WebApplication1.ViewModels;
using System.IO;
using System.Web;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    public class RegisterModel(
        UserManager<ApplicationUser> userManager,
        AesEncryptionService encryptionService,
        IWebHostEnvironment environment) : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AesEncryptionService _encryptionService = encryptionService;
        private readonly IWebHostEnvironment _environment = environment;

        [BindProperty]
        public Register Input { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index"); // Redirect to index if already logged in
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // HTML encode inputs to prevent XSS
                Input.Email = HttpUtility.HtmlEncode(Input.Email);
                Input.FirstName = HttpUtility.HtmlEncode(Input.FirstName);
                Input.LastName = HttpUtility.HtmlEncode(Input.LastName);
                Input.MobileNumber = HttpUtility.HtmlEncode(Input.MobileNumber);
                Input.BillingAddress = HttpUtility.HtmlEncode(Input.BillingAddress);
                Input.ShippingAddress = HttpUtility.HtmlEncode(Input.ShippingAddress);
                Input.CreditCardNo = HttpUtility.HtmlEncode(Input.CreditCardNo);

                // Handle file upload
                if (Input.Photo != null && Input.Photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Input.Photo.CopyToAsync(fileStream);
                    }

                    // Create user
                    var user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        EncryptedCreditCardNumber = _encryptionService.Encrypt(Input.CreditCardNo),
                        MobileNo = Input.MobileNumber,
                        BillingAddress = Input.BillingAddress,
                        ShippingAddress = Input.ShippingAddress,
                        PhotoPath = uniqueFileName
                    };

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        // Redirect to login page or homepage
                        return RedirectToPage("Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return Page();
        }
    }
}
