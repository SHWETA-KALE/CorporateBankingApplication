using CorporateBankingApplication.DTOs;
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
        void SendSalaryDisbursementApprovalEmail(string clientEmail, EmployeeDTO employee, double salaryAmount, string month);
        void SendBatchSalaryDisbursementApprovalEmail(string clientEmail, List<EmployeeDTO> employeeSalaries, string month);

        void SendPayslipToEmployee(string employeeEmail, byte[] payslipPdf);



    }
}
