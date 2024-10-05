using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class DocumentDTO
    {

        [Required]
        
        public string DocumentType { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }
    }
}