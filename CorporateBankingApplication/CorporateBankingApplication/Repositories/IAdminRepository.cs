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
    }
}