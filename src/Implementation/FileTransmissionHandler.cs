
namespace TinyWebStack.Implementation
{
    [ContentType("?")]
    internal class FileTransmissionHandler : IContentTypeHandler
    {
        public bool TryGetWriter(string contentType, System.Type handlerType, System.Type dataType, out IContentTypeWriter writer)
        {
            writer = (typeof(FileTransmission).IsAssignableFrom(dataType)) ? new FileTransmissionWriter() : null;

            return writer != null;
        }
    }
}
