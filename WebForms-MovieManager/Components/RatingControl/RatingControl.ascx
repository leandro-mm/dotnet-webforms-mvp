<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RatingControl.ascx.cs" Inherits="WebForms_MovieManager.Components.RatingControl.RatingControl" %>

<div class="rating-control" data-movie-id="<%= MovieId %>">

    <div class="rating-stars-container">

        <div class="stars">
            <% for (int i = 1; i <= 5; i++) { %>
                <span class="star" data-rating="<%= i %>">*</span>
            <% } %>
        </div>

        <asp:HiddenField ID="hdnRating" runat="server" Value="0" />

    </div>

    <div class="rating-info">
        <asp:Label ID="lblRatingDisplay" runat="server" Cssclass="rating-value" />
        <asp:Label ID="lblVotes" runat="server" Cssclass="votes-count" />
    </div>

    <div class="rating-actions">
        <asp:Button ID="btnSaveRating" runat="server" Text="Save Rating" CssClass="btn-save-rating" Visible="false" OnClick="btnSaveRating_Click" />
        <asp:Button ID="btnClearRating" runat="server" Text="Clear" CssClass="btn-clear-rating" Visible="false" OnClick="btnClearRating_Click" />
    </div>

    <div class="loading-overlay" id="loadingOverlay" runat="server" visible="false">
         <div class="spinner"></div>
    </div>

</div>