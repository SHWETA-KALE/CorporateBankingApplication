using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApplication.Services
{
    public interface IEmailService
    {
        void SendClientOnboardingStatusEmail(string toEmail, string subject, string body);
    }
}
