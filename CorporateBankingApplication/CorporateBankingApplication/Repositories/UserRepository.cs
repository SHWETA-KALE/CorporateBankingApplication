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
            //var existingUser = _session.Query<User>().FirstOrDefault(u => u.UserName == user.UserName && PasswordHelper.VerifyPassword(user.Password, u.Password));
            //return existingUser;

            var existingUser = _session.Query<User>().FirstOrDefault(u => u.UserName == user.UserName);

            if (existingUser != null && PasswordHelper.VerifyPassword(user.Password, existingUser.Password))
            {
                return existingUser; 
            }
            return null;

        }

        //fetching the user by their username 
        public User GetUserByUsername(string username)
        {
            return _session.Query<User>().FirstOrDefault(u => u.UserName == username);
        }

        //&& PasswordHelper.VerifyPassword(user.Password, u.Password));


        //REGISTER
        public void AddingNewClient(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                client.Password = PasswordHelper.HashPassword(client.Password);
                var role = new Role()
                {
                    RoleName = "Client",
                    User = client
                };
                _session.Save(client);
                _session.Save(role);
                transaction.Commit();
            }
        }


    }
}