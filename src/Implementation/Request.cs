using System;
using System.Web;
using TinyWebStack.Extensions;

namespace TinyWebStack.Implementation
{
    internal class Request : IRequest
    {
        public Request()
        {
            this.HttpRequest = HttpContext.Current.Request;
        }

        private HttpRequest HttpRequest { get; set; }

        public string ApplicationPath
        {
            get { return this.HttpRequest.ApplicationPath.WithTrailingSlash(); }
        }

        public string ApplicationRootUrl
        {
            get { return this.HttpRequest.Url.GetLeftPart(UriPartial.Authority) + this.HttpRequest.ApplicationPath.WithTrailingSlash(); }
        }

        public Uri Url
        {
            get { return this.HttpRequest.Url; }
        }

        public Uri Referrer
        {
            get { return this.HttpRequest.UrlReferrer; }
        }

        public string ResolveApplicationUrl(string url)
        {
            return (url != null && url.StartsWith("~/", StringComparison.Ordinal)) ?
                String.Concat(this.HttpRequest.Url.GetLeftPart(UriPartial.Authority), this.HttpRequest.ApplicationPath.WithTrailingSlash(), url.Substring(2)) :
                url;
        }
    }
}
