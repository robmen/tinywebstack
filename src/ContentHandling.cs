using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TinyWebStack
{
    public static class ContentHandling
    {
        private static IDictionary<string, IList<IContentTypeHandler>> ContentTypeToHandlers { get; set; }

        /// <summary>
        /// Gets or sets the content type to use in place of "*/*". Default is "text/html".
        /// </summary>
        public static string ConsiderWildCardContentTypeAs { get; set; }

        /// <summary>
        /// Registers the content type handlers found in the provided assemblies.
        /// </summary>
        /// <param name="assemblies">Optional list of assemblies to register. Default is the calling assembly.</param>
        public static void RegisterContentTypes(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            ContentTypeToHandlers = new Dictionary<string, IList<IContentTypeHandler>>();

            foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
            {
                foreach (var attribute in type.GetCustomAttributes(typeof(ContentTypeAttribute), true).Cast<ContentTypeAttribute>())
                {
                    IList<IContentTypeHandler> contentHandlers;
                    if (!ContentTypeToHandlers.TryGetValue(attribute.ContentType, out contentHandlers))
                    {
                        contentHandlers = new List<IContentTypeHandler>();
                        ContentTypeToHandlers.Add(attribute.ContentType, contentHandlers);
                    }

                    var contentHandler = (IContentTypeHandler)Activator.CreateInstance(type);
                    contentHandlers.Add(contentHandler);
                }
            }
        }

        /// <summary>
        /// Tries to find a content type writer for the provided accepted content types.
        /// </summary>
        /// <param name="accepted">Accepted content types processed in order.</param>
        /// <param name="handlerType">Type of handler processing the request.</param>
        /// <param name="dataType">Optional type of data returned by the handler.</param>
        /// <param name="writer">Writer to provide.</param>
        /// <returns>True if a writer was found for the accepted content types.</returns>
        public static bool TryGetContentTypeWriter(string[] accepted, Type handlerType, Type dataType, out IContentTypeWriter writer)
        {
            writer = null;

            foreach (var accept in accepted)
            {
                var contentType = accept.Equals("*/*") ? (ConsiderWildCardContentTypeAs ?? "text/html") : accept;

                IList<IContentTypeHandler> contentHandlers;
                if (ContentTypeToHandlers.TryGetValue(contentType, out contentHandlers))
                {
                    foreach (var contentHandler in contentHandlers)
                    {
                        if (contentHandler.TryGetWriter(contentType, handlerType, dataType, out writer))
                        {
                            break;
                        }
                    }
                }
            }

            return writer != null;
        }
    }
}
