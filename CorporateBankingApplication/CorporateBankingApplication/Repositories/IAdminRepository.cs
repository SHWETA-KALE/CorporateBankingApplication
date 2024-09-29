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

        //SALARY DISBURSEMENT
        IEnumerable<EmployeeSalaryDisbursementDTO> GetSalaryDisbursementsByStatus(CorporateStatus status);

        bool ApproveSalaryDisbursement(Guid salaryDisbursementId);
        bool RejectSalaryDisbursement(Guid salaryDisbursementId);






    }
}