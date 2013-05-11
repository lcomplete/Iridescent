using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Iridescent.Data.QueryModel;
using System.Linq;

namespace Iridescent.OrmExpress
{
    internal class SqlServerOrmQuery:OrmQuery
    {
        public SqlServerOrmQuery(string connectionString) : base(connectionString)
        {
        }

        protected override IDataReader ExecuteReader(SqlExpressCommand command)
        {
            if(command.DbParameters==null)
            {
                return SqlHelper.ExecuteReader(ConnectionString, command.CommandType, command.StatementString);
            }
            SqlParameter[] sqlParameters = command.DbParameters.Cast<SqlParameter>().ToArray();
            return SqlHelper.ExecuteReader(ConnectionString, command.CommandType, command.StatementString,
                                           sqlParameters);
        }

    }
}