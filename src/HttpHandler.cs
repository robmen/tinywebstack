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
            Status status = null;

            HandlerEntry entry;

            if (TryGetMethodNameForMethod(http.Request.HttpMethod, this.HandlerType, out entry))
            {
                var queryStringData = PopulateDictionary(http.Request.Unvalidated().QueryString, this.RouteData.Values);
                var formData = PopulateDictionary(http.Request.Unvalidated().Form);

                object queryStringObject = null;
                if (entry.QueryStringObjectType != null)
                {
                    queryStringObject = Activator.CreateInstance(entry.QueryStringObjectType);
                    this.AssignInputs(queryStringData, entry.QueryStringObjectType, queryStringObject);
                }

                Type inputDataType = null;
                Type genericInputDataType = null;
                if (this.HandlerType.TryGetGenericInterfaceImplementedType(typeof(IInput<>), out inputDataType))
                {
                    genericInputDataType = typeof(IInput<>).MakeGenericType(inputDataType);
                }

                Type outputDataType = null;
                Type genericOutputDataType = null;
                IContentTypeWriter writer = null;
                if (!entry.NoOutput && this.HandlerType.TryGetGenericInterfaceImplementedType(typeof(IOutput<>), out outputDataType))
                {
                    genericOutputDataType = typeof(IOutput<>).MakeGenericType(outputDataType);

                    // TODO: throw if this comes back false?
                    ContentHandling.TryGetContentTypeWriter(http.Request.AcceptTypes, this.HandlerType, outputDataType, out writer);
                }

                var handler = Activator.CreateInstance(this.HandlerType);

                var appState = handler as IAccessApplicationState;
                if (appState != null)
                {
                    appState.ApplicationState = new ApplicationState(http.Cache);
                }

                this.AssignInputs(queryStringData, this.HandlerType, handler);
                this.ReadCookies(http.Request.Cookies, this.HandlerType, handler);

                if (inputDataType != null)
                {
                    var inputDataObject = Activator.CreateInstance(inputDataType);
                    this.AssignInputs(formData, inputDataType, inputDataObject);

                    PropertyInfo prop = genericInputDataType.GetProperty("Input");
                    prop.SetValue(handler, inputDataObject, null);
                }

                status = (Status)this.HandlerType.InvokeMember(entry.MethodName, BindingFlags.InvokeMethod, null, handler, (queryStringObject == null) ? null : new[] { queryStringObject });

                object outputData = null;
                if (outputDataType != null)
                {
                    PropertyInfo prop = genericOutputDataType.GetProperty("Output");
                    outputData = prop.GetValue(handler, null);
                }

                if (writer != null && outputData != null)
                {
                    http.Response.ContentType = writer.ContentType;

                    writer.Write(http.Response.Output, outputData);
                }

                this.WriteCookies(http.Response.Cookies, this.HandlerType, handler);
            }
            else
            {
                status = Status.MethodNotAllowed;
            }

            http.Response.StatusCode = status.Code;
            http.Response.StatusDescription = status.Description;
            http.Response.RedirectLocation = http.Request.ResolveUrl(status.Location);
            http.Response.TrySkipIisCustomErrors = true;
        }

        private IDictionary<string, object> PopulateInputs(RouteData routeData, NameValueCollection queryString, NameValueCollection formData)
        {
            var inputs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (string key in queryString.Keys)
            {
                var values = queryString.GetValues(key);

                if (values.Length == 1)
                {
                    inputs[key] = values[0];
                }
                else
                {
                    inputs[key] = values[0];
                }
            }

            foreach (var data in routeData.Values)
            {
                inputs[data.Key] = data.Value;
            }

            return inputs;
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
                        inputs[key] = nvc.GetValues(key);
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

        private void AssignInputs(IDictionary<string, object> inputs, Type type, object target)
        {
            foreach (var propInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => p.GetSetMethod() != null))
            {
                object value;
                if (inputs.TryGetValue(propInfo.Name, out value))
                {
                    var valueType = value.GetType();

                    if (!propInfo.PropertyType.IsArray && valueType.IsArray)
                    {
                        value = ((Array)value).GetValue(0);
                    }

                    var assign = Convert.ChangeType(value, propInfo.PropertyType);
                    propInfo.SetValue(target, assign, null);
                }
            }
        }

        private void ReadCookies(HttpCookieCollection cookies, Type type, object target)
        {
            foreach (var propInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => p.GetSetMethod() != null))
            {
                foreach (var cookieAttribute in propInfo.GetCustomAttributes(typeof(CookieAttribute), false).Cast<CookieAttribute>())
                {
                    var cookie = cookies[cookieAttribute.Name];

                    if (cookie != null)
                    {
                        object value = (cookie.Values.Count > 1) ? cookie[propInfo.Name] : cookie.Value;
                        var assign = Convert.ChangeType(value, propInfo.PropertyType);
                        propInfo.SetValue(target, assign, null);
                    }
                }
            }
        }

        private void WriteCookies(HttpCookieCollection cookies, Type type, object target)
        {
            foreach (var propInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => p.GetGetMethod() != null))
            {
                foreach (var cookieAttribute in propInfo.GetCustomAttributes(typeof(CookieAttribute), false).Cast<CookieAttribute>())
                {
                    var cookie = new HttpCookie(cookieAttribute.Name);
                    cookies.Add(cookie);

                    object value = propInfo.GetValue(target, null);

                    if (value == null)
                    {
                        cookie.Expires = HttpHandler.DeleteCookieDate;
                    }
                    else
                    {
                        cookie.Value = value.ToString();
                        cookie.HttpOnly = cookieAttribute.HttpOnly;
                        cookie.Secure = cookieAttribute.Secure;

                        if (cookieAttribute.Expires > 0)
                        {
                            cookie.Expires = DateTime.UtcNow.AddMinutes(cookieAttribute.Expires);
                        }

                        if (!String.IsNullOrEmpty(cookieAttribute.Path))
                        {
                            cookie.Path = cookieAttribute.Path;
                        }
                    }
                }
            }
        }

        private static bool TryGetMethodNameForMethod(string method, Type handlerType, out HandlerEntry entry)
        {
            entry = null;

            string methodName = null;
            Type methodType = null;
            Type methodInputType = null;

            switch (method)
            {
                case "GET":
                    methodName = "Get";
                    methodType = typeof(IGet);
                    methodInputType = typeof(IGet<>);
                    break;

                case "POST":
                    methodName = "Post";
                    methodType = typeof(IPost);
                    methodInputType = typeof(IPost<>);
                    break;

                case "PUT":
                    methodName = "Put";
                    methodType = typeof(IPut);
                    methodInputType = typeof(IPut<>);
                    break;

                case "DELETE":
                    methodName = "Delete";
                    methodType = typeof(IDelete);
                    methodInputType = typeof(IDelete<>);
                    break;

                case "PATCH":
                    methodName = "Patch";
                    methodType = typeof(IPatch);
                    methodInputType = typeof(IPatch<>);
                    break;

                case "HEAD":
                    methodName = "Head";
                    methodType = typeof(IHead);
                    methodInputType = typeof(IHead<>);
                    break;

                default:
                    return false;
            }

            Type inputType = null;
            if (methodInputType != null && handlerType.TryGetGenericInterfaceImplementedType(methodInputType, out inputType))
            {
                entry = new HandlerEntry() { MethodName = methodName, QueryStringObjectType = inputType };
            }
            else if (handlerType.ImplementsInterface(methodType))
            {
                entry = new HandlerEntry() { MethodName = methodName };
            }

            return entry != null;
        }

        private class HandlerEntry
        {
            public bool NoOutput { get { return "Head".Equals(this.MethodName); } }

            public string MethodName { get; set; }

            public Type QueryStringObjectType { get; set; }
        }
    }
}
