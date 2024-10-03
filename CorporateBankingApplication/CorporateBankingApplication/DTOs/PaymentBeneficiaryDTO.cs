using CorporateBankingApplication.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class PaymentBeneficiaryDTO
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }

        [EnsureBeneficiaries(ErrorMessage = "At least one beneficiary must be selected.")]
        public List<BeneficiaryDTO> Beneficiaries { get; set; }
    }
}