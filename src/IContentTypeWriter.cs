using System.IO;

namespace TinyWebStack
{
    /// <summary>
    /// Interface to write content to response.
    /// </summary>
    public interface IContentTypeWriter
    {
        /// <summary>
        /// Content type for the output.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Writes the data to the writer.
        /// </summary>
        /// <param name="writer">Text writer to output data.</param>
        /// <param name="data">Data from handler to output.</param>
        void Write(TextWriter writer, object data);
    }
}
