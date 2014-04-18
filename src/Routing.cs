﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;

namespace TinyWebStack
{
    public static class Routing
    {
        /// <summary>
        /// Registers the routes found in the provided assemblies into the global <see cref="RouteTable"/>.
        /// </summary>
        /// <param name="assemblies">Optional list of assemblies to register. Default is the calling assembly.</param>
        public static void RegisterRoutes(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            var collection = RouteTable.Routes;

            // Gather all the route attributes and sort them by their path information
            // to ensure routes are added to the collection in the correct order.
            var routedTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .AsParallel()
                .Select(t => new { Type = t, Attributes = t.GetCustomAttributes(typeof(RouteAttribute), true) })
                .Where(ta => ta.Attributes != null && ta.Attributes.Length > 0)
                .Select(ta => new RoutedType(ta.Type, ta.Attributes))
                .ToList();
            routedTypes.Sort();

            foreach (var routedType in routedTypes)
            {
                var routeHandler = new RouteHandler(routedType.Type);

                foreach (var routeAttribute in routedType.Attributes)
                {
                    var defaultsAndContraints = (routeAttribute.DefaultsAndConstraintsProviderType != null) ?
                        (IRouteDefaultsAndConstraintsProvider)Activator.CreateInstance(routeAttribute.DefaultsAndConstraintsProviderType) :
                        NullRouteDefaultsAndConstraintsProvider.Instance;

                    var route = new Route(routeAttribute.Path, defaultsAndContraints.RouteDefaults, defaultsAndContraints.RouteConstraints, routeHandler);

                    if (String.IsNullOrEmpty(routeAttribute.Name))
                    {
                        collection.Add(route);
                    }
                    else
                    {
                        collection.Add(routeAttribute.Name, route);
                    }
                }
            }
        }

        private class RoutedType : IComparable<RoutedType>
        {
            public RoutedType(Type t, object[] attributes)
            {
                this.Type = t;
                this.Attributes = attributes.Cast<RouteAttribute>().OrderByDescending(a => a.Complexity).ToList();
            }

            public Type Type { get; private set; }

            public IEnumerable<RouteAttribute> Attributes { get; private set; }

            public int CompareTo(RoutedType other)
            {
                // Compare the most complex route attributes (the first in the array).
                return this.Attributes.First().CompareTo(other.Attributes.First());
            }
        }
    }
}
