using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class UserMap:ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(u => u.Id).GeneratedBy.GuidComb();
            Map(u => u.UserName).Not.Nullable();
            Map(u => u.Password).Not.Nullable();
            Map(u => u.Email).Not.Nullable();
            HasOne(u=>u.Role).Cascade.None().PropertyRef(r=>r.User).Constrained();
            HasMany(x => x.Reports).Cascade.All().Inverse();
        }
    }
}