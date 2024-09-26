using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public List<Client> ViewAllClients()
        {
            var clients = _adminRepository.GetAllClients();
            return clients;
        }
        public void EditClient(ClientDTO clientDTO, Guid id)
        {
            var client = new Client
            {
                Id = id,
                UserName = clientDTO.UserName,
                Email = clientDTO.Email,
                CompanyName = clientDTO.CompanyName,
                ContactInformation = clientDTO.ContactInformation,
                Location = clientDTO.Location
            };
            _adminRepository.UpdateClientDetails(client);
        }

        public void RemoveClient(Guid id)
        {
            _adminRepository.DeleteClientDetails(id);
        }
    }
}