
namespace TinyWebStack
{
    /// <summary>
    /// Interface to handle DELETE requests.
    /// </summary>
    public interface IDelete
    {
        Status Delete();
    }

    /// <summary>
    /// Interface to handle DELETE requests.
    /// </summary>
    /// <typeparam name="T">Type to contain route data and query string.</typeparam>
    public interface IDelete<T>
    {
        Status Delete(T input);
    }
}
