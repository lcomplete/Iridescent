namespace Iridescent.OrmExpress
{
    public sealed class OrmQueryFactory
    {
         public static OrmQuery Create(string connectionString)
         {
             return new SqlServerOrmQuery(connectionString);
         }
    }
}