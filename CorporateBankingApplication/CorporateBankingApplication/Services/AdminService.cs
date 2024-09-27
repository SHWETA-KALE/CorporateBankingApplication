using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;
        public AdminService(IAdminRepository adminRepository, IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
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

        //*******************************VERIFY**************************
        public List<ClientDTO> GetClientsForVerification(UrlHelper urlHelper)
        {
            var clients = _adminRepository.GetPendingClients();
            var clientDtos = clients.Select(c => new ClientDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                CompanyName = c.CompanyName,
                ContactInformation = c.ContactInformation,
                Location = c.Location,
                DocumentPaths = c.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return clientDtos;
        }

        public bool UpdateClientOnboardingStatus(Guid id, string status)
        {
            var client = _adminRepository.GetClientById(id);
            if (client == null)
            {
                // Client not found
                return false;
            }
            // Update onboarding status based on the status string
            if (status == "APPROVED")
            {
                client.OnBoardingStatus = CorporateStatus.APPROVED;
                _emailService.SendClientOnboardingStatusEmail(client.Email, "Client Approved!!", $"Dear {client.UserName}, Your client account has been approved as all submitted details and documents meet our onboarding requirements. Now you can access all our services.");
            }
            else if (status == "REJECTED")
            {
                client.OnBoardingStatus = CorporateStatus.REJECTED;
                _emailService.SendClientOnboardingStatusEmail(client.Email, "Client Rejected!!", $"Dear {client.UserName}, Your client account has been rejected due to discrepancies in the submitted details and documents, which do not meet our onboarding requirements.");
            }
            _adminRepository.UpdateClient(client);
            return true;
        }
    }
}