using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;

namespace CorporateBankingApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly CloudinaryService _cloudinaryService;
        public UserService(IUserRepository userRepository, CloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
        }
        public string IsLogging(UserDTO userDto)
        {
            //converted dto to model
            var user = new User { UserName = userDto.UserName, Password = userDto.Password };
            var existingUser = _userRepository.LoggingUser(user);

            if (existingUser != null)
            {
                if (existingUser.Role.RoleName == "Admin")
                {
                    return "Admin";
                }
                else if (existingUser.Role.RoleName == "Client")
                {
                    return "Client";
                }
            }
            return null;
        }

        // Method to get user by their username (needed to retrieve UserId)
        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username); //fetch the user from repository
        }

        public void CreateNewClient(ClientDTO clientDto, IList<HttpPostedFileBase> uploadedFiles)
        {
            var client = new Client()
            {
                UserName = clientDto.UserName,
                Password = clientDto.Password,
                Email = clientDto.Email,
                CompanyName = clientDto.CompanyName,
                ContactInformation = clientDto.ContactInformation,
                Location = clientDto.Location,
                AccountNumber = clientDto.AccountNumber,
                IFSC = clientDto.IFSC,
                Balance = clientDto.Balance,
                OnBoardingStatus = CorporateStatus.PENDING,
                IsActive = true
            };

            string[] documentTypes = { "Company Id Proof", "Address Proof" };

            for (int i = 0; i < uploadedFiles.Count; i++)
            {

                var file = uploadedFiles[i];
                if (file != null && file.ContentLength > 0)
                {
                    string cloudinaryUrl = _cloudinaryService.UploadClientFile(file, client.UserName);
                    var document = new Document
                    {
                        DocumentType = documentTypes[i], // Get document type based on index
                        FilePath = cloudinaryUrl,
                        UploadDate = DateTime.Now,
                        Client = client
                    };

                    client.Documents.Add(document);
                }
            }

            _userRepository.AddingNewClient(client);
        }


    }
}