using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace TinyWebStack.Implementation
{
    internal class Routes : IRoutes
    {
        public string GetVirtualPath(string routeName, object values)
        {
            return this.GetVirtualPath(routeName, new RouteValueDictionary(values));
        }

        public string GetVirtualPath(string routeName, IDictionary<string, object> values)
        {
            var dictionary = values as RouteValueDictionary;

            if (dictionary == null)
            {
                dictionary = new RouteValueDictionary(values);
            }

            var data = RouteTable.Routes.GetVirtualPath(HttpContext.Current.Request.RequestContext, routeName, dictionary);

            return data.VirtualPath;
        }
    }
}
