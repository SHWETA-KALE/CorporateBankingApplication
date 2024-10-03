using CorporateBankingApplication.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Exceptions
{
    public class EnsureBeneficiaries: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var beneficiaries = value as List<BeneficiaryDTO>;
            if (beneficiaries == null || !beneficiaries.Any())
            {
                return new ValidationResult(ErrorMessage ?? "The Beneficiaries list must not be empty.");
            }

            return ValidationResult.Success;
        }

    }
}