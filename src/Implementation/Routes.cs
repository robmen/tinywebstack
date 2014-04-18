using System.Web;
using System.Web.Routing;

namespace TinyWebStack.Implementation
{
    internal class Routes : IRoutes
    {
        public string GetVirtualPath(string routeName, RouteValueDictionary values)
        {
            var data = RouteTable.Routes.GetVirtualPath(HttpContext.Current.Request.RequestContext, routeName, values);

            return data.VirtualPath;
        }
    }
}
