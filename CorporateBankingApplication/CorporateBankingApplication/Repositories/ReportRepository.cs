using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ISession _session;

        public ReportRepository(ISession session)
        {
            _session = session;
        }
        public List<EmployeeSalaryDisbursementDTO> GetSalaryDisbursements()
        {
            return _session.Query<SalaryDisbursement>()
                           .Select(x => new EmployeeSalaryDisbursementDTO
                           {
                               SalaryDisbursementId = x.Id,
                               CompanyName = x.Employee.Client.CompanyName,
                               EmployeeFirstName = x.Employee.FirstName,
                               EmployeeLastName = x.Employee.LastName,
                               Salary = x.Employee.Salary,
                               DisbursementDate = x.DisbursementDate,
                               SalaryStatus = x.SalaryStatus
                           })
                           .ToList();
        }
        public List<EmployeeSalaryDisbursementDTO> GetSalaryDisbursementsOfClient(Guid id)
        {
            return _session.Query<SalaryDisbursement>().Where(x => x.Employee.Client.Id == id)
                           .Select(x => new EmployeeSalaryDisbursementDTO
                           {
                               SalaryDisbursementId = x.Id,
                               CompanyName = x.Employee.Client.CompanyName,
                               EmployeeFirstName = x.Employee.FirstName,
                               EmployeeLastName = x.Employee.LastName,
                               Salary = x.Employee.Salary,
                               DisbursementDate = x.DisbursementDate,
                               SalaryStatus = x.SalaryStatus
                           })
                           .ToList();
        }

        public void AddReportInfo(string role, Guid userId)
        {
            var user = _session.Get<User>(userId);
            var report = new Report
            {
                Id = Guid.NewGuid(),
                GeneratedDate = DateTime.Now,
                ReportType = "Salary Disbursement",
                GeneratedBy = role,
                User = user
            };
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(report);
                transaction.Commit();
            }
        }

        public List<PaymentDTO> GetPayments()
        {

            return _session.Query<Payment>()
                           .Select(x => new PaymentDTO
                           {
                               PaymentId = x.Id,
                               CompanyName = _session.Get<Client>(x.ClientId).CompanyName,
                               AccountNumber = _session.Get<Client>(x.ClientId).UserName,
                               BeneficiaryName = x.Beneficiary.BeneficiaryName,
                               Amount = x.Amount,
                               PaymentRequestDate = x.PaymentRequestDate,
                               PaymentStatus = x.PaymentStatus
                           })
                           .ToList();
        }
        public List<PaymentDTO> GetPaymentsOfClient(Guid id)
        {
            var client = _session.Get<Client>(id);
            return _session.Query<Payment>().Where(x => x.ClientId== id)
                           .Select(x => new PaymentDTO
                           {
                               PaymentId = x.Id,
                               CompanyName = _session.Get<Client>(x.ClientId).CompanyName,
                               AccountNumber = _session.Get<Client>(x.ClientId).UserName,
                               BeneficiaryName = x.Beneficiary.BeneficiaryName,
                               Amount = x.Amount,
                               PaymentRequestDate = x.PaymentRequestDate,
                               PaymentStatus = x.PaymentStatus,                
                           })
                           .ToList();
        }
        public void AddPaymentReportInfo(string role, Guid userId)
        {
            var user = _session.Get<User>(userId);
            var report = new Report
            {
                Id = Guid.NewGuid(),
                GeneratedDate = DateTime.Now,
                ReportType = "Payment",
                GeneratedBy = role,
                User = user
            };
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(report);
                transaction.Commit();
            }
        }

    }
}