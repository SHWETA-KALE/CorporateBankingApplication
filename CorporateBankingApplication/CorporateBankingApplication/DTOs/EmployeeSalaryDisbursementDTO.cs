using CorporateBankingApplication.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class EmployeeSalaryDisbursementDTO
    {

        [Display(Name = "Id")]
        public Guid SalaryDisbursementId { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } // From Client

        [Display(Name = "First Name")]
        public string EmployeeFirstName { get; set; } // From Employee


        [Display(Name = "Last Name")]
        public string EmployeeLastName { get; set; } // From Employee
        public double Salary { get; set; } // From Employee

        [Display(Name = "Disbursement Date")]
        public DateTime DisbursementDate { get; set; } // From SalaryDisbursement

        [Display(Name = "Status")]
        public CorporateStatus SalaryStatus { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }
    }
}