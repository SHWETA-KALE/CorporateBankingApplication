using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class User
    {
        public virtual Guid Id { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual Role Role { get; set; } = new Role();

        //public virtual DateTime LoginDate { get; set; } => analytics



    }
}