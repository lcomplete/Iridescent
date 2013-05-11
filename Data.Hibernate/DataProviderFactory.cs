namespace Iridescent.Data.Hibernate
{
    public class DataProviderFactory : IDataProviderFactory
	{
        public IDataContext GetDataContext()
        {
            return new NHibernateDataContext();
        }
    }
}
