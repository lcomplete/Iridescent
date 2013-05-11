using Iridescent.Data.QueryModel;

namespace Iridescent.OrmExpress
{
    public interface ISqlExpressCommandGenerator
    {

        SqlExpressCommand GenerateSelectCommand<TEntity>(Query query = null,bool getCount=false);

        SqlExpressCommand GenerateGetPagingListCommand<TEntity>(int pageIndex, int pageSize,Query query=null);

        SqlExpressCommand GenerateDeleteCommand<TEntity>(Query query = null);

        SqlExpressCommand GenerateDeleteCommand<TEntity>(TEntity entity);

        SqlExpressCommand GenerateUpdateCommand<TEntity>(TEntity entity);

        SqlExpressCommand GenerateInsertCommand<TEntity>(TEntity entity);
        
    }
}
