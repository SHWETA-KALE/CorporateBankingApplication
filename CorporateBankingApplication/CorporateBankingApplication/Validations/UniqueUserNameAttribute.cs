using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CorporateBankingApplication.Validations
{
    public class UniqueUserNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var username = value as string;

            // Get the current ClientDTO (which includes the Id of the user being edited)
            var clientDto = validationContext.ObjectInstance as ClientDTO;
            if (clientDto == null || string.IsNullOrEmpty(username))
            {
                return ValidationResult.Success; // No validation needed if no ClientDTO or username
            }

            using (var session = NHibernateHelper.CreateSession())
            {
                if (session == null)
                {
                    throw new NullReferenceException("Failed to create a session.");
                }

                // Check if there is another user with the same username, but a different Id
                var userExists = session.Query<User>()
                    .Any(u => u.UserName.ToLower() == username.ToLower() && u.Id != clientDto.Id);

                if (userExists)
                {
                    return new ValidationResult("Username must be unique.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
