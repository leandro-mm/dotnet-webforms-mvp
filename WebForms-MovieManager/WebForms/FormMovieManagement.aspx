<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormMovieManagement.aspx.cs" Inherits="WebForms_MovieManager.WebForms.MovieManagement" %>

<%@ Register Src="~/Components/MovieSearch/MovieSearchView.ascx" TagPrefix="uc1" TagName="MovieSearchView" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Movie Management System - MVP Pattern</title>    
    <link href="../Content/FormMovieManagement.css" rel="stylesheet" />
</head>
<body>
    
        <!-- movie management form -->
         <form id="form1" runat="server">
              <div class="components-section">
             <!-- Movie Search Component -->
                <uc1:MovieSearchView runat="server" ID="MovieSearchView" />

             <div class="container">
                 <h1> Movie Management System (MVP Pattern) With Reusable Component</h1>

         
         
                 <asp:Label ID="lblMessage" 
                             runat="server" CssClass="message" Visible="false"></asp:Label>

                 <div class="form-section">
                     <h3>Movie Information</h3>
                     <asp:ValidationSummary ID="ValidationSummary1" runat="server"
                         CssClass ="validation-summary"
                         DisplayMode="BulletList"
                         HeaderText ="Please fix the following errors:"/>
                 </div>

                 <div class="form-group">
                     <label>Title:</label>
                     <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                     <asp:RequiredFieldValidator ID="rfvTitle" 
                         runat="server" 
                         ErrorMessage="Title is Required"
                         ControlToValidate="txtTitle"
                         Display="Dynamic"
                         ForeColor="Red">
                     </asp:RequiredFieldValidator>
                 </div>

                  <div class="form-group">
                      <label>Director:</label>
                      <asp:TextBox ID="txtDirector" runat="server"></asp:TextBox>
                      <asp:RequiredFieldValidator ID="rfvDirector" 
                          runat="server" 
                          ErrorMessage="Director is Required"
                          ControlToValidate="txtDirector"
                          Display="Dynamic"
                          ForeColor="Red">
                      </asp:RequiredFieldValidator>
                 </div>

                  <div class="form-group">
                      <label>Release Year:</label>
                      <asp:TextBox ID="txtReleaseYear" runat="server" TextMode="Number"></asp:TextBox>
                      <asp:RequiredFieldValidator ID="rfvReleaseYear" 
                          runat="server" 
                          ErrorMessage="Release year is Required"
                          ControlToValidate="txtReleaseYear"
                          Display="Dynamic"
                          ForeColor="Red">
                      </asp:RequiredFieldValidator>
                      <asp:RangeValidator ID="rvReleaseYear"
                          runat="server"
                          ErrorMessage="Year must be between 1888 and 2026"
                          ControlToValidate ="txtReleaseYear"
                          MinimumValue="1888"
                          MaximumValue="2026"
                          ForeColor="Red"
                          Type="Integer">

                      </asp:RangeValidator>
                 </div>
        
                 <div class="form-group">
                     <label>Genre:</label>
                     <asp:DropDownList ID="ddlGenre" runat="server">
                         <asp:ListItem Text="-- Select Genre --" Value=""/>
                         <asp:ListItem Text="Action" Value="Action"/>
                         <asp:ListItem Text="Comedy" Value="Comedy"/>
                         <asp:ListItem Text="Drama" Value="Drama"/>
                         <asp:ListItem Text="Horror" Value="Horror"/>
                         <asp:ListItem Text="Sci-Fi" Value="Sci-Fi"/>
                         <asp:ListItem Text="Romance" Value="Romance"/>
                         <asp:ListItem Text="Thriller" Value="Thriller"/>
                     </asp:DropDownList>
                      <asp:RequiredFieldValidator ID="rfvGenre" 
                          runat="server" 
                          ErrorMessage="Genre is Required"
                          ControlToValidate="ddlGenre"
                          Display="Dynamic"
                          ForeColor="Red"
                          InitialValue="">
                      </asp:RequiredFieldValidator>
                 </div>
         
                 <div class="form-group">
                     <label>Rating (0-10):</label>
                     <asp:TextBox ID="txtRating" runat="server" TextMode="Number"></asp:TextBox>
                      <asp:RangeValidator ID="rvRating"
                          runat="server"
                          ErrorMessage="Rating must be between 0 and 10"
                          ControlToValidate ="txtRating"
                          MinimumValue="0"
                          MaximumValue="10"
                          ForeColor="Red"
                          Type="Integer">

                      </asp:RangeValidator>
                 </div>

                 <div class="button-group">
                     <asp:Button ID="btnAdd" runat="server" Text="Add Movie" OnClick="btnAdd_Click" CssClass="button-add" />
                     <asp:Button ID="btnUpdate" runat="server" Text="Update Movie" Visible="false" OnClick="btnUpdate_Click" CssClass="button-update" />
                     <asp:Button ID="btnDelete" runat="server" Text="Delete Movie" OnClick="btnDelete_Click" CssClass="button-delete" />
                     <asp:Button ID="btnClear" runat="server" Text="Clear Form" OnClick="btnClear_Click" CssClass="button-clear" />
                     <asp:Button ID="btnLoad" runat="server" Text="Load Movies" OnClick="btnLoad_Click" CssClass="btn-load" />
                     <asp:HiddenField ID="hdnMovieId" runat="server" Value="" />
                 </div>
    
             <div class="movie-grid">
                 
                 <asp:GridView ID="gvMovies" runat="server" 
                     AutoGenerateColumns="false"
                     CssClass="grid-view"
                     OnSelectedIndexChanged="gvMovies_SelectedIndexChanged"
                     DataKeyNames="Id">
                     <Columns>
                         <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" />
                         <asp:BoundField DataField="MovieTitle" HeaderText="Title"/>
                         <asp:BoundField DataField="Director" HeaderText="Director"/>
                         <asp:BoundField DataField="ReleaseYear" HeaderText="ReleaseYear"/>
                         <asp:BoundField DataField="Genre" HeaderText="Genre"/>
                         <asp:BoundField DataField="Rating" HeaderText="Rating"/>
                         <asp:BoundField DataField="CreatedDate" HeaderText="Added Date" DataFormatString="{0:MM/dd/yyyy}"/>
                         <asp:CommandField 
                             ShowSelectButton="true" 
                             SelectText="Edit" 
                             ButtonType="Button"
                             ControlStyle-CssClass="btn-edit"/>
                     </Columns>

                     <EmptyDataTemplate>
                         <div class="empty-data-template">No Movies Found.</div>
                     </EmptyDataTemplate>

                 </asp:GridView>
             </div>    
             </div>
             </div>
         </form>
        <!-- end movie form -->
    
</body>
</html>
