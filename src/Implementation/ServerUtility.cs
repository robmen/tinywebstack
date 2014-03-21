using System.Web;

namespace TinyWebStack.Implementation
{
    internal class ServerUtility : IServerUtility
    {
        public ServerUtility()
        {
            this.Server = HttpContext.Current.Server;
        }

        private HttpServerUtility Server { get; set; }

        public string MapPath(string path)
        {
            return this.Server.MapPath(path);
        }
    }
}
