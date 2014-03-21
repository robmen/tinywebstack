using System;
using System.Linq;
using System.Reflection;
using System.Web.Routing;

namespace TinyWebStack.v1
{
    public static class Routing
    {
        public static void RegisterRoutes(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            RegisterRoutes(null, assemblies);
        }

        public static RouteCollection RegisterRoutes(RouteCollection collection, params Assembly[] assemblies)
        {
            collection = collection ?? RouteTable.Routes;

            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            // Gather all the route attributes and sort them by their path information
            // to ensure routes are added to the collection in the correct order.
            var routedTypes = (from t in Assembly.GetCallingAssembly().GetTypes().AsParallel()
                               let attributes = t.GetCustomAttributes(typeof(RouteAttribute), true)
                               where attributes != null && attributes.Length > 0
                               select new RoutedType(t, attributes)).ToList();
            routedTypes.Sort();

            foreach (var routedType in routedTypes)
            {
                ControllerBase controller = Activator.CreateInstance(routedType.Type) as ControllerBase;
                if (controller == null)
                {
                    continue;
                }

                foreach (var routeAttribute in routedType.Attributes)
                {
                    var defaultsAndContraints = (routeAttribute.DefaultsAndConstraintsProviderType != null) ?
                        (IRouteDefaultsAndConstraintsProvider)Activator.CreateInstance(routeAttribute.DefaultsAndConstraintsProviderType) :
                        NullRouteDefaultsAndConstraintsProvider.Instance;

                    collection.Add(new Route(routeAttribute.Path, defaultsAndContraints.RouteDefaults, defaultsAndContraints.RouteConstraints, controller));
                }
            }

            return collection;
        }

        private class RoutedType : IComparable<RoutedType>
        {
            public RoutedType(Type t, object[] attributes)
            {
                this.Type = t;
                this.Attributes = attributes.Cast<RouteAttribute>().OrderByDescending(a => a.Complexity).ToArray();
            }

            public Type Type { get; set; }

            public RouteAttribute[] Attributes { get; set; }

            public int CompareTo(RoutedType other)
            {
                // Compare the most complex route attributes (the first in the array).
                return this.Attributes[0].CompareTo(other.Attributes[0]);
            }
        }
    }
}
