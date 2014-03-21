using System;

namespace TinyWebStack
{
    /// <summary>
    /// Attribute to indicate the content types supported by a class that implements <see cref="IContentTypeHandler"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(string contentType)
        {
            this.ContentType = contentType;
        }

        /// <summary>
        /// Content type supported by the handler.
        /// </summary>
        public string ContentType { get; private set; }
    }
}
