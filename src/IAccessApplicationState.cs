using System.Web.Caching;

namespace TinyWebStack
{
    public interface IAccessApplicationState
    {
        ApplicationState ApplicationState { set; }
    }
}
