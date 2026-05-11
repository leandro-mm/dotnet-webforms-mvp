<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GlobalError.aspx.cs" Inherits="WebForms_MovieManager.ErrorPages.GlobalError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Application Error</title>
    <link href="../Content/gobalerror.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <div class="error-code"> 
                XYZ
            </div>
            <h1>Something Went Wrong</h1>
            <div class="error-message">
                <p>we're sorry, but an unexpected error has occurred.</p>
                <p>our technical team has been notified and is working to resolve the issue.</p>
            </div>
        </div>

        <asp:Label ID="lblErrorMessage" 
            runat="server" 
            Cssclass="error-message" 
            Visible="false" />

        <div class="details" id="errorDetails">
            <strong>Error Details:</strong><br />
            <asp:Literal ID="Literal1" runat="server" />
        </div>

        <button type="button" class="show-details" onclick="toggleDetails()">
Show Technical Details
</button>

        <div class="buttons">
            <a href="javascript:history.back()" class="btn bin-secondary">Go Back</a>
            <a href="~/MovieManagement.aspx" class="btn bin-primary">Go to Home Page</a>
            <asp:LinkButton ID="btnReportError" runat="server" Cssclass="btn bin-danger" OnClick="btnReportError_Click">Report This Error</asp:LinkButton>
        </div>

        <div class="support-info">
            <p>Error Reference: 
                <asp:Label ID="lblErrorld" runat="server" Text="">
            </p>
            <p>If the problem persists, please contact support at 
                <a free="mailto:support@moviemanager.com">support@moviemanager.com</a>
            </p>
        </div>

        <script>
            function toggleDetails() {
                var details = document.getElementByld('errorDetails');
                if (details.style.display === 'none') {
                    details.style.display = block;
                }
                else {
                    details.style.display = 'none';
                }
            }
        </script>
    </form>
</body>
</html>
