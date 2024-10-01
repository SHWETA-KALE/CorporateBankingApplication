using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class DocumentMap:ClassMap<Document>
    {
        public DocumentMap()
        {
            Table("Documents");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.DocumentType).Not.Nullable();
            Map(x => x.FilePath).Not.Nullable();
            Map(x => x.UploadDate).Not.Nullable();
            //References(x => x.Client).Column("ClientId").Cascade.None().Not.Nullable();
            References(x => x.Client).Column("ClientId").Nullable();
            References(x => x.Beneficiary).Column("BeneficiaryId").Nullable();
        }
    }
}