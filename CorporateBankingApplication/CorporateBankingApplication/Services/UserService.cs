using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;

namespace CorporateBankingApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string IsLogging(UserDTO userDto) 
        {
            //converted dto to model
            var user = new User { UserName = userDto.UserName, Password = userDto.Password };
            var existingUser = _userRepository.LoggingUser(user);

            if (existingUser != null)
            {
                if(existingUser.Role.RoleName == "Admin")
                {
                    return  "Admin";
                }
                else if(existingUser.Role.RoleName == "Client")
                {
                    return "Client";
                }
            }
            return  null;           
        }

        // Method to get user by their username (needed to retrieve UserId)
        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username); //fetch the user from repository
        }
    }
}