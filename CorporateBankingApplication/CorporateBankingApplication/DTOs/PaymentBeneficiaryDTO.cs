using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class PaymentBeneficiaryDTO
    {
        public double Amount { get; set; }
        public List<BeneficiaryDTO> Beneficiaries { get; set; }
    }
}