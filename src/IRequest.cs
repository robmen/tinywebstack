using System;

namespace TinyWebStack
{
    /// <summary>
    /// Interface that can be resolved using the IoC <see cref="Container"/> to access the web request.
    /// </summary>
    public interface IRequest
    {
        string ApplicationPath { get; }

        string ApplicationRootUrl { get; }

        Uri Url { get; }

        Uri Referrer { get; }

        string ResolveApplicationUrl(string url);
    }
}
