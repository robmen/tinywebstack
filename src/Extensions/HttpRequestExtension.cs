using System;
using System.Web;

namespace TinyWebStack.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string AppPath(this HttpRequest request)
        {
            return request.ApplicationPath.WithTrailingSlash();
        }

        public static string ApplicationUrl(this HttpRequest request)
        {
            return request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath.WithTrailingSlash();
        }

        public static string ResolveUrl(this HttpRequest request, string url)
        {
            return (url != null && url.StartsWith("~/", StringComparison.Ordinal)) ?
                String.Concat(request.Url.GetLeftPart(UriPartial.Authority), request.ApplicationPath.WithTrailingSlash(), url.Substring(2)) :
                url;
        }
    }
}
