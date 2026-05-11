using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebForms_MovieManager
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            Application["StartTime"] = DateTime.Now;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void Application_End(object sender, EventArgs e)
        {

        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            Context.Response.AddHeader("X-Request-ID",Guid.NewGuid().ToString());
        }
        void Application_EndRequest(object sender, EventArgs e)
        {
            if(Context.Response.StatusCode >= 400)
            {
                LogHttpError(Context);
            }
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
            catch {}
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

        private bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttprequest" ||
                context.Request.Params["IsAjax"] == "true";
        }

        private void RedirectToErrorPage(Exception ex, HttpContext context)
        {
            context.Session["LastError"] = ex;
            context.Session["LastErrorUrl"] = context.Request.Url.ToString();

            context.Response.Redirect("~/ErrorPages/GlobalError.aspx");
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
            logEntry.AppendLine($"Error type: {ex.GetType().Name}");
            logEntry.AppendLine($"Message: {ex.Message}");
            logEntry.AppendLine($"Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                logEntry.AppendLine($"Inner exception: {ex.InnerException.Message}");
            }

            if(context != null && context.Request != null)
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
                var logPath = Server.MapPath("~/App_Data/ErrorLog.txt");
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
