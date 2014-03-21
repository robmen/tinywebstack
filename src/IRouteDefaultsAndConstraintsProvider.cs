using System.Web.Routing;

namespace TinyWebStack
{
    /// <summary>
    /// Interface that provides rout defaults and contraints.
    /// </summary>
    public interface IRouteDefaultsAndConstraintsProvider
    {
        RouteValueDictionary RouteDefaults { get; }

        RouteValueDictionary RouteConstraints { get; }
    }
}
