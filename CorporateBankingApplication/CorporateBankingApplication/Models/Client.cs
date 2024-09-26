using System;
using System.Collections.Generic;
using CorporateBankingApplication.Enum;

namespace CorporateBankingApplication.Models
{
    public class Client:User
    {
        //public virtual Guid Id {  get; set; }
        public virtual string CompanyName {  get; set; }
        public virtual string ContactInformation { get; set; }
        public virtual string Location { get; set; }

        public virtual double Balance { get; set; }
        public virtual IList<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();
        public virtual IList<Document> Documents { get; set; } = new List<Document>();

        public virtual IList<Report> Reports { get; set; } = new List<Report>();

        public virtual IList<Employee> Employees { get; set; } = new List<Employee>();

        public virtual Status OnBoardingStatus { get; set; }


    }
}