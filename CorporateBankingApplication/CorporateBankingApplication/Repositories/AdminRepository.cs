using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace CorporateBankingApplication.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ISession _session;

        public AdminRepository(ISession session)
        {
            _session = session;
        }
        public List<Client> GetAllClients()
        {
            var clientList = _session.Query<Client>().Where(cd => cd.IsActive == true).ToList();
            return clientList;
        }

        public void UpdateClientDetails(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var existingClient = _session.Get<Client>(client.Id);
                if (existingClient != null)
                {
                    existingClient.UserName = client.UserName;
                    existingClient.Email = client.Email;
                    existingClient.CompanyName = client.CompanyName;
                    existingClient.ContactInformation = client.ContactInformation;
                    existingClient.Location = client.Location;
                }
                _session.Update(existingClient);
                transaction.Commit();
            }
        }
        public void DeleteClientDetails(Guid id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var existingClient = _session.Get<Client>(id);
                if (existingClient != null)
                {
                    existingClient.IsActive = false;
                }
                _session.Update(existingClient);
                transaction.Commit();
            }
        }

        ////////////

        public List<Client> GetPendingClients()
        {
            var clients = _session.Query<Client>().FetchMany(c => c.Documents).Where(c => c.OnBoardingStatus == CorporateStatus.PENDING).ToList();
            return clients;
        }

        public Client GetClientById(Guid id)
        {
            return _session.Get<Client>(id);
        }

        public void UpdateClient(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(client);
                transaction.Commit();
            }
        }

        //***********Salary Disbursement***********

        public IEnumerable<EmployeeSalaryDisbursementDTO> GetSalaryDisbursementsByStatus(CorporateStatus status)
        {
            return _session.Query<SalaryDisbursement>()
                           .Where(x => x.SalaryStatus == status)
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

        public bool ApproveSalaryDisbursement(Guid salaryDisbursementId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var salaryDisbursement = _session.Get<SalaryDisbursement>(salaryDisbursementId);
                    if (salaryDisbursement == null || salaryDisbursement.SalaryStatus != CorporateStatus.PENDING)
                        return false;

                    var employee = salaryDisbursement.Employee;
                    var client = employee.Client;

                    double totalSalary = employee.Salary;

                    if (client.Balance < totalSalary)
                    {
                        return false;
                    }

                    client.Balance -= totalSalary;
                    salaryDisbursement.SalaryStatus = CorporateStatus.APPROVED;

                    _session.Update(client);
                    _session.Update(salaryDisbursement);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        public bool RejectSalaryDisbursement(Guid salaryDisbursementId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var salaryDisbursement = _session.Get<SalaryDisbursement>(salaryDisbursementId);
                    if (salaryDisbursement == null || salaryDisbursement.SalaryStatus != CorporateStatus.PENDING)
                        return false;

                    // Set the status to REJECTED
                    salaryDisbursement.SalaryStatus = CorporateStatus.REJECTED;

                    _session.Update(salaryDisbursement);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    
                    transaction.Rollback();
                    throw;
                }
            }
        }


    }
}