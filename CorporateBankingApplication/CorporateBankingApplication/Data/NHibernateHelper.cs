using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using CorporateBankingApplication.Mappings;
using NHibernate.Tool.hbm2ddl;

namespace CorporateBankingApplication.Data
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        public static ISession CreateSession()
        {
            if (_sessionFactory == null)
            {
                _sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2012.ConnectionString("Data Source=DELL1520;Initial Catalog=CorporateBankingDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>())
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                    .BuildSessionFactory();
            }
            return _sessionFactory.OpenSession();
        }
    }
}