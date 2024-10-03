using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public interface IAdminRepository
    {
        List<Client> GetAllClients();
        void UpdateClientDetails(Client client);
        void DeleteClientDetails(Guid id);

        List<Client> GetPendingClients();

        Client GetClientById(Guid id);

        void UpdateClient(Client client);

        void UpdateClientBeneficiaryStatus(Client client);

        //SALARY DISBURSEMENT
        IEnumerable<EmployeeSalaryDisbursementDTO> GetSalaryDisbursementsByStatus(CorporateStatus status);

        bool ApproveSalaryDisbursement(Guid salaryDisbursementId);
        bool RejectSalaryDisbursement(Guid salaryDisbursementId);

        SalaryDisbursement GetSalaryDisbursementById(Guid id);

        //beneficiary verification
        List<Beneficiary> GetPendingBeneficiaries();
        Beneficiary GetBeneficiaryById(Guid id);

        void UpdateBeneficiary(Beneficiary beneficiary);

        //admin profile 
        Admin GetAdminById(Guid id);

        //payment verification
        IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CorporateStatus status);

        void UpdatePaymentStatus(Guid paymentId, CorporateStatus status);
    }
}