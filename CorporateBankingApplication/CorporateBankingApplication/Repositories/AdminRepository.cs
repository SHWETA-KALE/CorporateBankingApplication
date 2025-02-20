﻿using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using NHibernate;
using NHibernate.Linq;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Payment = CorporateBankingApplication.Models.Payment;

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
            var clients = _session.Query<Client>().FetchMany(c => c.Documents).Where(c => c.OnBoardingStatus == CorporateStatus.PENDING && c.IsActive == true).ToList();
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
                           .OrderByDescending(x => x.DisbursementDate)
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

        public SalaryDisbursement GetSalaryDisbursementById(Guid id)
        {
            return _session.Query<SalaryDisbursement>()
                           .Fetch(sd => sd.Employee) // Eager load the Employee
                           .ThenFetch(e => e.Client) // Eager load the Client
                           .FirstOrDefault(sd => sd.Id == id);
        }



        /**************************VERIFY OUTBOUND BENEFICIARIES*****************************/

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

        /**************************PROFILE*****************************/

        public Admin GetAdminById(Guid id)
        {
            return _session.Get<Admin>(id);
        }

        /***********************Verification of Payments******************/

        public IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CorporateStatus status)
        {

            return _session.Query<Payment>()
                .Where(x => x.PaymentStatus == status)
                .OrderByDescending(x => x.PaymentRequestDate)
                .Select(x => new PaymentDTO
                {
                    PaymentId = x.Id,
                    CompanyName = x.Beneficiary.BeneficiaryName,
                    AccountNumber = x.Beneficiary.AccountNumber,
                    // BeneficiaryType = x.Beneficiary.BeneficiaryType.ToString().ToUpper(),
                    BeneficiaryType = x.Beneficiary.BeneficiaryType,
                    Amount = x.Amount,
                    PaymentRequestDate = x.PaymentRequestDate
                })
                .ToList();
        }



        public void UpdatePaymentStatus(Guid paymentId, CorporateStatus status)
        {
            var payment = _session.Get<Payment>(paymentId);



            if (payment != null)
            {
                payment.PaymentStatus = status;
                if (status == CorporateStatus.APPROVED)
                {
                    payment.PaymentApprovalDate = DateTime.Now;
                    var client = _session.Get<Client>(payment.ClientId);

                    if (client != null)
                    {
                        client.Balance -= payment.Amount;
                        _session.Update(client);
                    }
                }
                _session.Update(payment);
                _session.Flush(); // Save changes to the database
            }
        }

        public Payment GetPaymentById(Guid id)
        {
            return _session.Query<Payment>().FirstOrDefault(p => p.Id == id);
        }

        public void UpdatePayment(Payment payment)
        {
            using (ITransaction transaction = _session.BeginTransaction())
            {
                _session.Update(payment);
                transaction.Commit();
            }
        }


        //REPORTS
        public List<EmployeeSalaryDisbursementDTO> GetAllSalaryDisbursements(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _session.Query<SalaryDisbursement>().AsQueryable();

            // Apply filters if any
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                // Filter by company name
                query = query.Where(s => s.Employee.Client.UserName.Contains(companyName));
            }

            if (startDate.HasValue)
            {
                // Include records from the start date
                query = query.Where(s => s.DisbursementDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Include records up to and including the end date
                query = query.Where(s => s.DisbursementDate < endDate.Value.AddDays(1));
            }

            return query.OrderBy(s => s.DisbursementDate)
                         .Select(x => new EmployeeSalaryDisbursementDTO
                         {
                             SalaryDisbursementId = x.Id,
                             ClientName = x.Employee.Client.UserName,
                             EmployeeFirstName = x.Employee.FirstName,
                             EmployeeLastName = x.Employee.LastName,
                             Salary = x.Employee.Salary,
                             DisbursementDate = x.DisbursementDate,
                             SalaryStatus = x.SalaryStatus
                         })
                         .ToList();
        }
        public List<PaymentDTO> GetPayments(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Start the query on the Payment table
            var query = _session.Query<Payment>().AsQueryable();

            // Join with Client to filter by company name
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                query = query.Where(p => _session.Query<Client>().Where(c => c.UserName.Contains(companyName)).Select(c => c.Id).Contains(p.ClientId));
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.PaymentRequestDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.PaymentRequestDate < endDate.Value.AddDays(1)); // Include the end date
            }

            // Now project to DTO
            return query.OrderBy(p => p.PaymentRequestDate)
                        .Select(x => new PaymentDTO
                        {
                            PaymentId = x.Id,
                            ClientName = _session.Query<Client>().Where(c => c.Id == x.ClientId).Select(c => c.UserName).FirstOrDefault(),
                            AccountNumber = _session.Query<Client>().Where(c => c.Id == x.ClientId).Select(c => c.AccountNumber).FirstOrDefault(),
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
                GeneratedBy = "Admin",
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