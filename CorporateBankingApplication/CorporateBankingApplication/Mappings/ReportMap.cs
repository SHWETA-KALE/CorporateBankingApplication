using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class ReportMap:ClassMap<Report>
    {
        public ReportMap()
        {
            Table("Reports");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.GeneratedDate).Not.Nullable();
            Map(x => x.ReportType).Not.Nullable();
            Map(x => x.GeneratedBy).Not.Nullable();
            References(x => x.User).Column("UserId").Cascade.None().Not.Nullable();
        }
    }
}