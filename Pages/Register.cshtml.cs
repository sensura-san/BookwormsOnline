using WebApplication1.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Services;
using WebApplication1.Attributes;
using WebApplication1.ViewModels;

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

        // this probs binds the viewmodel to the page????
        [BindProperty]
        public Register Input { get; set; }

        public void OnGet()
        {
            Console.WriteLine("Checkpoint: Returning Page()");
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Handle file upload
        //        if (Input.Photo != null && Input.Photo.Length > 0)
        //        {
        //            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.Photo.FileName;
        //            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await Input.Photo.CopyToAsync(fileStream);
        //            }

        //            // Create user
        //            var user = new ApplicationUser
        //            {
        //                UserName = Input.Email,
        //                Email = Input.Email,
        //                FirstName = Input.FirstName,
        //                LastName = Input.LastName,
        //                EncryptedCreditCardNumber = _encryptionService.Encrypt(Input.CreditCardNo),
        //                MobileNo = Input.MobileNumber,
        //                BillingAddress = Input.BillingAddress,
        //                ShippingAddress = Input.ShippingAddress,
        //                PhotoPath = uniqueFileName
        //            };

        //            var result = await _userManager.CreateAsync(user, Input.Password);

        //            if (result.Succeeded)
        //            {
        //                // Redirect to login page or homepage
        //                return RedirectToPage("Login");
        //            }

        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //    }

        //    return Page();
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Console.WriteLine("Checkpoint: Entered OnPostAsync");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Checkpoint: ModelState is invalid");
                    return Page();
                }

                Console.WriteLine("Checkpoint: ModelState is valid");

                // DEBUG: Commenting out file upload temporarily
                string? uniqueFileName = null;

                /*
                if (Input.Photo != null && Input.Photo.Length > 0)
                {
                    Console.WriteLine("Checkpoint: Handling file upload");
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Input.Photo.CopyToAsync(fileStream);
                    }
                }
                */

                Console.WriteLine("Checkpoint: Creating new user");
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

                Console.WriteLine($"Checkpoint: User {user.Email} created (before saving)");
                var result = await _userManager.CreateAsync(user, Input.Password);
                Console.WriteLine($"Checkpoint: User creation result: {result.Succeeded}");

                if (result.Succeeded)
                {
                    Console.WriteLine("Checkpoint: User created successfully");

                    // DEBUG: Temporarily disabling session handling
                    /*
                    Console.WriteLine("Checkpoint: Storing session");
                    var sessionId = Guid.NewGuid().ToString();
                    await _sessionService.CreateSessionAsync(user.Id, sessionId);
                    HttpContext.Session.SetString("SessionId", sessionId);
                    HttpContext.Session.SetString("UserId", user.Id);
                    */

                    // DEBUG: Temporarily disabling audit logging
                    /*
                    Console.WriteLine("Checkpoint: Logging user activity");
                    await _auditService.LogActivity(Convert.ToInt32(user.Id), "User registered");
                    */

                    Console.WriteLine("Checkpoint: Redirecting to Login page");
                    return RedirectToPage("Login");
                }

                Console.WriteLine("Checkpoint: User creation failed. Adding errors to ModelState.");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            return Page();
        }

    }
}
