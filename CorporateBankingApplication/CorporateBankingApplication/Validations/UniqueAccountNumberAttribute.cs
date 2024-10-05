using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Validations
{
    public class UniqueAccountNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var accountNumber = value as string;

            var clientDto = (ClientDTO)validationContext.ObjectInstance;

            if (clientDto == null || string.IsNullOrEmpty(accountNumber))
            {
                return ValidationResult.Success; // No validation needed if no ClientDTO or username
            }

            using (var session = NHibernateHelper.CreateSession())
            {
                if (session == null)
                {
                    throw new NullReferenceException("Failed to create a session.");
                }
                var existingClient = session.Query<Client>()
                    .Any(c => c.AccountNumber == accountNumber && c.Id != clientDto.Id);
                if (existingClient)
                {
                    return new ValidationResult("Account number must be unique.");
                }

            }
            return ValidationResult.Success;
        }
    }
}