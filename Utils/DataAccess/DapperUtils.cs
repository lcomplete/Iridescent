using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Iridescent.Utils.DataAccess
{
    public static class DapperUtils
    {
        /// <summary>
        /// 执行参数化SQL
        /// </summary>
        /// <returns>受影响的行数</returns>
        public static int Execute(string connectionString, string sql, object param,
            CommandType commandType = CommandType.Text)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                return SqlMapper.Execute(cnn, sql, param, null, null, commandType);
            }
        }

        /// <summary>
        /// 执行查询, 返回指定的T数据类型
        /// </summary>
        public static IList<T> Query<T>(string connectionString, string sql, object param,
            CommandType commandType = CommandType.Text)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                return SqlMapper.Query<T>(cnn, sql, param, null, true, null, commandType).ToList();
            }
        }

        /// <summary>
        /// 执行查询返回多结果集, 可依次访问
        /// </summary>
        public static SqlMapper.GridReader QueryMultiple(string connectionString, string sql, object param,
            CommandType commandType = CommandType.Text)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                return SqlMapper.QueryMultiple(cnn, sql, param, null, null, commandType);
            }
        }

        /// <summary>
        /// 执行查询，返回一个动态对象列表
        /// </summary>
        public static IList<dynamic> Query(string connectionString, string sql, object param,
            CommandType? commandType = CommandType.Text)
        {
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                return SqlMapper.Query(cnn, sql, param, null, true, null, commandType).ToList();
            }
        }

    }
}
