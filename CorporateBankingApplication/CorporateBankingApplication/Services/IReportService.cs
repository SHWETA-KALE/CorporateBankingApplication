using CorporateBankingApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public interface IReportService
    {
        List<EmployeeSalaryDisbursementDTO> GetSalaryDisbursements(string role, Guid id);
        void AddReportInfo(string role, Guid userId);
        List<PaymentDTO> GetPayments(string role, Guid id);
        void AddPaymentReportInfo(string role, Guid userId);
    }
}