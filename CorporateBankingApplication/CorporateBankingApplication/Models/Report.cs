using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class Report
    {
        public virtual Guid Id { get; set; }

        public virtual DateTime GeneratedDate { get; set; }   

        public virtual string ReportType { get; set; }  // PaymentReport, SalaryReport

        public virtual string GeneratedBy { get; set; }

        //public virtual Client Client { get; set; }
        public virtual User User { get; set; }
        //change this to user

        ///filepath for the report 

    }
}