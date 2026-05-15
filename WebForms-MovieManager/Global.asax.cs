using System;

using System.Diagnostics;

using System.IO;

using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;


namespace WebForms_MovieManager
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            try
            {
                // Code that runs on application startup
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                Application["RequestStartTime"] = DateTime.Now;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {

                // Log startup errors to a file (since nothing else is available yet)
                string startupErrorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "StartupError.log");
                Directory.CreateDirectory(Path.GetDirectoryName(startupErrorPath));
                File.WriteAllText(startupErrorPath, $"{DateTime.Now}: {ex.ToString()}");
            }
            
        }

        

        void Application_BeginRequest(object sender, EventArgs e)
        {
            string requestId = Guid.NewGuid().ToString();
            Context.Items["RequestId"] = requestId;  
            Context.Response.AddHeader("X-Request-ID", requestId);
        }
        void Application_EndRequest(object sender, EventArgs e)
        {
            if(Context.Response.StatusCode >= 400)
            {
                LogHttpError(Context);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null) 
            { 
                LogException(ex,HttpContext.Current);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Get the exception that caused the error
            Exception ex = Server.GetLastError();

            //Get the http context
            HttpContext context = HttpContext.Current;

            //Log the exception
            LogException(ex, context);

            //clear the errir to orevent default yellow screen
            Server.ClearError();

            //check if it is an AJAX request
            if (IsAjaxRequest(Context))
            {
                HandleAjaxError(ex, context);
            }
            else
            {
                //redirect to custom error page
                RedirectToErrorPage(ex, context);
            }
        }
        private void RedirectToErrorPage(Exception ex, HttpContext context)
        {
            try
            {
                // Ensure response is valid
                if (context.Response == null)
                    return;

                // Try to store error details
                try
                {
                    if (context.Session != null)
                    {
                        if (ex != null)
                            context.Session["LastError"] = ex;
                        context.Session["LastErrorUrl"] = context.Request?.Url?.ToString() ?? "Unknown URL";
                    }
                }
                catch { /* Ignore session errors */ }

                // Check if we can redirect
                if (context.Response.IsClientConnected && !context.Response.IsRequestBeingRedirected)
                {
                    string redirectUrl = context.Session != null
                        ? "~/ErrorPages/GlobalError.aspx"
                        : "~/ErrorPages/StaticError.html";

                    context.Response.Redirect(redirectUrl, false);
                    context.Response.End();
                }
                else
                {
                    // Write direct response
                    context.Response.Clear();
                    context.Response.ContentType = "text/html";
                    context.Response.Write(GetSimpleErrorHtml(ex?.Message ?? "Application Error"));
                    context.Response.StatusCode = 500;
                    context.Response.End();
                }

            }
            catch (Exception redirectEx)
            {

                // Ultimate fallback - write simple error
                try
                {
                    context.Response.Clear();
                    context.Response.Write(GetSimpleErrorHtml("Application Error"));
                    context.Response.StatusCode = 500;
                    context.Response.End();
                }
                catch { /* Give up */ }
            }
        }

        private string GetSimpleErrorHtml(string errorMessage)
        {
            return $@"<!DOCTYPE html>
                <html>
                <head><title>Application Error</title></head>
                <body>
                    <h1>Application Error</h1>
                    <p>Sorry, an error occurred while processing your request. {errorMessage}</p>
                    <p><a href=""/"">Return to Home</a></p>
                </body>
                </html>";
        }

        void Application_End(object sender, EventArgs e)
        {

        }

        private void LogHttpError(HttpContext context)
        {
            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"HTTP Error: {context.Response.StatusCode}");
                logEntry.AppendLine($"URL: {context.Request.Url}");
                logEntry.AppendLine($"Timestamp: {DateTime.Now}");

                WriteToLogFile(logEntry.ToString());
            }
            catch { }
        }
        private bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttprequest" ||
                context.Request.Params["IsAjax"] == "true";
        }

        private void HandleAjaxError(Exception ex, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var errorResponse = new
            {
                success = false,
                message = "An error occured while processing your request",
                error = ex.Message,
                StackTrace = context.IsDebuggingEnabled ? ex.StackTrace : null
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse);
            context.Response.Write(json);
        }

        private void LogException(Exception ex, HttpContext context)
        {
            var logEntry = new StringBuilder();
            logEntry.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            if (ex != null)
            {
                logEntry.AppendLine($"Error type: {ex.GetType().Name}");
                logEntry.AppendLine($"Message: {ex.Message}");
                logEntry.AppendLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    logEntry.AppendLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            else
            {
                logEntry.AppendLine("Exception is null");
            }




            if (context != null && context.Request != null)
            {
                logEntry.AppendLine($"Url: {context.Request.Url}");
                logEntry.AppendLine($"User IP: {context.Request.UserHostAddress}");
                logEntry.AppendLine($"User Agent: {context.Request.UserAgent}");
                logEntry.AppendLine($"Http Method: {context.Request.HttpMethod}");

                if (context.User != null && context.User.Identity.IsAuthenticated)
                {
                    logEntry.AppendLine($"User: {context.User.Identity.Name}");
                }
            }

            logEntry.AppendLine(new string('-',80));

            //write to windows event log
            WriteToEventLog(logEntry.ToString());


        }
        private void WriteToLogFile(string message)
        {
            try
            {
                var logPath = Server.MapPath("~/App_Data/Logs/ErrorLog.txt");
                var directory = Path.GetDirectoryName(logPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.AppendAllText(logPath, message + Environment.NewLine);

                CleanOldLogFiles(directory);
            }
            catch {}           
        }

        private void CleanOldLogFiles(string directory)
        {
            try
            {
                var files = Directory.GetFiles(directory, "ErrorLog*.txt");
                var cutOff = DateTime.Now.AddDays(-30);

                foreach (var file in files)
                {
                    if(File.GetCreationTime(file) < cutOff)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch{}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void WriteToEventLog(string message)
        {
            try
            {
                string source = "MovieManager";
                string log = "Application";

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog.WriteEntry(source, message, EventLogEntryType.Error);
            }
            catch {}
           
        }
    }
}
