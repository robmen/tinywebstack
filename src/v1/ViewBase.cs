using System.Web;

namespace TinyWebStack.v1
{
    public abstract class ViewBase
    {
        public abstract void Execute(HttpContextBase context);
    }
}
