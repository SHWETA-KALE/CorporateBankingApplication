using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Services
{
    public interface IClientService
    {
        List<Employee> GetAllEmployees(Guid clientId);
        void AddEmployee(EmployeeDTO employeeDto, Client client);
        Employee GetEmployeeById(Guid id);

        void UpdateEmployee(EmployeeDTO employeeDto, Client client);

        void UpdateEmployeeStatus(Guid id, bool isActive);

        Client GetClientById(Guid clientId);

        //re-editing client after rejection
        void EditClientRegistrationDetail(Client client, IList<HttpPostedFileBase> uploadedFiles);

        //******Salary disbursement ***********
        bool DisburseSalary(List<Guid> employeeIds, bool isBatch, out List<Guid> skippedEmployees);

        /*benenficiaries*/
        List<BeneficiaryDTO> GetAllOutboundBeneficiaries(Guid clientId, UrlHelper urlHelper);

        void UpdateBeneficiaryStatus(Guid id, bool isActive);
        void AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles);
        Beneficiary GetBeneficiaryById(Guid id);
        void UpdateBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles);

        //payments
        List<BeneficiaryDTO> GetBeneficiaryList(Guid id);

        //balance update
        void AddBalance(Guid id, double amount);


    }
}