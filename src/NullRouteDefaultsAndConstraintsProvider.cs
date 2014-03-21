using System.Web.Routing;

namespace TinyWebStack
{
    /// <summary>
    /// Internal class that implements default route defaults and constraints.
    /// </summary>
    internal class NullRouteDefaultsAndConstraintsProvider : IRouteDefaultsAndConstraintsProvider
    {
        public static readonly NullRouteDefaultsAndConstraintsProvider Instance = new NullRouteDefaultsAndConstraintsProvider();

        RouteValueDictionary IRouteDefaultsAndConstraintsProvider.RouteDefaults
        {
            get { return null; }
        }

        RouteValueDictionary IRouteDefaultsAndConstraintsProvider.RouteConstraints
        {
            get { return null; }
        }
    }
}
