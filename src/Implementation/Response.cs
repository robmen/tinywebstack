using System.Web;

namespace TinyWebStack.Implementation
{
    internal class Response : IResponse
    {
        public Response()
        {
            this.HttpResponse = HttpContext.Current.Response;
        }

        private HttpResponse HttpResponse { get; set; }
    }
}
