using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;

namespace WebApplication1.Pages
{
    [Authorize]
    public class IndexModel(UserManager<ApplicationUser> userManager) : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserMobileNumber { get; set; }
        public string UserBillingAddress { get; set; }
        public string UserShippingAddress { get; set; }
        public string UserPhotoPath { get; set; }
        public string UserEncryptedCreditCardNo { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                UserFirstName = user.FirstName;
                UserLastName = user.LastName;
                UserEmail = user.Email;
                UserMobileNumber = user.MobileNo;
                UserBillingAddress = user.BillingAddress;
                UserShippingAddress = user.ShippingAddress;
                UserPhotoPath = user.PhotoPath;
                UserEncryptedCreditCardNo = user.EncryptedCreditCardNumber;


            }
        }
    }
}
