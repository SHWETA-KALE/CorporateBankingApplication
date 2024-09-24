using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class BeneficiaryMap:ClassMap<Beneficiary>
    {
        public BeneficiaryMap()
        {
            Table("Beneficiaries");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.BeneficiaryName).Not.Nullable();
            Map(x => x.AccountNumber).Not.Nullable();
            Map(x => x.BankIFSC).Not.Nullable();
            Map(x => x.BeneficiaryType).CustomType<BeneficiaryType>().Not.Nullable();
            References(x => x.Client).Column("ClientId").Cascade.None().Nullable();
            HasMany(x => x.Payments).Cascade.All().Inverse();  
        }
    }
}