
namespace TinyWebStack
{
    /// <summary>
    /// Interface to handle PUT requests.
    /// </summary>
    public interface IPut
    {
        Status Put();
    }

    /// <summary>
    /// Interface to handle PUT requests.
    /// </summary>
    /// <typeparam name="T">Type to contain route data and query string.</typeparam>
    public interface IPut<T>
    {
        Status Put(T input);
    }
}
