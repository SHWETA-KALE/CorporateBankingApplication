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
        List<ClientDTO> ViewAllClients();
        void EditClient(ClientDTO clientDTO, Guid id);
        bool ToggleClientActiveStatus(Guid id, bool isActive, out string message);
        List<ClientDTO> GetClientsForVerification(UrlHelper urlHelper);

        bool UpdateClientOnboardingStatus(Guid id, string status);


        //SALARY DISBURSEMENTS
        IEnumerable<EmployeeSalaryDisbursementDTO> GetPendingSalaryDisbursements();


        bool ApproveSalaryDisbursement(Guid salaryDisbursementId);
        bool RejectSalaryDisbursement(Guid salaryDisbursementId);

        /****************************VERIFY OUTBOUND BENEFICIARIES*******************************/
        List<BeneficiaryDTO> GetBeneficiariesForVerification(UrlHelper urlHelper);
        bool UpdateOutboundBeneficiaryOnboardingStatus(Guid id, string status);

        /****************************PROFILE*******************************/
        AdminDTO GetAdminById(Guid id);
    }
}