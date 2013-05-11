using Iridescent.Data;

namespace Iridescent.OrmExpress
{
    public class DataProviderFactory : IDataProviderFactory
    {
        public IDataContext GetDataContext()
        {
            return new SqlServerDataContext();
        }
    }
}