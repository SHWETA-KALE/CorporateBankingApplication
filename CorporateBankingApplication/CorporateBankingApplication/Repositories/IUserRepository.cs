﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateBankingApplication.Models;

namespace CorporateBankingApplication.Repositories
{
    public interface IUserRepository
    {
        User LoggingUser(User user);
        User GetUserByUsername(string username);

        void AddingNewClient(Client client);
    }
}