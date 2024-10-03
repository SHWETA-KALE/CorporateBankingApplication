using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class BeneficiaryDTO
    {
        public Guid Id { get; set; }
        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryName { get; set; }
        [Display(Name = "Account Number")]

        public string AccountNumber { get; set; }
        [Display(Name = "IFSC")]

        public string BankIFSC { get; set; }
        [Display(Name = "Type")]

        public string BeneficiaryType { get; set; }
        public bool IsActive { get; set; }
        public string BeneficiaryStatus { get; set; }

        
        [Display(Name = "Beneficiary Id Proof")]

        public HttpPostedFileBase BeneficiaryIdProof { get; set; }
        
        [Display(Name = "Beneficiary Address Proof")]

        public HttpPostedFileBase BeneficiaryAddressProof { get; set; }

        public List<string> DocumentUrls { get; set; }
    }
}