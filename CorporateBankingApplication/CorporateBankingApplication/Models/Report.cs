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

        public virtual string ReportType { get; set; }  // PaymentReport, SalaryReport, TransactionReport

        public virtual string GeneratedBy { get; set; }

        public virtual User User { get; set; }  

        //public virtual Client Client { get; set; }

        ///filepath for the report 

    }
}