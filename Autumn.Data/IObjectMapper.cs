using System.Data;

namespace Autumn.Data
{
    public interface IObjectMapper<T>
    {
        T Get(IDataReader reader);
    }
}