
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MovieSearchView.ascx.cs" Inherits="WebForms_MovieManager.Components.MovieSearch.MovieSearchView" %>

<link href="../../Content/MovieSearch.css" rel="stylesheet" />

<div class="movie-search-component">
    <div class="search-header">
        <h3>Search Movies</h3>

        <asp:Button ID="btnToggleAdvanced" 
            runat="server" 
            Text="Advanced Search"
            Cssclass="btn-toggle" 
            OnClick="btnTogg1eAdvanced_Click" />
    </div>

    <div class="search-basic">
        <div class="search-input-group">
            <asp:TextBox ID="txtSearchTerm" 
                runat="server" 
                placeholder="search by title or director..."
                CssClass="search-input" />

            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-search" OnClick="btnSearch_Click" />

            <asp:Button ID="btnClear" runat="server" Text="Clear" Cssclass="btn-clear" OnClick="btnClear_Click" />
        </div>
    </div>

    <div class="search-advanced" 
        id="divAdvancedSearch" 
        runat="server" style="display: none;">

        <div class="filter-group"> 
            <label>Genre:</label>
            <asp:DropDownList ID="ddlGenre" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlGenre_SelectedIndexChanged" />
        </div>

        <div class="filter-group">
            <label>year:</label>
            <asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
                <asp:ListItem Text="All Years" Value="" />
            </asp:DropDownList>
        </div>

        <div class="filter-group">
            <label>Min Rating:</label>
            <asp:DropDownList ID="ddlRating" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRating_SelectedIndexChanged">
                <asp:ListItem Text="Any Rating" Value="" />
                <asp:ListItem Text="* 5+ (5.0)" Value="5" />
                <asp:ListItem Text="** 6+ (6.0)" Value="6" />
                <asp:ListItem Text="*** 7+ (7.0)" Value="7" />
                <asp:ListItem Text="**** 8+ (8.0)" Value="8" />
                <asp:ListItem Text="***** 9+ (9.0)" Value="9" />
            </asp:DropDownList>
        </div>
    </div>

    <div class="search-results-info">
        <asp:Label ID="lblResultsCount" runat="server" CsClass="results-count" />

        <div class="loading-indicator" id="loadingIndicator" runat="server" visible="false">
            Loading...
        </div>
    </div>

    <div class="search-results">
        <asp:Repeater ID="rptResults" runat="server">

            <HeaderTemplate>
                <div class="movie-grid"></div>
            </HeaderTemplate>

            <ItemTemplate>
                <div class="movie-card" data-movie-id='<%#Eval("Id") %>'>
                    
                    <div class="movie-title">
                        <%# HighlightSearchTerm(Eval("MovieTitle").ToString())%>
                        </div>
                    <div class="movie-director">Director: <%# Eval("Director") %></div>

                    <div class="movie-year">year: <%# Eval("ReleaseYear") %></div>
                    <div class="movie-genre">Genre: <%# Eval("Genre") %></div>
                    <div class="movie-rating">Rating: <%# Eval("Rating") %> *</div>

                </div>

            </ItemTemplate>

            <FooterTemplate>
                <div></div>
            </FooterTemplate>                

        </asp:Repeater>

        <asp:Label ID="lblNoResults" 
            runat="server" 
            Text="no movies found matching your criteria"
            CssClass="no-results" Visible="false">

        </asp:Label>
    </div>
</div>