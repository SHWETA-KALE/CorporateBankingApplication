using System;
using System.Collections.Generic;
using CorporateBankingApplication.Enum;

namespace CorporateBankingApplication.Models
{
    public class Client:User
    {
        public virtual string CompanyName {  get; set; }
        public virtual string ContactInformation { get; set; }
        public virtual string Location { get; set; }

        public virtual double Balance { get; set; }
        public virtual IList<Beneficiary> Beneficiaries { get; set; } 
        public virtual IList<Document> Documents { get; set; }

        public virtual IList<Report> Reports { get; set; }

        public virtual IList<Employee> Employees { get; set; }

        public virtual Status OnBoardingStatus { get; set; }


    }
}