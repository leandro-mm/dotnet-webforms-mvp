using System.Web;

namespace WebForms_MovieManager.Services
{
    public class HttpContextAccessor : IHttpContextAccessor
    {
        public HttpContextBase Current
        {
            get
            {
                var context = HttpContext.Current;
                return context != null ? new HttpContextWrapper(context) : null;
            }
        }
    }
}