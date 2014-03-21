
namespace TinyWebStack
{
    /// <summary>
    /// Interface to handle GET requests.
    /// </summary>
    public interface IGet
    {
        Status Get();
    }

    /// <summary>
    /// Interface to handle GET requests.
    /// </summary>
    /// <typeparam name="T">Type to contain route data and query string.</typeparam>
    public interface IGet<T>
    {
        Status Get(T input);
    }
}
