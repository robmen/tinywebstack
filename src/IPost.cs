
namespace TinyWebStack
{
    /// <summary>
    /// Interface to handle POST requests.
    /// </summary>
    public interface IPost
    {
        Status Post();
    }

    /// <summary>
    /// Interface to handle POST requests.
    /// </summary>
    /// <typeparam name="T">Type to contain route data and query string.</typeparam>
    public interface IPost<T>
    {
        Status Post(T input);
    }
}
