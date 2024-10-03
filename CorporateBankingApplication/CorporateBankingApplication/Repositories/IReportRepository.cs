using CorporateBankingApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApplication.Repositories
{
    public interface IReportRepository
    {
        List<EmployeeSalaryDisbursementDTO> GetSalaryDisbursements();
        List<EmployeeSalaryDisbursementDTO> GetSalaryDisbursementsOfClient(Guid id);
        void AddReportInfo(string role, Guid userId);

        List<PaymentDTO> GetPayments();
        List<PaymentDTO> GetPaymentsOfClient(Guid id);

        void AddPaymentReportInfo(string role, Guid userId);
    }
}
