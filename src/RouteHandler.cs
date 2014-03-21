using System;
using System.Web;
using System.Web.Routing;

namespace TinyWebStack
{
    public class RouteHandler : IRouteHandler
    {
        public RouteHandler(Type handlerType)
        {
            this.HandlerType = handlerType;
        }

        private Type HandlerType { get; set; }

        /// <summary>
        /// Gets the HTTP handler for this handler type.
        /// </summary>
        /// <param name="requestContext">Route data provided to the request.</param>
        /// <returns>HTTP handler that can process the handler type.</returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler(this.HandlerType, requestContext.RouteData);
        }
    }
}
