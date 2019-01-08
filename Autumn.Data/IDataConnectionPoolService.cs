using System.Data;

namespace Autumn.Net.Data
{
    public interface IDataConnectionPoolService
    {
        IDbConnection Get();
        void Release(IDbConnection connection);

        IDbDataParameter CreateParameter(string paramName, DbType dbType, object value);
    }
}