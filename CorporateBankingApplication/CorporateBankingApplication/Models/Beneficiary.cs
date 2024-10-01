using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using System.Collections.Generic;
using System;

public class Beneficiary
{
    public virtual Guid Id { get; set; }

    public virtual string BeneficiaryName { get; set; }

    public virtual string AccountNumber { get; set; }

    public virtual string BankIFSC { get; set; }
    public virtual CorporateStatus BeneficiaryStatus { get; set; }
    public virtual BeneficiaryType BeneficiaryType { get; set; } //Inbound Outbound
    public virtual bool IsActive { get; set; }

    public virtual IList<Document> Documents { get; set; } = new List<Document>();
    public virtual IList<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Client Client { get; set; }


}