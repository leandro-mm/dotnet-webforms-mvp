using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForms_MovieManager.ErrorPages
{
    public partial class GlobalError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string errorId = Guid.NewGuid().ToString().Substring(0, 8);
            lblErrorld.Text = errorId;

            if (!IsPostBack)
            {
                DisplayErrorDetails();
            }
        }

        private void DisplayErrorDetails()
        {
            Exception lastError = Session["LastError"] as Exception;
            string lastErrorUrl = Session["LastErrorUrl"] as string;

            if (lastError != null)
            {
                // Show user-friendLy message
                string userMessage = GetUserFriendlyMessage(lastError);
                lblErrorMessage.Text = userMessage;
                lblErrorMessage.Visible = true;

                // Show techmlcaL detaiLs (onLy for authenticated users or in debug mode)
                if (IsUserAdmin() || Context.IsDebuggingEnabled)
                {
                    var details = new StringBuilder();

                    details.AppendLine($"<strong>Error ID:</strong> {lblErrorld.Text}<br /> ");
                    details.AppendLine($"<strong>URL:</strong> {lastErrorUrl}<br />");
                    details.AppendLine($"<strong>Exception Type:</strong> {lastError.GetType().Name}<br />");
                    details.AppendLine($"<strong>Message:</strong> {lastError.Message}<br /> ");
                    details.AppendLine($"<strong>stack Trace:</strong><br />{lastError.StackTrace?.Replace(Environment.NewLine, "<br />")}<br />");

                    if (lastError.InnerException != null)
                    {
                        details.AppendLine($"<strong>Inner Exception:</strong> {lastError.InnerException.Message}<br />");
                    }

                    Literal1.Text = details.ToString();
                }
                else
                {
                    Literal1.Text = "Technical details are hidden for security reasons.Please contact support with the Error ID.";
                }

                // CLeGr session to prevent re-dispLay
                Session.Remove("LastError");
                Session.Remove("LastErrorUrl");
            }

        }

        private bool IsUserAdmin()
        {
            // ImpLement your admin check Logic
            return Context.User != null && Context.User.IsInRole("Admin");
        }

        private string GetUserFriendlyMessage(Exception ex)
        {
            if (ex is System.Data.SqlClient.SqlException)
            {
                return "A database error occurred. Please try again later.";
            }
            else if (ex is UnauthorizedAccessException) 
            {
                return "You don't have permission to access this resource.";
            }
            else if (ex is HttpException httpEx && httpEx.GetHttpCode() == 404)
            {
                return "The page you requested could not be found.";
            }
            else if (ex is System.ArgumentException)
            {
                return "Invalid data was provided. Please check your input.";
            }
            else
            {
                return "An unexpected error occurred. Our team has been notified.";
            }
        }

        protected void btnReportError_Click(object sender, EventArgs e)
        {
            try
            {
                // Send error report via emaiL or API
                SendErrorReport();
                lblErrorMessage.Text = "Error report sent successfully. well look into it shortly.";
                lblErrorMessage.CssClass = "error-message success";
            }
            catch 
            {

                lblErrorMessage.Text = "Failed to send error report. Please contact support directly";
            }
        }

        private void SendErrorReport()
        {
            // ImpLement error reporting Logic (emaiL, API, etc.)
            // This is o pLacehoLder for actuaL impLementation
        }
    }
}