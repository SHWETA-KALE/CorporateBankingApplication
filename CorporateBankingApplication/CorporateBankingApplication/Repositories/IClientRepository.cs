using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public interface IClientRepository
    {
        List<Employee> GetAllEmployees(Guid clientId);
        void AddEmployee(Employee employee);
        Employee GetEmployeeById(Guid id);

        Client GetClientById(Guid clientId);
        void UpdateEmployee(Employee employee);

        void UpdateEmployeeStatus(Guid id, bool isActive);

        //re-editing client after rejection
        void UpdateClientRegistrationDetails(Client client);

        //****************SALARY DISBURSEMENTS **********************

        List<Employee> GetEmployeesByIds(List<Guid> employeeIds);
        void AddSalaryDisbursement(SalaryDisbursement salaryDisbursement);

        SalaryDisbursement GetSalaryDisbursementForEmployee(Guid employeeId, DateTime currentDate);

        /*benenficiaries*/
        List<Beneficiary> GetAllOutboundBeneficiaries(Guid clientId);
        void UpdateBeneficiaryStatus(Guid id, bool isActive);
        void AddNewBeneficiary(Beneficiary beneficiary);
        Beneficiary GetBeneficiaryById(Guid id);
        void UpdateBeneficiary(Beneficiary beneficiary);

        List<Client> GetAllInboundBeneficiaries(Guid clientId);

        //payments
        List<Beneficiary> GetBeneficiaryList(Guid clientId);

        void AddBalance(Guid id, double amount);

        void SaveNewPassword(Guid id, string password);

        //REPORTS
        List<EmployeeSalaryDisbursementDTO> GetAllSalaryDisbursements(Guid id, DateTime? startDate, DateTime? endDate);

        List<PaymentDTO> GetPayments(Guid id, string beneficiaryName, DateTime? startDate, DateTime? endDate);
        void AddReportInfo(Guid id);

        void AddPaymentReportInfo(Guid id);
    }
}