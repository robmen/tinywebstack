
namespace TinyWebStack
{
    /// <summary>
    /// Interface to provide input data to handler.
    /// </summary>
    /// <typeparam name="T">Type of data input to handler</typeparam>
    public interface IInput<T>
    {
        T Input { set; }
    }
}
