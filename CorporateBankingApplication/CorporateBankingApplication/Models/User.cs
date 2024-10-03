using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class User
    {
        public virtual Guid Id { get; set; }

        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public virtual string Email { get; set; }

        public virtual Role Role { get; set; } = new Role();

        public virtual IList<Report> Reports { get; set; } = new List<Report>();
    }
}