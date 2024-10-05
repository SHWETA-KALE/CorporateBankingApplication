using CorporateBankingApplication.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class ClientDTO
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email Id")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "Contact Information")]
        public string ContactInformation { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public double Balance { get; set; }
        [Required]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Account number must be exactly 12 digits.")]
        public string AccountNumber { get; set; }
        [Required]
        public string IFSC { get; set; }
        [Required]
        public CorporateStatus OnboardingStatus { get; set; }

       
        public HttpPostedFileBase Document1 { get; set; }
       
        public HttpPostedFileBase Document2 { get; set; }

        
        public List<string> DocumentPaths { get; set; } = new List<string>();

        public List<DocumentDTO> Documents { get; set; }

        //new addition
        public string BeneficiaryStatus { get; set; }

        [Required]
        [Display(Name = "Previous Password")]
        public string PreviousPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
        [Display(Name = "New Password")]

        public string NewPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}