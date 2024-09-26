using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class Employee
    {
        public virtual Guid Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public virtual string Position { get; set; }
        public virtual long Phone { get; set; }

        public virtual bool IsActive { get; set; } 

        public virtual IList<SalaryDisbursement> SalaryDisbursements { get; set; } = new List<SalaryDisbursement>();

        public virtual Client Client { get; set; }





    }
}