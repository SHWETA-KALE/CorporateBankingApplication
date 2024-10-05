using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Xml.Linq;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System.IO;

namespace CorporateBankingApplication.Services
{
    public class EmailService : IEmailService
    {
        public void SendSalaryDisbursementApprovalEmail(string clientEmail, EmployeeDTO employee, double salaryAmount, string month)
        {
            var subject = "Salary Disbursement Request Approved";
            var body = $@"
                Dear Client,<br/><br/>
                We are pleased to inform you that the salary disbursement request for your employee <strong>{employee.FirstName} {employee.LastName}</strong> 
                for the month of <strong>{month}</strong> amounting to <strong>{salaryAmount:C}</strong> has been approved successfully.<br/><br/>
                Thank you,<br/>
                Corporate Banking Application Team";

            SendClientOnboardingStatusEmail(clientEmail, subject, body);
        }

        //batch
        public void SendBatchSalaryDisbursementApprovalEmail(string clientEmail, List<EmployeeDTO> employeeSalaries, string month)
        {
            var subject = "Batch Salary Disbursement Request Approved";

            var body = "Dear Client,<br/><br/>";
            body += $"We are pleased to inform you that the salary disbursement requests for the following employees for the month of <strong>{month}</strong> have been approved successfully:<br/><br/>";
            body += "<ul>";

            foreach (var employee in employeeSalaries)
            {
                body += $"<li>Employee: <strong>{employee.FirstName} {employee.LastName}</strong> - Salary Amount: <strong>{employee.Salary:C}</strong></li>";
            }
            body += "</ul><br/>";
            body += "Thank you,<br/>Corporate Banking Application Team";

            SendClientOnboardingStatusEmail(clientEmail, subject, body);
        }
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

        //Payslip
        public void SendPayslipToEmployee(string employeeEmail, byte[] payslipPdf)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("it.a.47.shweta.kale@gmail.com"),
                Subject = "Your Payslip for " + DateTime.Now.ToString("MMMM yyyy"),
                Body = "Dear Employee, please find your payslip attached.",
                IsBodyHtml = true
            };
            mailMessage.To.Add(employeeEmail);

            //Attach pdf
            mailMessage.Attachments.Add(new Attachment(new MemoryStream(payslipPdf), "Payslip.pdf", "application/pdf"));
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                //Credentials = new NetworkCredential("it.a.47.shweta.kale@gmail.com", "IIpr sdnt Inxk xdto"),
                Credentials = new NetworkCredential("it.b.32.roshani.poojari@gmail.com", "zobr qnbb fqlz lezb"),
                EnableSsl = true
            };
            smtpClient.Send(mailMessage);
        }

    }

}