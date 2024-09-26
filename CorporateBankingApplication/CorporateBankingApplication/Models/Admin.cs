using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class Admin:User
    {
        //public virtual Guid Id { get; set; }
        public virtual string BankName { get; set; }
    }
}