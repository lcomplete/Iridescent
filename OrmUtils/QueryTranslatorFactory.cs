using Iridescent.Data.QueryModel;

namespace Iridescent.OrmExpress
{
    public class QueryTranslatorFactory
    {
        public static IQueryTranslator Create(SqlExpressCommand command,Query query)
        {
            return new SqlServerQueryTranslator(command,query);
        }
    }
}