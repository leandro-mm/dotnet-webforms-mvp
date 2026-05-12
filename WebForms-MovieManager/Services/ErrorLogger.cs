
using System;

using System.Diagnostics;

using System.IO;

using System.Text;
using System.Web;

namespace WebForms_MovieManager.Services
{
    public class ErrorLogger : IErrorLogger
    {
        private readonly string _logPath;
        private readonly object _lockObject = new object();

        public ErrorLogger()
        {
            string basePath = HttpContext
                                .Current?
                                .Server
                                .MapPath("~/App_Data/Logs") ?? AppDomain.CurrentDomain.BaseDirectory;

            _logPath = Path.Combine(basePath, "ErrorLog");

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }
        private bool IsCriticalError(Exception ex)
        {
            return ex is OutOfMemoryException ||
                ex is StackOverflowException ||
                ex is AccessViolationException ||
                ex is System.Data.SqlClient.SqlException sqlEx && sqlEx.Number > 50_000;
        }
        public void LogError(Exception ex, string aditionalInfo = null)
        {
           var logEntry = BuildExceptionLogEntry(ex, aditionalInfo);
            WriteToFile(logEntry);
            WriteToEventLog(logEntry);

            if (IsCriticalError(ex))
            {
                SendCriticalErrorNotification(ex, aditionalInfo);
            }
        }

        private StringBuilder BuildExceptionLogEntry(Exception ex, string additionalInfo)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=".PadRight(80,'='));
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Error ID: {Guid.NewGuid()}");
            sb.AppendLine($"Severity: {(IsCriticalError(ex)?"CRITICAL":"ERROR")}");
            sb.AppendLine($"Error type: {ex.GetType().Name}");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"Target Site: {ex.TargetSite}");
            sb.AppendLine($"Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
                sb.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
            }

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                sb.AppendLine($"Aditional Info: {additionalInfo}");
            }

            if (HttpContext.Current != null)
            {
                var context = HttpContext.Current;
                sb.AppendLine($"URL: {context.Request.Url}");
                sb.AppendLine($"HTTP Method: {context.Request.HttpMethod}");
                sb.AppendLine($"User IP: {context.Request.UserHostAddress}");
                sb.AppendLine($"User Agent: {context.Request.UserAgent}");

                if (context.User != null && context.User.Identity.IsAuthenticated)
                {
                    sb.AppendLine($"User: {context.User.Identity.Name}");
                }

                //form data
                if (context.Request.Form.Count > 0)
                {
                    sb.AppendLine("Form Data:");
                    foreach (string key in context.Request.Form.Keys)
                    {
                        if (!key.ToLower().Contains("password") &&
                            !key.ToLower().Contains("card"))
                        {
                            sb.AppendLine($" {key}: {context.Request.Form[key]}");
                        }
                        else
                        {
                            sb.AppendLine($" {key}: [REDACTED]");
                        }
                    }
                    
                }
                
            }
            sb.AppendLine("=".PadRight(80, '='));
            return sb;
        }

        public void LogError(string message, ErrorSeverity errorSeverity = ErrorSeverity.Error)
        {
            var logEntry = BuildMessageLogEntry(message, errorSeverity);

            WriteToFile(logEntry);

            if (errorSeverity == ErrorSeverity.Critical)
            {
                WriteToEventLog(logEntry);
                SendCriticalErrorNotification(new Exception(message), null);
            }
        }

        private void SendCriticalErrorNotification(Exception exception, object value)
        {
            try
            {
                // ExampLe: Send emaiL
                // EmaiL5ervice . SendErrorALert(ex, additionaLInfo);
            }
            catch {}
        }

        private void WriteToFile(object logEntry)
        {
            try
            {
                lock (_lockObject)
                {
                    string filename = $"error_log_{DateTime.Now:yyyyMMdd}.txt";
                    string fullPath = Path.Combine(_logPath, filename);
                    File.AppendAllText(fullPath, logEntry.ToString() + Environment.NewLine);

                    //Archive oLd fiLes
                    ArchiveOldLogs();
                }
            }
            catch{}
        }

        private void ArchiveOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(_logPath, "error_log_*.txt");
                var cutOff = DateTime.Now.AddDays(-30);

                foreach (var file in files)
                {
                    if (File.GetCreationTime(file) < cutOff)
                    {
                        string archivepath = Path.Combine(_logPath, "Archive");

                        if (!Directory.Exists(archivepath))
                        {
                            Directory.CreateDirectory(archivepath);
                        }

                        string destFile = Path.Combine(archivepath, Path.GetFileName(file));
                        File.Move(file, destFile);
                    }
                }
            }
            catch { }
        }

        private void WriteToEventLog(StringBuilder logEntry)
        {
            try
            {
                string source = "MovieManager";
                string log = "Application";

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog.WriteEntry(source, logEntry.ToString(), EventLogEntryType.Error);
            }
            catch{}
        }        

        private StringBuilder BuildMessageLogEntry(string message, object severity)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-".PadRight(80, '-'));
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Severity: {severity}");
            sb.AppendLine($"Message: {message}");
            sb.AppendLine("-".PadRight(80, '-'));
            return sb;
        }

        public void LogInformation(string message)
        {
            LogError(message, ErrorSeverity.Information);
        }

        public void LogWarning(string message)
        {
            LogError(message, ErrorSeverity.Warning);
        }
    }
}