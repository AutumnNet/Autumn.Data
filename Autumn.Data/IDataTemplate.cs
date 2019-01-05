using System.Collections.Generic;
using System.Data;

namespace Autumn.Data
{
    public interface IDataTemplate
    {
        IDbCommand CreateCommand(DataTransaction t, IDbConnection dbConnection);
        IDbDataParameter CreateParameter(string paramName, DbType dbType, object value);

        void ExecuteParams(IEnumerable<DataTransaction> transactions);
        void ExecuteParams(string query, IDbDataParameter[] dataParams, params object[] args);

        
        void Execute(string query, params object[] args);
        object Query(DExecute exec, string query, params object[] args);

        T One<T>(IObjectMapper<T> mapper, string query, params object[] args);

        IEnumerable<T> Query<T>(IObjectMapper<T> mapper, string query, params object[] args);
    }
}