using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using Microsoft.AspNetCore.Http;

namespace CorporateBankingApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string IsLogging(UserDTO userDto) //converted dto to model
        {
            var user = new User { UserName = userDto.UserName, Password = userDto.Password };
            var existingUser = _userRepository.LoggingUser(user);
            string role;
            if (existingUser != null)
            {
                if (existingUser.Role.RoleName == "Admin")
                {
                    return role = "Admin";
                }
                else if (existingUser.Role.RoleName == "Client")
                {
                    return role = "Client";
                }
            }
            return role = null;
        }

        /*public void CreateNewClient(ClientDTO clientDto)
        {
            var user = new User()
            {
                UserName = clientDto.UserName,
                Password = clientDto.Password,
                Email = clientDto.Email,
                Role = new Role() { RoleName = "Client" } 
            };
            user.Role.User = user;
            var documents = new List<Document>();
            if (clientDto.Document1 != null)
            {
                var document1 = new Document()
                {
                    DocumentType="d1",
                    FilePath=SaveDocument(clientDto.Document1),
                    UploadDate = DateTime.Now
                };
                documents.Add(document1);
            }
            if (clientDto.Document2 != null)
            {
                var document2 = new Document()
                {
                    DocumentType = "d2",
                    FilePath = SaveDocument(clientDto.Document2),
                    UploadDate = DateTime.Now
                };
                documents.Add(document2);
            }

            //clientdto to client model
            var client = new Client()
            {
                UserName = clientDto.UserName,
                Password = clientDto.Password,
                Email = clientDto.Email,
                CompanyName = clientDto.CompanyName,
                ContactInformation = clientDto.ContactInformation,
                Location = clientDto.Location,
                Role = user.Role,
                Documents = documents,
                OnBoardingStatus = Status.PENDING
            };
            client.Id = user.Id;
            _userRepository.AddingNewClient(client);
        }*/

        public void CreateNewClient(ClientDTO clientDto)
        {
            var client = new Client()
            {
                UserName = clientDto.UserName,
                Password = clientDto.Password,
                Email = clientDto.Email,
                CompanyName = clientDto.CompanyName,
                ContactInformation = clientDto.ContactInformation,
                Location = clientDto.Location,
                OnBoardingStatus = Status.PENDING
            };
            _userRepository.AddingNewClient(client);
        }

        /*private string SaveDocument(IFormFile document)
        {
            
             //string _FileName = Path.GetFileName(file.FileName);
                   // string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
             
            var uploadPath = Path.Combine("Uploads", document.FileName);

            // Ensure the directory exists
            if (!Directory.Exists("Uploads"))
            {
                Directory.CreateDirectory("Uploads");
            }

            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                document.CopyTo(fileStream);
            }

            return uploadPath; // Return the path where the file is saved
        }*/
    }
}