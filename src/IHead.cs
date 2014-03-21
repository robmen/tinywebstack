
namespace TinyWebStack
{
    /// <summary>
    /// Interface to handle HEAD requests.
    /// </summary>
    public interface IHead
    {
        Status Head();
    }

    /// <summary>
    /// Interface to handle HEAD requests.
    /// </summary>
    /// <typeparam name="T">Type to contain route data and query string.</typeparam>
    public interface IHead<T>
    {
        Status Head(T input);
    }
}
