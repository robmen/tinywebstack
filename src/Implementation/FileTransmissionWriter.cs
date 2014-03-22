using System.IO;
using System.Web;
using TinyWebStack.Models;

namespace TinyWebStack.Implementation
{
    public class FileTransmissionWriter : IContentTypeWriter
    {
        public string ContentType { get; private set; }

        public void Write(TextWriter writer, object data)
        {
            var transmission = data as FileTransmission;

            this.ContentType = transmission.ContentType;

            HttpContext.Current.Response.TransmitFile(transmission.Path); // TODO: provide an interface instead of just the TextWriter.
        }
    }
}
