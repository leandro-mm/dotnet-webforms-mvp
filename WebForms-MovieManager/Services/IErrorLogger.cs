
using System;

namespace WebForms_MovieManager.Services
{
    public interface IErrorLogger
    {
        void LogError(Exception ex, string aditionalInfo = null);
        void LogError(string message, ErrorSeverity errorSeverity = ErrorSeverity.Error);
        void LogWarning(string message);
        void LogInformation(string message);
        
    }

    public enum ErrorSeverity
    {
        Information,
        Warning,
        Error,
        Critical
    }
}