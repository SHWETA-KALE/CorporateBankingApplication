using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Services
{
    public interface IAdminService
    {
        List<Client> ViewAllClients();
        void EditClient(ClientDTO clientDTO, Guid id);
        void RemoveClient(Guid id);

        List<ClientDTO> GetClientsForVerification(UrlHelper urlHelper);

        bool UpdateClientOnboardingStatus(Guid id, string status);


        //SALARY DISBURSEMENTS
        IEnumerable<EmployeeSalaryDisbursementDTO> GetPendingSalaryDisbursements();


        bool ApproveSalaryDisbursement(Guid salaryDisbursementId);
        bool RejectSalaryDisbursement(Guid salaryDisbursementId);
    }
}