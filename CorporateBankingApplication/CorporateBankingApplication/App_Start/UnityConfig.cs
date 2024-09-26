using System.Web.Mvc;
using CorporateBankingApplication.Data;
using CorporateBankingApplication.Repositories;
using CorporateBankingApplication.Services;
using NHibernate;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace CorporateBankingApplication
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<ISession>(new InjectionFactory(c => NHibernateHelper.CreateSession()));
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserRepository, UserRepository>();

           
            container.RegisterType<IClientService, ClientService>();
            container.RegisterType<IClientRepository, ClientRepository>();


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}