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

        public Guid SalaryDisbursementId { get; set; }
       
        public string CompanyName { get; set; } // From Client
        
        public string EmployeeFirstName { get; set; } // From Employee

       
        public string EmployeeLastName { get; set; } // From Employee
        public double Salary { get; set; } // From Employee
        public DateTime DisbursementDate { get; set; } // From SalaryDisbursement

        public CorporateStatus SalaryStatus { get; set; }

        public string ClientName { get; set; }
    }
}