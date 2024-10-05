using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ISession _session;

        public ClientRepository(ISession session)
        {
            _session = session;
        }
        public void AddEmployee(Employee employee)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(employee);
                transaction.Commit();
            }

        }

        public List<Employee> GetAllEmployees(Guid clientId)
        {
            var client = _session.Query<Client>().FetchMany(c => c.Employees).SingleOrDefault(c => c.Id == clientId);
            return client?.Employees.ToList() ?? new List<Employee>();
        }

        public Employee GetEmployeeById(Guid id)
        {
            return _session.Get<Employee>(id);
        }

        public Client GetClientById(Guid clientId)
        {
            return _session.Get<Client>(clientId);
        }

        public void UpdateEmployee(Employee employee)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(employee);
                transaction.Commit();
            }

        }

        public void UpdateEmployeeStatus(Guid id, bool isActive)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var employee = _session.Get<Employee>(id);
                if (employee != null)
                {
                    employee.IsActive = isActive;
                    _session.Update(employee);
                    transaction.Commit();
                }
            }

        }


        /************************************Re-editing of details on rejection***************************************/
        public void UpdateClientRegistrationDetails(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                //if (!string.IsNullOrEmpty(client.Password))
                //{
                //    client.Password = PasswordHelper.HashPassword(client.Password);
                //}

                _session.Update(client);
                transaction.Commit();
            }
        }

        //******************SALARY DISBURSEMENTS *********************

        public List<Employee> GetEmployeesByIds(List<Guid> employeeIds)
        {
            return _session.Query<Employee>().Where(e => employeeIds.Contains(e.Id)).ToList();
        }

        public void AddSalaryDisbursement(SalaryDisbursement salaryDisbursement)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(salaryDisbursement);
                transaction.Commit();
            }
        }

        public SalaryDisbursement GetSalaryDisbursementForEmployee(Guid employeeId, DateTime currentDate)
        {
            // Get the first day of the month of the current date
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            // Get the first day of the next month
            var endOfMonth = startOfMonth.AddMonths(1);

            return _session.Query<SalaryDisbursement>().FirstOrDefault(sd => sd.Employee.Id == employeeId && sd.DisbursementDate >= startOfMonth && sd.DisbursementDate < endOfMonth);
        }


        /*beneficiaries*/
        public List<Beneficiary> GetAllOutboundBeneficiaries(Guid clientId)
        {
            var beneficiaries = _session.Query<Beneficiary>().Where(b => b.Client.Id == clientId).OrderBy(b => b.BeneficiaryType).ToList();
            return beneficiaries;
        }
        //new addition
        public List<Client> GetAllInboundBeneficiaries(Guid clientId)
        {
            var existingBeneficiaries = _session.Query<Beneficiary>()
                .Select(b => b.BeneficiaryName) // Get only the BeneficiaryName for comparison
                .ToList();

            // Fetch all clients that are approved and not the current client
            var existingClients = _session.Query<Client>()
                .Where(c => c.Id != clientId && c.OnBoardingStatus == CorporateStatus.APPROVED)
                .ToList();

            // Filter clients who are not in the beneficiary list
            var clientsNotInBeneficiaries = existingClients
                .Where(c => !existingBeneficiaries.Contains(c.UserName))
                .ToList();

            return clientsNotInBeneficiaries;
        }

        public void UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var beneficiary = _session.Get<Beneficiary>(id);
                if (beneficiary != null)
                {
                    beneficiary.IsActive = isActive;
                    _session.Update(beneficiary);
                    transaction.Commit();
                }
            }

        }

        public void AddNewBeneficiary(Beneficiary beneficiary)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(beneficiary);
                transaction.Commit();
            }

        }
        public Beneficiary GetBeneficiaryById(Guid id)
        {
            return _session.Get<Beneficiary>(id);
        }
        public void UpdateBeneficiary(Beneficiary beneficiary)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(beneficiary);
                transaction.Commit();
            }

        }

        /****************************AddBalance**************************************/
        public void AddBalance(Guid id, double amount)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var client = GetClientById(id);
                client.Balance += amount;
                _session.Update(client);
                transaction.Commit();
            }
        }
        public void SaveNewPassword(Guid id, string password)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var client = _session.Get<Client>(id);
                client.Password = PasswordHelper.HashPassword(password);
                _session.Update(client);
                transaction.Commit();
            }
        }

        /*******************************PAYMENTS*********************************/
        public List<Beneficiary> GetBeneficiaryList(Guid clientId)
        {
            var client = GetClientById(clientId);
            var beneficiaries = _session.Query<Beneficiary>().Where(b => b.Client.Id == clientId && b.BeneficiaryStatus == CorporateStatus.APPROVED && b.IsActive == true).ToList();
            return beneficiaries;
        }


        //REPORTS
        public List<EmployeeSalaryDisbursementDTO> GetAllSalaryDisbursements(Guid id)
        {
            return _session.Query<SalaryDisbursement>().Where(x => x.Employee.Client.Id == id)
                           .OrderBy(s => s.DisbursementDate)
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
        public List<PaymentDTO> GetPayments(Guid id)
        {

            return _session.Query<Payment>().Where(x => x.ClientId == id)
                           .OrderBy(p => p.PaymentRequestDate)
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
        public void AddReportInfo(Guid id)
        {
            var user = _session.Get<User>(id);
            var report = new Report
            {
                Id = Guid.NewGuid(),
                GeneratedDate = DateTime.Now,
                ReportType = "Salary Disbursement",
                GeneratedBy = "Admin",
                User = user
            };
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(report);
                transaction.Commit();
            }
        }
        public void AddPaymentReportInfo(Guid id)
        {
            var user = _session.Get<User>(id);
            var report = new Report
            {
                Id = Guid.NewGuid(),
                GeneratedDate = DateTime.Now,
                ReportType = "Payment",
                GeneratedBy = "Client",
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