
using System;

using System.Web;
using WebForms_MovieManager.Services;

namespace WebForms_MovieManager.Modules
{
    public class GlobalErrorHandlerModule : IHttpModule
    {
        private static IErrorLogger _logger;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            _logger = new ErrorLogger();
            context.Error += OnError;
            context.BeginRequest += Context_BeginRequest;
            context.EndRequest += Context_EndRequest;

        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            if (app.Context.Items["RequestStartTime"] is DateTime startTime)
            {
                var duration = DateTime.Now - startTime;

                if (duration.TotalSeconds > 5)
                {
                    _logger.LogWarning($"Slow request detected: " +
                        $"{app.Request.Url} took {duration.TotalSeconds:F2} seconds");
                }
            }
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            string requestId = Guid.NewGuid().ToString();
            app.Context.Items["RequestId"] = requestId;
            app.Context.Response.AddHeader("X-Request-ID", requestId);
        }

        private void OnError(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var ex = app.Server.GetLastError();

            if (ex != null)
            {
                _logger.LogError(ex, "Unhandled exception caught by GlobalErrorHandlerModule");

                // CLear sensitive data for security
                if (!app.Context.IsDebuggingEnabled && ex is HttpException httpEx)
                {
                    app.Server.ClearError();

                    switch (httpEx.GetHttpCode())
                    {
                        case 404:
                            app.Response.Redirect("~/Errorpages/404.aspx");
                            break;
                        case 403:
                            app.Response.Redirect("~/ErrorPages/403.aspx");
                            break;
                        default:
                            app.Response.Redirect("~/Errorpages/GlobalError.aspx");
                            break;
                    }

                }

            }
        }
    }
}