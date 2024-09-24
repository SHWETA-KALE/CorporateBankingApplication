using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class Document
    {
        public virtual Guid Id { get; set; }

        public virtual string DocumentType { get; set; } //beneficiary, payement, transaction

        public virtual string FilePath { get; set; }

        public virtual DateTime UploadDate { get; set; }

        public virtual Client Client { get; set; }

    }
}