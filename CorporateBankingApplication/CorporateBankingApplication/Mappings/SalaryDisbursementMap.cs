using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class SalaryDisbursementMap:ClassMap<SalaryDisbursement>
    {
        public SalaryDisbursementMap()
        {
            Table("SalaryDisbursements");
            Id(x => x.Id).GeneratedBy.GuidComb();
            //Map(x => x.Salary).Not.Nullable();
            Map(x => x.DisbursementDate).Not.Nullable();
            Map(x => x.IsBatch).Not.Nullable();
            Map(x => x.SalaryStatus).CustomType<CorporateStatus>().Not.Nullable();
            References(x => x.Employee).Column("EmployeeId").Cascade.None().Not.Nullable();
        }
    }
}