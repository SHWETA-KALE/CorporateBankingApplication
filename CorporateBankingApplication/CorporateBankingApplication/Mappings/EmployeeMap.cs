using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employees");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.FirstName).Not.Nullable();
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.Email).Not.Nullable();
            Map(x => x.Position).Not.Nullable();
            Map(x => x.Phone).Not.Nullable();
            Map(x=> x.Salary).Not.Nullable();
            Map(x => x.IsActive).Not.Nullable();
            HasMany(x => x.SalaryDisbursements).Cascade.All().Inverse();
            References(x => x.Client).Column("ClientId").Cascade.None().Not.Nullable();
        }
    }
}