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
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Email Id")]
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
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Required]
        public string IFSC { get; set; }
        [Required]
        public double Balance { get; set; }
        [Required]
        public HttpPostedFileBase Document1 { get; set; }
        [Required]
        public HttpPostedFileBase Document2 { get; set; }

        [Required]//added for showing document
        public List<string> DocumentPaths { get; set; } = new List<string>();

        //for editing of registrationdetails
        public List<DocumentDTO> Documents { get; set; }
    }
}