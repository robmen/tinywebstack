
namespace TinyWebStack
{
    public class FileTransmission
    {
        public FileTransmission(string contentType, string path)
        {
            this.ContentType = contentType;

            this.Path = path;
        }

        public string ContentType { get; set; }

        public string Path { get; set; }
    }
}
