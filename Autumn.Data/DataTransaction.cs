using System.Data;

namespace Autumn.Net.Data
{
    public class DataTransaction
    {
        public string Query { get; set; }
        public readonly IDbDataParameter[] DataParams = null;
        public object[] Args {get;set;}

        public DataTransaction(string query, IDbDataParameter[] dataParams, params object[] args){
            Query = query;
            DataParams = dataParams;
            Args = args;
        }
    }}