using System;

namespace TinyWebStack
{
    /// <summary>
    /// Interface for factories to handle content types. Classes that implement this interface should
    /// have one or more <see cref="ContentTypeAttribute"/> indicating the supported content types.
    /// </summary>
    public interface IContentTypeHandler
    {
        /// <summary>
        /// Tries to find a <see cref="ContentTypeWriter"/> for the provided information.
        /// </summary>
        /// <param name="contentType">Content type the writer should output.</param>
        /// <param name="handlerType">Type of handler providing content.</param>
        /// <param name="dataType">Date type of provided content.</param>
        /// <param name="writer">Found content type writer.</param>
        /// <returns>True if content type writer is found for the provided content type, handler type and data type. Otherwise, false.</returns>
        bool TryGetWriter(string contentType, Type handlerType, Type dataType, out IContentTypeWriter writer);
    }
}
