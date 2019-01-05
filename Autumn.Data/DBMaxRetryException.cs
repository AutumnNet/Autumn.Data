using System.Data.Common;

namespace Autumn.Data
{
    public class DbMaxRetryException : DbException
    {
        public DbMaxRetryException() : base("Max retry")
        {            
        }
    }
}