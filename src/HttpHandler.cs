using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Routing;
using TinyWebStack.Extensions;

namespace TinyWebStack
{
    public class HttpHandler : IHttpHandler
    {
        private static readonly DateTime DeleteCookieDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public HttpHandler(Type handlerType, RouteData routeData)
        {
            this.HandlerType = handlerType;

            this.RouteData = routeData;
        }

        public virtual bool IsReusable
        {
            get { return false; }
        }

        private Type HandlerType { get; set; }

        private RouteData RouteData { get; set; }

        public void ProcessRequest(HttpContext http)
        {
            Status status = this.GetStatus(http);

            http.Response.StatusCode = status.Code;
            http.Response.StatusDescription = status.Description;
            http.Response.RedirectLocation = http.Request.ResolveUrl(status.Location);
            http.Response.TrySkipIisCustomErrors = true;
        }

        private Status GetStatus(HttpContext http)
        {
            var handlerMethod = this.HandlerType.GetMethod(http.Request.HttpMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);

            if (handlerMethod == null)
            {
                return Status.MethodNotAllowed;
            }

            var handler = this.CreateInstanceWithResolution(this.HandlerType);

            if (handler == null)
            {
                return Status.InternalServerError;
            }

            var handlerProperties = this.HandlerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).ToList();

            // Assign query string/route data to the handler and query string objects.
            //
            var queryStringObjects = handlerMethod.GetParameters().Select(param => Activator.CreateInstance(param.ParameterType)).ToArray();

            var queryString = http.Request.Unvalidated().QueryString;

            var queryStringData = PopulateDictionary(queryString, this.RouteData.Values);

            if (queryString.Count > 0)
            {
                queryStringData["_RawQueryString"] = queryString.ToString().UrlDecode();
            }

            this.AssignInputs(queryStringData, handler, handlerProperties.Where(p => p.GetSetMethod() != null && !p.Name.Equals("Input") && !(p.Equals("Output"))));

            foreach (var queryStringObject in queryStringObjects)
            {
                this.AssignInputs(queryStringData, queryStringObject, null);
            }

            // If there is an input property, assign it.
            //
            var inputDataProperty = handlerProperties.Where(p => p.GetSetMethod() != null && p.Name.Equals("Input")).SingleOrDefault();

            if (inputDataProperty != null)
            {
                var inputObject = Activator.CreateInstance(inputDataProperty.PropertyType);

                // TODO: check the incoming content type and do not assume it's always POSTed form data.
                var formData = PopulateDictionary(http.Request.Unvalidated().Form);

                this.AssignInputs(formData, inputObject, null);

                inputDataProperty.SetValue(handler, inputObject, null);
            }

            // Get cookie properties and assign any cookies from the request.
            //
            var cookieProperties = this.GetCookieProperties(handlerProperties).ToList();

            this.ReadCookies(http.Request.Cookies, handler, cookieProperties);

            // If there is an output and the output is requested, find a content writer.
            //
            var outputDataProperty = handlerProperties.Where(p => p.GetGetMethod() != null && p.Name.Equals("Output")).SingleOrDefault();

            IContentTypeWriter writer = null;

            if (outputDataProperty != null && !"HEAD".Equals(http.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                // TODO: throw if this comes back false?
                ContentHandling.TryGetContentTypeWriter(http.Request.AcceptTypes, this.HandlerType, outputDataProperty.PropertyType, out writer);
            }

            Status status = (Status)handlerMethod.Invoke(handler, queryStringObjects);

            object outputData = null;
            if (outputDataProperty != null)
            {
                outputData = outputDataProperty.GetValue(handler, null);
            }

            if (outputData != null && writer != null)
            {
                writer.Write(http.Response.Output, outputData);

                http.Response.ContentType = writer.ContentType;
            }

            // Write cookies back to the response.
            //
            this.WriteCookies(http.Response.Cookies, handler, cookieProperties);

            return status;
        }

        private object CreateInstanceWithResolution(Type type)
        {
            bool success = false;
            object[] arguments = null;

            foreach (var constructor in type.GetConstructors().OrderByDescending(c => c.GetParameters().Length))
            {
                var parameters = constructor.GetParameters();
                arguments = new object[parameters.Length];

                try
                {
                    for (int i = 0; i < arguments.Length; ++i)
                    {
                        arguments[i] = Container.Current.Resolve(parameters[i].ParameterType);
                    }

                    success = true;
                    break;
                }
                catch (KeyNotFoundException)
                {
                }
            }

            return success ? Activator.CreateInstance(type, arguments) : null;
        }

        private IDictionary<string, object> PopulateDictionary(params object[] collections)
        {
            var inputs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (var collection in collections)
            {
                NameValueCollection nvc = collection as NameValueCollection;
                if (nvc != null)
                {
                    foreach (string key in nvc.Keys)
                    {
                        inputs[key ?? "_UnnamedQueryStringValue"] = nvc.GetValues(key);
                    }
                }
                else
                {
                    var dictionary = collection as IDictionary<string, object>;
                    foreach (var kvp in dictionary)
                    {
                        inputs[kvp.Key] = kvp.Value;
                    }
                }
            }
            return inputs;
        }

        private void AssignInputs(IDictionary<string, object> inputs, object target, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties ?? target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => p.GetSetMethod() != null))
            {
                if (property.PropertyType.IsInterface)
                {
                    try
                    {
                        object resolved = Container.Current.Resolve(property.PropertyType);
                        property.SetValue(target, resolved, null);
                    }
                    catch (KeyNotFoundException)
                    {
                        // TODO: should we rethrow here since it indicates the interface was not registered with the container?
                    }
                }
                else
                {
                    object value;
                    if (inputs.TryGetValue(property.Name, out value))
                    {
                        if (value != null)
                        {
                            var valueType = value.GetType();

                            if (!property.PropertyType.IsArray && valueType.IsArray)
                            {
                                value = ((Array)value).GetValue(0);
                            }
                        }

                        var assign = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(target, assign, null);
                    }
                }
            }
        }

        private IEnumerable<CookiedProperty> GetCookieProperties(IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                foreach (var attribute in property.GetCustomAttributes(typeof(CookieAttribute), false).Cast<CookieAttribute>())
                {
                    yield return new CookiedProperty() { Property = property, Attribute = attribute };
                }
            }
        }

        private void ReadCookies(HttpCookieCollection cookies, object target, IEnumerable<CookiedProperty> cookiedProperties)
        {
            foreach (var cookiedProperty in cookiedProperties.Where(p => p.Property.GetSetMethod() != null))
            {
                var cookie = cookies[cookiedProperty.Attribute.Name];

                if (cookie != null)
                {
                    object value = (cookie.Values.Count > 1) ? cookie[cookiedProperty.Property.Name] : cookie.Value;

                    var assign = Convert.ChangeType(value, cookiedProperty.Property.PropertyType);

                    cookiedProperty.Property.SetValue(target, assign, null);
                }
            }
        }

        private void WriteCookies(HttpCookieCollection cookies, object target, IEnumerable<CookiedProperty> cookiedProperties)
        {
            foreach (var cookiedProperty in cookiedProperties.Where(p => p.Property.GetGetMethod() != null))
            {
                var cookie = new HttpCookie(cookiedProperty.Attribute.Name);
                cookies.Add(cookie);

                object value = cookiedProperty.Property.GetValue(target, null);

                if (value == null)
                {
                    cookie.Expires = HttpHandler.DeleteCookieDate;
                }
                else
                {
                    cookie.Value = value.ToString();
                    cookie.HttpOnly = cookiedProperty.Attribute.HttpOnly;
                    cookie.Secure = cookiedProperty.Attribute.Secure;

                    if (cookiedProperty.Attribute.Expires > 0)
                    {
                        cookie.Expires = DateTime.UtcNow.AddMinutes(cookiedProperty.Attribute.Expires);
                    }

                    if (!String.IsNullOrEmpty(cookiedProperty.Attribute.Path))
                    {
                        cookie.Path = cookiedProperty.Attribute.Path;
                    }
                }
            }
        }

        private class CookiedProperty
        {
            public PropertyInfo Property { get; set; }

            public CookieAttribute Attribute { get; set; }
        }
    }
}
