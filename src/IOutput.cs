
namespace TinyWebStack
{
    /// <summary>
    /// Interface to provide output data from handler.
    /// </summary>
    /// <typeparam name="T">Type of data output from handler</typeparam>
    public interface IOutput<T>
    {
        T Output { get; }
    }
}
