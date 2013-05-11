using NHibernate;
using NHibernate.Cfg;

namespace Iridescent.Data.Hibernate
{
	public class SessionHelper
	{
		private static Configuration cfg;
		private static ISessionFactory sessionFactory;

		static SessionHelper()
		{
            cfg = new Configuration();
            cfg.Configure();
            sessionFactory = cfg.BuildSessionFactory();
		}

        public static ISession GetNewSession() 
        {
            return sessionFactory.OpenSession();
        }
	}
}
