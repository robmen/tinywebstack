
namespace TinyWebStack
{
    /// <summary>
    /// Interface that can be resolved using the IoC <see cref="Container"/> to access the web server functionality.
    /// </summary>
    public interface IServerUtility
    {
        string MapPath(string path);
    }
}
