using System.Web.Routing;

namespace TinyWebStack
{
    public interface IRoutes
    {
        string GetVirtualPath(string routeName, RouteValueDictionary values);
    }
}
