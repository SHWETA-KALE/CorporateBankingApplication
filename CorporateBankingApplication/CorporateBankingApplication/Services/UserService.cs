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

        public string IsLogging(UserDTO userDto) //converted dto to model
        {
            var user = new User { UserName = userDto.UserName, Password = userDto.Password };
            var existingUser = _userRepository.LoggingUser(user);
            string role;
            if (existingUser != null)
            {
                if(existingUser.Role.RoleName == "Admin")
                {
                    return role = "Admin";
                }
                else if(existingUser.Role.RoleName == "Client")
                {
                    return role = "Client";
                }
            }
            return role = null;           
        }
    }
}