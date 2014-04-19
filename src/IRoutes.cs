using System.Collections.Generic;

namespace TinyWebStack
{
    public interface IRoutes
    {
        string GetVirtualPath(string routeName, object values);

        string GetVirtualPath(string routeName, IDictionary<string, object> values);
    }
}
