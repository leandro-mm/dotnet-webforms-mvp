using System.Web;

namespace WebForms_MovieManager.Services
{
    public interface IHttpContextAccessor
    {
        HttpContextBase Current { get; }
    }
}