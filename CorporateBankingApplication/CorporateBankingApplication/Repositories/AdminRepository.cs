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
            var clientList = _session.Query<Client>().ToList();
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
        //update client onboarding status & add it as a beneficiary
        public void UpdateClient(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var beneficiary = new Beneficiary()
                {
                    BeneficiaryName = client.UserName,
                    AccountNumber = client.AccountNumber,
                    BankIFSC = client.IFSC,
                    BeneficiaryStatus = client.OnBoardingStatus,
                    BeneficiaryType = BeneficiaryType.INBOUND,
                    IsActive = client.IsActive
                };
                _session.Save(beneficiary);
                _session.Update(client);
                transaction.Commit();
            }
        }
        //if client is active inbound beneficiary is active
        public void UpdateClientBeneficiaryStatus(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var existingBeneficiary = _session.Query<Beneficiary>().Where(b => b.BeneficiaryName == client.UserName).FirstOrDefault();
                existingBeneficiary.IsActive = client.IsActive;
                _session.Update(existingBeneficiary);
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
        /****************************VERIFY OUTBOUND BENEFICIARIES*******************************/

        public List<Beneficiary> GetPendingBeneficiaries()
        {
            var beneficiaries = _session.Query<Beneficiary>().FetchMany(c => c.Documents).Where(b => b.BeneficiaryStatus == CorporateStatus.PENDING && b.BeneficiaryType == BeneficiaryType.OUTBOUND).ToList();
            return beneficiaries;
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
        /****************************PROFILE*******************************/

        public Admin GetAdminById(Guid id)
        {
            return _session.Get<Admin>(id);
        }
    }
}