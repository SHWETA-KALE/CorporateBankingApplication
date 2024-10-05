using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.DTOs
{
    public class AnalyticsDTO
    {
        public int ClientsOnboardedToday { get; set; }
        public int PaymentTransactions { get; set; }
        public int SalaryDisbursements { get; set; }
        public Dictionary<string, int> MostAccessedFeatures { get; set; }
        public Dictionary<string, int> UserEngagement { get; set; }
    }

}