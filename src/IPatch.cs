
namespace TinyWebStack
{
    /// <summary>
    /// Interface to handle PATCH requests.
    /// </summary>
    public interface IPatch
    {
        Status Patch();
    }

    /// <summary>
    /// Interface to handle PATCH requests.
    /// </summary>
    /// <typeparam name="T">Type to contain route data and query string.</typeparam>
    public interface IPatch<T>
    {
        Status Patch(T input);
    }
}
