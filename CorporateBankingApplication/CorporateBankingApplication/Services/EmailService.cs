using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public class EmailService : IEmailService
    {
        public void SendClientOnboardingStatusEmail(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("it.b.32.roshani.poojari@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("it.b.32.roshani.poojari@gmail.com", "zobr qnbb fqlz lezb"),
                EnableSsl = true,
            };
            smtpClient.Send(mailMessage);
        }
    }
}