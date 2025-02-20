﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApplication.Mappings
{
    public class PaymentMap:ClassMap<Payment>
    {
        public PaymentMap()
        {
            Table("Payments");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Amount).Not.Nullable();
            Map(x => x.PaymentStatus).CustomType<CorporateStatus>().Not.Nullable();
            Map(x => x.PaymentRequestDate).Not.Nullable();
            Map(x => x.PaymentApprovalDate).Nullable();
            Map(x => x.RazorpayPaymentId);
            Map(x => x.ClientId);
          //  References(x => x.Client).Column("ClientId").Cascade.None().Not.Nullable();


            References(x => x.Beneficiary).Column("BeneficiaryId").Cascade.None().Not.Nullable();
        }
    }
}