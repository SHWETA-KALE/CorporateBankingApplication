using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class AdminMap:SubclassMap<Admin>
    {
        public AdminMap()
        {
            Table("Admins");
            KeyColumn("UserId");
            Map(x => x.BankName).Not.Nullable();
        }
    }
}