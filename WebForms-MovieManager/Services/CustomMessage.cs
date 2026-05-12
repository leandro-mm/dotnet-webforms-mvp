using System;
using System.Web;

namespace WebForms_MovieManager.Services
{
    public static class CustomMessage
    {
        public static string FriendlyMessageGlobalError(Exception ex)
        {
            switch (ex)
            {
                case System.Data.SqlClient.SqlException _:
                    return "A database error occurred. Please try again later.";
                case UnauthorizedAccessException _:
                    return "You don't have permission to access this resource.";
                case HttpException httpEx when httpEx.GetHttpCode() == 404:
                    return "The page you requested could not be found.";
                case ArgumentException _:
                    return "Invalid data was provided. Please check your input.";
                default:
                    return "An unexpected error occurred. Our team has been notified.";
            }
        }

        public static string FriendlyMessagePresenter(Exception ex)
        {
            switch (ex)
            {
                case ArgumentException _:
                    return "Invalid data provided. Please check your input";
                case InvalidOperationException _:
                    return "Operation could not be completed. Please try again.";
                case UnauthorizedAccessException _:
                    return "You don't have permission";
                case TimeoutException _:
                    return "The operation timed out.";
                default:
                    return "An error occured while processing your request.";
            }            
        }
    }
}