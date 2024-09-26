using CorporateBankingApplication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class ClientDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string ContactInformation { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public IFormFile Document1 { get; set; }
        [Required]
        public IFormFile Document2 { get; set; }
    }
}