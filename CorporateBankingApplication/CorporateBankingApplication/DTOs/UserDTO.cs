using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Username field is required.")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
       // [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

    }
}