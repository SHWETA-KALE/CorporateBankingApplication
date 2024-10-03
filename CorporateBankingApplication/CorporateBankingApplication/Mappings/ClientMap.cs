using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class ClientMap:SubclassMap<Client>
    {
        public ClientMap()
        {
            Table("Clients");
            KeyColumn("UserId");
            Map(x => x.CompanyName).Not.Nullable();
            Map(x => x.ContactInformation).Nullable();
            Map(x => x.Location).Nullable();
            Map(x => x.Balance).Not.Nullable();
            Map(x => x.IsActive).Not.Nullable();
            Map(x=>x.AccountNumber).Not.Nullable();
            Map(x=>x.IFSC).Not.Nullable();
            Map(x => x.OnBoardingStatus).CustomType<CorporateStatus>().Not.Nullable(); //enum
            HasMany(x => x.Beneficiaries).Cascade.All().Inverse();
            HasMany(x => x.Documents).Cascade.All().Inverse();
            //HasMany(x => x.Reports).Cascade.All().Inverse();
            HasMany(x => x.Employees).Cascade.All().Inverse();
        }
    }
}