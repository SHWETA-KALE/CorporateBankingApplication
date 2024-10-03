using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CorporateBankingApplication.Enum;

namespace CorporateBankingApplication.Models
{
    public class Client:User
    {
        [Required]
        public virtual string CompanyName {  get; set; }

        [Required]
        public virtual string ContactInformation { get; set; }

        [Required]
        public virtual string Location { get; set; }

        [Required]
        public virtual double Balance { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        [Required]
        public virtual string AccountNumber { get; set; }

        [Required]
        public virtual string IFSC { get; set; }

        public virtual IList<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();
        public virtual IList<Document> Documents { get; set; } = new List<Document>();

        //public virtual IList<Report> Reports { get; set; } = new List<Report>();

        public virtual IList<Employee> Employees { get; set; } = new List<Employee>();

        public virtual CorporateStatus OnBoardingStatus { get; set; }


    }
}