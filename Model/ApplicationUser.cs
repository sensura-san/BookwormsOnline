using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        // Store encrypted credit card number (encrypted before saving)
        [Required]
        public string EncryptedCreditCardNumber { get; set; }

        [Required, Phone]
        public string MobileNo { get; set; }

        [Required, MaxLength(200)]
        public string BillingAddress { get; set; }

        [Required]
        public string ShippingAddress { get; set; } // Allow special characters

        // Photo file name or path (JPG only, enforced on upload)
        public string PhotoPath { get; set; }
    }
}
