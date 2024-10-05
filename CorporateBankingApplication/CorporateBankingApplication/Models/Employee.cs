using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class Employee
    {
        public virtual Guid Id { get; set; }

        [Required]
        public virtual string FirstName { get; set; }

        [Required]
        public virtual string LastName { get; set; }

        [Required]
        public virtual string Email { get; set; }

        [Required]
        public virtual string Position { get; set; }

        [Required]
        public virtual string Phone { get; set; }

        [Required]
        public virtual double Salary { get; set; }

        [Required]
        public virtual bool IsActive { get; set; } 

        public virtual IList<SalaryDisbursement> SalaryDisbursements { get; set; } = new List<SalaryDisbursement>();

        public virtual Client Client { get; set; }

      




    }
}