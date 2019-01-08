using System.Data.Common;

namespace Autumn.Net.Data
{
    public class DbMaxRetryException : DbException
    {
        public DbMaxRetryException() : base("Max retry")
        {            
        }
    }
}