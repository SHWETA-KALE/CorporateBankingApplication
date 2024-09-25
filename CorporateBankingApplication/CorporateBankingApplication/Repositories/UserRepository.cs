using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CorporateBankingApplication.Data;
using CorporateBankingApplication.Models;
using NHibernate;

namespace CorporateBankingApplication.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISession _session;

        public UserRepository(ISession session)
        {
            _session = session;
        }

        public User LoggingUser(User user)
        {
            var existingUser = _session.Query<User>().FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
            return existingUser;

        }

        //&& PasswordHelper.VerifyPassword(user.Password, u.Password));


    }
}