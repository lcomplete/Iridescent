namespace Iridescent.OrmExpress
{
    public class SqlExpressCommandGeneratorFactory
    {
         public static ISqlExpressCommandGenerator Create()
         {
             return new SqlExpressCommandGenerator();
         }
    }
}