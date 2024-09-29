using CorporateBankingApplication.Enum;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class SalaryDisbursement
    {
        public virtual Guid Id { get; set; }

        //public virtual Client Client { get; set; }

        public virtual Employee Employee { get; set; }  

        //public virtual double Salary {  get; set; }

        public virtual DateTime DisbursementDate { get; set; }

        public virtual bool IsBatch {  get; set; }
        public virtual CorporateStatus SalaryStatus { get; set; } 
    }
}