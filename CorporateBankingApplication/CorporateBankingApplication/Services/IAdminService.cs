using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
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
        void RemoveClient(Guid id);

        List<ClientDTO> GetClientsForVerification(UrlHelper urlHelper);

        bool UpdateClientOnboardingStatus(Guid id, string status);

        bool ToggleClientActiveStatus(Guid id, bool isActive, out string message);

        //SALARY DISBURSEMENTS
        IEnumerable<EmployeeSalaryDisbursementDTO> GetPendingSalaryDisbursements();


        //bool ApproveSalaryDisbursement(Guid salaryDisbursementId);
        // bool RejectSalaryDisbursement(Guid salaryDisbursementId);

        bool ApproveSalaryDisbursement(Guid salaryDisbursementId, bool isBatch = false);
        bool RejectSalaryDisbursement(Guid salaryDisbursementId, bool isBatch = false);

        //beneficiary


        List<BeneficiaryDTO> GetBeneficiariesForVerification(UrlHelper urlHelper);
        bool UpdateOutboundBeneficiaryOnboardingStatus(Guid id, string status);

        //admin profile 
        AdminDTO GetAdminById(Guid id);

        //payment verification 
        IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CorporateStatus status);

        void UpdatePaymentStatuses(List<Guid> paymentIds, CorporateStatus status);
    }
}