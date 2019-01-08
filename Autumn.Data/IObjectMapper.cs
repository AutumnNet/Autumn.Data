using System.Data;

namespace Autumn.Net.Data
{
    public interface IObjectMapper<T>
    {
        T Get(IDataReader reader);
    }
}