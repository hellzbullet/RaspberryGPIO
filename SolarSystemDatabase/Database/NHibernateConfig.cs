using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using SolarSystemDatabase.Mappings;
using System;

namespace SolarSystemDatabase.Database
{
    public class NHibernateConfig
    {
        public static Lazy<ISessionFactory> SessionFactory = new Lazy<ISessionFactory>(CreateSessionFactory);

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                           .Database(MySQLConfiguration.Standard.ConnectionString(x => x.FromConnectionStringWithKey("MySQLConnectionString")))
                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<DeviceMap>())
                           .ExposeConfiguration(Configure)
                           .BuildConfiguration()
                           .BuildSessionFactory();
        }

        private static void Configure(Configuration config)
        {
            config.BuildMappings();
            new SchemaUpdate(config).Execute(false, true);
        }
    }
}
